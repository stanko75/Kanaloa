package com.milosev.kanaloa.ui.home

import android.content.Context
import androidx.fragment.app.FragmentActivity
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.Marker
import com.google.android.gms.maps.model.MarkerOptions
import com.google.maps.android.data.kml.KmlLayer
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.logger.LoggingEventType
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.Request
import org.json.JSONObject
import java.net.URL

class LiveLocationUpdater(
    private val map: GoogleMap,
    private val client: OkHttpClient,
    private val coroutineScope: CoroutineScope
) {
    var marker: Marker? = null
    private val updateInterval = 30_000L

    fun start(
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity,
        logViewModelLogger: LogViewModelLogger,
        kmlUrl: String
    ) {
        coroutineScope.launch {
            while (isActive) {
                try {
                    loadKmlFromUrl(kmlUrl, googleMap, context, requireActivity, logViewModelLogger)

                    val newLocation = fetchLiveLocation()
                    newLocation?.let {
                        withContext(Dispatchers.Main) {
                            if (marker == null) {
                                marker = map.addMarker(
                                    MarkerOptions()
                                        .position(it)
                                        .title("Live Marker")
                                )
                            } else {
                                marker?.position = it
                                marker?.title = "Updated at ${System.currentTimeMillis() / 1000}"
                            }
                            googleMap.moveCamera(CameraUpdateFactory.newLatLngZoom(it, 15f))
                        }
                    }
                } catch (e: Exception) {
                    logViewModelLogger.Log(LogEntry(LoggingEventType.Error, e.message, e))
                }

                delay(updateInterval)
            }
        }
    }

    private suspend fun fetchLiveLocation(): LatLng? = withContext(Dispatchers.IO) {
        val request = Request.Builder()
            .url("https://kanaloa.azurewebsites.net/live.json")
            .build()

        client.newCall(request).execute().use { response ->
            if (response.isSuccessful) {
                val json = JSONObject(response.body.string())
                val lat = json.getDouble("lat")
                val lng = json.getDouble("lng")
                return@withContext LatLng(lat, lng)
            }
        }
        null
    }

    private fun loadKmlFromUrl(url: String, googleMap: GoogleMap, context: Context?, requireActivity: FragmentActivity, logViewModelLogger: LogViewModelLogger) {
        Thread {
            try {
                val inputStream = URL(url).openStream()
                val kmlLayer = KmlLayer(googleMap, inputStream, context)
                requireActivity.runOnUiThread {
                    kmlLayer.addLayerToMap()
                }
            } catch (e: Exception) {
                logViewModelLogger.Log(LogEntry(LoggingEventType.Error, e.message, e))
            }
        }.start()
    }
}
