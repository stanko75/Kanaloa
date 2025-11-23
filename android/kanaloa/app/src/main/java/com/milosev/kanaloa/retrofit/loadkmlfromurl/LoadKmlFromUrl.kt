package com.milosev.kanaloa.retrofit.loadkmlfromurl

import android.content.Context
import android.util.Log
import com.google.android.gms.maps.GoogleMap
import com.google.maps.android.data.kml.KmlLayer
import com.milosev.kanaloa.SharedPreferencesGlobal
import com.milosev.kanaloa.logger.ILogger
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LoggingEventType
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive
import kotlinx.coroutines.withContext
import okhttp3.ResponseBody
import java.util.Locale

class LoadKmlFromUrl(private var getKml: IGetKml, var logViewModelLogger: ILogger) :
    ILoadKmlFromUrl {

    private var kmlLayer: KmlLayer? = null

    override suspend fun loadKmlFromUrl(
        url: String?,
        googleMap: GoogleMap,
        context: Context?
    ) {
        val strUrl = url.toString()
        loadKml(strUrl, googleMap, context, getKml)
    }

    suspend fun loadKml(strUrl: String, googleMap: GoogleMap, context: Context?, getKml: IGetKml) {
        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                "Request: $strUrl"
            )
        )

        try {
            loadKmlFromWeb(getKml, strUrl, googleMap, context)
        } catch (ex: Exception) {

            Log.e("KML ERROR", ex.message.toString())

            logViewModelLogger.Log(
                LogEntry(
                    LoggingEventType.Error,
                    "Error by loading or parsing: ${ex.message}",
                    ex
                )
            )
        }
    }

    suspend fun loadKmlFromWeb(
        getKml: IGetKml,
        strUrl: String,
        googleMap: GoogleMap,
        context: Context?
    ) {
        val webApiRequest = getKml.getKml(strUrl)

        if (webApiRequest.isSuccessful) {

            val body = webApiRequest.body()
            val (isValid, bytes) = checkIfResponseBodyIsNullOrEmpty(body)

            if (isValid) {
                try {

                    actualLoadKml(
                        strUrl,
                        googleMap,
                        context,
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

    suspend fun actualLoadKml(
        strUrl: String,
        googleMap: GoogleMap,
        context: Context?,
        bytes: ByteArray?
    ) {
        val input = java.io.ByteArrayInputStream(bytes)

        val parsedLayer = withContext(Dispatchers.Default) {
            KmlLayer(googleMap, input, context)
        }

        withContext(Dispatchers.Main) {

            val formattedSize = String.format(
                Locale.ROOT,
                "%.2f KB",
                (bytes?.size?.toDouble() ?: 0.0) / 1024.0
            )
            logViewModelLogger.Log(
                LogEntry(
                    LoggingEventType.Information,
                    "KML: $strUrl loaded (${formattedSize})"
                )
            )

            try {
                kmlLayer?.removeLayerFromMap()
                kmlLayer = parsedLayer
                kmlLayer?.addLayerToMap()
            } catch (uiEx: Exception) {
                Log.e("KML ERROR", uiEx.message.toString())
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