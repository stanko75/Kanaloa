package com.milosev.kanaloa.ui.home

import android.content.Context
import androidx.fragment.app.FragmentActivity
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.Marker
import com.google.android.gms.maps.model.MarkerOptions
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
import androidx.core.net.toUri
import com.google.maps.android.data.kml.KmlLayer
import com.milosev.kanaloa.retrofit.CreateRetrofitBuilder
import com.milosev.kanaloa.retrofit.IGetKml
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

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

                    loadLocationFromUrlAndMoveCamera(url, googleMap)
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

    suspend fun loadLocationFromUrlAndMoveCamera(
        url: String?,
        googleMap: GoogleMap
    ) {
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

        /*
        client.newCall(request).execute().use { response ->
            if (response.isSuccessful) {
                val json = JSONObject(response.body.string())
                val lat = json.getDouble("lat")
                val lng = json.getDouble("lng")
                return@withContext LatLng(lat, lng)
            }
        }
         */
        null
    }

    fun loadKmlFromUrl(url: String, googleMap: GoogleMap, context: Context?, requireActivity: FragmentActivity, logViewModelLogger: LogViewModelLogger) {
        Thread {
            try {

                context?.let { Config(it).webHost }?.let {
                    val kmlClient = CreateRetrofitBuilder().createRetrofitBuilder(it)
                        .create(IGetKml::class.java)

                    logViewModelLogger.Log(
                        LogEntry(
                            LoggingEventType.Information,
                            "Request: $url"
                        )
                    )

                    val webApiRequest = kmlClient.getKml(url);

                    webApiRequest.enqueue(object : Callback<ResponseBody> {
                        override fun onResponse(
                            p0: Call<ResponseBody>,
                            response: Response<ResponseBody>
                        ) {
                            if (response.isSuccessful) {
                                val kmlContent = response.body()?.byteStream()

                                if (kmlContent == null) {
                                    logViewModelLogger.Log(
                                        LogEntry(
                                            LoggingEventType.Error,
                                            "KML is empty!"
                                        )
                                    )
                                } else {
                                    logViewModelLogger.Log(
                                        LogEntry(
                                            LoggingEventType.Information,
                                            "KML: $url loaded"
                                        )
                                    )
                                    val kmlLayer = KmlLayer(googleMap, kmlContent, context)
                                    requireActivity.runOnUiThread {
                                        kmlLayer.addLayerToMap()
                                    }
                                }

                            } else {
                                val sendResponse = "${response.code()}: "
                                if (response.errorBody()?.charStream() != null
                                    && response.errorBody()?.charStream()?.readText() != null
                                    && response.errorBody()!!.charStream().readText().isNotBlank()
                                ) {
                                    logViewModelLogger.Log(
                                        LogEntry(
                                            LoggingEventType.Error,
                                            "${sendResponse}${
                                                response.errorBody()!!.charStream().readText()
                                            }"
                                        )
                                    )
                                }
                                else {
                                    logViewModelLogger.Log(
                                        LogEntry(
                                            LoggingEventType.Error,
                                            "${sendResponse}${response.message()}"
                                        )
                                    )
                                }
                            }
                        }

                        override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                            logViewModelLogger.Log(
                                LogEntry(
                                    LoggingEventType.Error,
                                    t.message.toString(),
                                )
                            )
                        }
                    })
                }
            } catch (e: Exception) {
                logViewModelLogger.Log(LogEntry(LoggingEventType.Error, e.message, e))
            }
        }.start()
    }
}
