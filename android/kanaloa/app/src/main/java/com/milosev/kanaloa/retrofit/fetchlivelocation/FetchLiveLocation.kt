package com.milosev.kanaloa.retrofit.fetchlivelocation

import android.content.Context
import com.google.android.gms.maps.model.LatLng
import com.milosev.kanaloa.logger.ILogger
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LoggingEventType
import androidx.core.net.toUri
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.model.Marker
import com.google.android.gms.maps.model.MarkerOptions
import okhttp3.ResponseBody
import org.json.JSONObject
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class FetchLiveLocation(
    private var getLiveLocation: IGetLiveLocationApiService,
    var logViewModelLogger: ILogger,
    var marker: Marker? = null
) : IFetchLiveLocation {
    override fun fetchLiveLocation(
        url: String?,
        googleMap: GoogleMap
    ) {

        val fullUrl = url?.toUri()
            ?.buildUpon()
            ?.appendPath("live.json")
            ?.build()
            .toString()

        fetchLiveLocation2(fullUrl, googleMap)
    }

    private fun moveGoogleMapCameraAndCreateMarkerIfNotExists(
        marker: Marker?,
        googleMap: GoogleMap,
        latLng: LatLng
    ): Marker {
        val updatedMarker = marker ?: googleMap.addMarker(
            MarkerOptions()
                .position(latLng)
                .title("Live Marker")
        )

        updatedMarker?.position = latLng
        updatedMarker?.title = "Updated at ${System.currentTimeMillis() / 1000}"
        googleMap.moveCamera(CameraUpdateFactory.newLatLngZoom(latLng, 15f))

        return updatedMarker!!
    }

    private fun fetchLiveLocation2(fullUrl: String, googleMap: GoogleMap) {

        var lat: Double
        var lng: Double

        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                "Request: $fullUrl"
            )
        )

        val webApiRequest = getLiveLocation.getLiveLocation(fullUrl)
        webApiRequest.enqueue(object : Callback<ResponseBody> {
            override fun onResponse(
                p0: Call<ResponseBody>,
                response: Response<ResponseBody>
            ) {
                if (response.isSuccessful) {
                    val liveLocationContent = response.body()?.string()

                    if (liveLocationContent.isNullOrEmpty()) {
                        logViewModelLogger.Log(
                            LogEntry(
                                LoggingEventType.Error,
                                "Live location JSON is empty!"
                            )
                        )
                    } else {
                        logViewModelLogger.Log(
                            LogEntry(
                                LoggingEventType.Information,
                                "Live location $liveLocationContent from $fullUrl loaded"
                            )
                        )

                        val json = JSONObject(liveLocationContent)
                        lat = json.getDouble("lat")
                        lng = json.getDouble("lng")

                        marker = moveGoogleMapCameraAndCreateMarkerIfNotExists(
                            marker,
                            googleMap,
                            LatLng(lat, lng)
                        )
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
                    } else {
                        logViewModelLogger.Log(
                            LogEntry(
                                LoggingEventType.Error,
                                "${sendResponse}${response.message()}"
                            )
                        )
                    }
                }
            }

            override fun onFailure(p0: Call<ResponseBody>, t: Throwable) {
                logViewModelLogger.Log(
                    LogEntry(
                        LoggingEventType.Error,
                        t.message.toString(),
                    )
                )
            }
        })
    }
}