package com.milosev.kanaloa.retrofit.loadgeojsonfromurl

import android.content.Context
import android.util.Log
import com.google.android.gms.maps.GoogleMap
import com.google.maps.android.data.parser.geojson.GeoJsonParser
import com.google.maps.android.data.renderer.UrlIconProvider
import com.google.maps.android.data.renderer.mapper.GeoJsonMapper
import com.google.maps.android.data.renderer.mapview.MapViewRenderer
import com.milosev.kanaloa.logger.ILogger
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LoggingEventType
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import okhttp3.ResponseBody
import java.util.Locale

class LoadGeoJsonFromUrl(private var getGeoJson: IGetGeoJson, var logViewModelLogger: ILogger) :
    ILoadGeoJson {

    private var renderer: MapViewRenderer? = null

    override suspend fun loadGeoJson(
        url: String?,
        googleMap: GoogleMap,
        context: Context?
    ) {
        val strUrl = url.toString()
        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                "Request: $strUrl"
            )
        )

        try {
            loadGeoJsonFromWeb(getGeoJson, strUrl, googleMap, context)
        } catch (ex: Exception) {

            Log.e("GeoJSON ERROR", ex.message.toString())

            logViewModelLogger.Log(
                LogEntry(
                    LoggingEventType.Error,
                    "Error by loading or parsing: ${ex.message}",
                    ex
                )
            )
        }
    }

    suspend fun loadGeoJsonFromWeb(
        getGeoJson: IGetGeoJson,
        strUrl: String,
        googleMap: GoogleMap,
        context: Context?
    ) {
        val webApiRequest = getGeoJson.getGeoJson(strUrl)

        if (webApiRequest.isSuccessful) {

            val body = webApiRequest.body()
            val (isValid, bytes) = checkIfResponseBodyIsNullOrEmpty(body)

            if (isValid) {
                try {

                    actualLoadGeoJson(
                        strUrl,
                        googleMap,
                        context,
                        bytes
                    )

                } catch (ex: Exception) {
                    logViewModelLogger.Log(
                        LogEntry(
                            LoggingEventType.Error,
                            "GeoJSON-Parsing failed (possible not completely loaded): ${ex.message}",
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
                        "${sendResponse}${webApiRequest.message()} url: ${webApiRequest.raw().request.url}"
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
                    "GeoJSON is empty (body==null)!"
                )
            )
            return Pair(false, null)
        } else {
            val bytes = body.bytes()
            if (bytes.isEmpty()) {
                logViewModelLogger.Log(
                    LogEntry(
                        LoggingEventType.Error,
                        "GeoJSON is empty (0 Bytes)!"
                    )
                )

                return Pair(false, null)
            } else {
                val head =
                    bytes.copyOfRange(0, minOf(bytes.size, 200)).decodeToString()
                if (!head.trim().startsWith("{")) {
                    logViewModelLogger.Log(
                        LogEntry(
                            LoggingEventType.Warning,
                            "GeoJSON-Header unexpected: ${head.replace("\n", " ")}"
                        )
                    )
                    return Pair(false, null)
                }

                return Pair(true, bytes)
            }
        }
    }

    suspend fun actualLoadGeoJson(
        strUrl: String,
        googleMap: GoogleMap,
        context: Context?,
        bytes: ByteArray?
    ) {
        if (context == null || bytes == null) return

        val input = java.io.ByteArrayInputStream(bytes)

        val parsedScene = withContext(Dispatchers.Default) {
            val geoJsonObject = GeoJsonParser().parse(input)
            geoJsonObject?.let { GeoJsonMapper.toScene(it) }
        }

        withContext(Dispatchers.Main) {

            val formattedSize = String.format(
                Locale.ROOT,
                "%.2f KB",
                bytes.size.toDouble() / 1024.0
            )
            logViewModelLogger.Log(
                LogEntry(
                    LoggingEventType.Information,
                    "GeoJSON: $strUrl loaded (${formattedSize})"
                )
            )

            try {
                renderer?.clear()
                renderer = MapViewRenderer(googleMap, UrlIconProvider())
                parsedScene?.let { renderer?.render(it) }
            } catch (uiEx: Exception) {
                Log.e("GeoJSON ERROR", uiEx.message.toString())
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