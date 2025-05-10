package com.milosev.kanaloa.ui.home

import android.content.Context
import android.net.Uri
import androidx.fragment.app.FragmentActivity
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.Marker
import com.google.android.gms.maps.model.MarkerOptions
import com.google.maps.android.data.kml.KmlLayer
import com.milosev.kanaloa.Config
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.logger.LoggingEventType
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.Request
import org.json.JSONObject
import java.net.URL
import androidx.core.net.toUri

class LiveLocationUpdater(
    private val map: GoogleMap,
    private val client: OkHttpClient,
    private val coroutineScope: CoroutineScope
) {
    var marker: Marker? = null
    private var updateJob: Job? = null

    fun start(
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity,
        logViewModelLogger: LogViewModelLogger,
        kmlUrl: String
    ) {
        val url = context?.let { Config(it).webHost };
        updateJob = coroutineScope.launch {
            while (isActive) {
                try {
                    loadKmlFromUrl(kmlUrl, googleMap, context, requireActivity, logViewModelLogger)

                    val newLocation = fetchLiveLocation(url)
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

                val sharedPreferences = context?.getSharedPreferences("settings", Context.MODE_PRIVATE)
                val intervalString = sharedPreferences?.getString("requestUpdates", "30") ?: "30"
                val updateInterval = intervalString.toLongOrNull()?.times(1000) ?: 30_000L
                delay(updateInterval)
            }
        }
    }

    fun stop() {
        updateJob?.cancel()
    }

    private suspend fun fetchLiveLocation(url: String?): LatLng? = withContext(Dispatchers.IO) {
        val fullUrl = url?.toUri()
            ?.buildUpon()
            ?.appendPath("live.json")
            ?.build()
            .toString()

        val request = fullUrl.let {
            Request.Builder()
                .url(it)
                .build()
        }

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
