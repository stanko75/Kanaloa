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

class LoadKmlFromUrl(var logViewModelLogger: ILogger) : ILoadKmlFromUrl {
    override suspend fun loadKmlFromUrl(
        url: String,
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity
    ) {
        try {

            context?.let { Config(it).webHost }?.let {
                loadKml(
                    url,
                    googleMap,
                    context,
                    requireActivity,
                    it
                )
            }
        } catch (e: Exception) {
            logViewModelLogger.Log(LogEntry(LoggingEventType.Error, e.message, e))
        }
    }

    suspend fun loadKml(
        strUrl: String,
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity,
        baseUrl: String
    ) {
        val kmlClient = CreateRetrofitBuilder().createRetrofitBuilder(baseUrl)
            .create(IGetKml::class.java)

        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                "Request: $strUrl"
            )
        )

        val webApiRequest = kmlClient.getKml(strUrl)

        if (webApiRequest.isSuccessful) {

            val body = webApiRequest.body()
            val (isValid, bytes) = checkIfResponseBodyIsNullOrEmpty(body)

            if (isValid) {
                try {

                    actualLoadKml(
                        strUrl,
                        googleMap,
                        context,
                        requireActivity,
                        bytes
                    )

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
                    webApiRequest.body()?.close()
                }
            }
        } else {
            val sendResponse = "${webApiRequest.code()}: "
            if (webApiRequest.errorBody()?.charStream() != null
                && webApiRequest.errorBody()?.charStream()?.readText() != null
                && webApiRequest.errorBody()!!.charStream().readText().isNotBlank()
            ) {
                logViewModelLogger.Log(
                    LogEntry(
                        LoggingEventType.Error,
                        "${sendResponse}${
                            webApiRequest.errorBody()!!.charStream().readText()
                        }"
                    )
                )
            } else {
                logViewModelLogger.Log(
                    LogEntry(
                        LoggingEventType.Error,
                        "${sendResponse}${webApiRequest.message()}"
                    )
                )
            }
        }
    }

    fun checkIfResponseBodyIsNullOrEmpty(body: ResponseBody?): Pair<Boolean, ByteArray?> {
        if (body == null) {
            logViewModelLogger.Log(
                LogEntry(
                    LoggingEventType.Error,
                    "KML is empty (body==null)!"
                )
            )
            return Pair(false, null)
        } else {
            val bytes = body.bytes()
            if (bytes.isEmpty()) {
                logViewModelLogger.Log(
                    LogEntry(
                        LoggingEventType.Error,
                        "KML is empty (0 Bytes)!"
                    )
                )

                return Pair(false, null)
            } else {
                val head =
                    bytes.copyOfRange(0, minOf(bytes.size, 200)).decodeToString()
                if (!head.contains("<kml", ignoreCase = true)) {
                    logViewModelLogger.Log(
                        LogEntry(
                            LoggingEventType.Warning,
                            "KML-Header unexpected: ${head.replace("\n", " ")}"
                        )
                    )
                    return Pair(false, null)
                }

                return Pair(true, bytes)
            }
        }
    }

    fun actualLoadKml(
        strUrl: String,
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity,
        bytes: ByteArray?
    ) {
        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                "KML: $strUrl loaded (${bytes?.size} Bytes)"
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
    }
}