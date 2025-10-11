package com.milosev.kanaloa.retrofit.loadkmlfromurl

import android.content.Context
import androidx.fragment.app.FragmentActivity
import com.google.android.gms.maps.GoogleMap
import com.google.maps.android.data.kml.KmlLayer
import com.milosev.kanaloa.Config
import com.milosev.kanaloa.logger.ILogger
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LoggingEventType
import com.milosev.kanaloa.retrofit.CreateRetrofitBuilder
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class LoadKmlFromUrl(var logViewModelLogger: ILogger) : ILoadKmlFromUrl {
    override fun loadKmlFromUrl(
        url: String,
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity
    ) {
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

                            val body = response.body()
                            if (body == null) {
                                logViewModelLogger.Log(
                                    LogEntry(
                                        LoggingEventType.Error,
                                        "KML is empty (body==null)!"
                                    )
                                )
                                return
                            }

                            try {
                                val bytes = body.bytes()
                                if (bytes.isEmpty()) {
                                    logViewModelLogger.Log(
                                        LogEntry(
                                            LoggingEventType.Error,
                                            "KML is empty (0 Bytes)!"
                                        )
                                    )
                                    return
                                }

                                val head =
                                    bytes.copyOfRange(0, minOf(bytes.size, 200)).decodeToString()
                                if (!head.contains("<kml", ignoreCase = true)) {
                                    logViewModelLogger.Log(
                                        LogEntry(
                                            LoggingEventType.Warning,
                                            "KML-Header unexpected: ${head.replace("\n", " ")}"
                                        )
                                    )
                                }

                                logViewModelLogger.Log(
                                    LogEntry(
                                        LoggingEventType.Information,
                                        "KML: $url loaded (${bytes.size} Bytes)"
                                    )
                                )

                                val input = java.io.ByteArrayInputStream(bytes)

                                val kmlLayer = KmlLayer(googleMap, input, context)

                                requireActivity.runOnUiThread {
                                    try {
                                        kmlLayer.addLayerToMap()
                                    } catch (uiEx: Exception) {
                                        logViewModelLogger.Log(
                                            LogEntry(
                                                LoggingEventType.Error,
                                                "addLayerToMap failed: ${uiEx.message}",
                                                uiEx
                                            )
                                        )
                                    }
                                }
                            } catch (ex: org.xmlpull.v1.XmlPullParserException) {
                                logViewModelLogger.Log(
                                    LogEntry(
                                        LoggingEventType.Error,
                                        "KML-Parsing failed (possible not completely loaded): ${ex.message}",
                                        ex
                                    )
                                )
                            } catch (ex: Exception) {
                                logViewModelLogger.Log(
                                    LogEntry(
                                        LoggingEventType.Error,
                                        "Error by loading or parsing: ${ex.message}",
                                        ex
                                    )
                                )
                            } finally {
                                response.body()?.close()
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
    }
}