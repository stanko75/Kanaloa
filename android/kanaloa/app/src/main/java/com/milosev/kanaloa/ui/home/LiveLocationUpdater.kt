package com.milosev.kanaloa.ui.home

import android.content.Context
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.model.Marker
import com.milosev.kanaloa.Config
import com.milosev.kanaloa.SharedPreferencesGlobal
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.logger.LoggingEventType
import com.milosev.kanaloa.retrofit.fetchlivelocation.IFetchLiveLocation
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch


class LiveLocationUpdater(
    private val fetchLiveLocation: IFetchLiveLocation
) {
    var marker: Marker? = null

    fun start(
        googleMap: GoogleMap,
        context: Context?,
        logViewModelLogger: LogViewModelLogger
    ): Job {

        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                "...Start..."
            )
        )

        val updateJob = Job()
        val url = context?.let { Config(it).webHost }
        CoroutineScope(Dispatchers.Main).launch(updateJob) {
            updateLocationOnUi(googleMap, context, logViewModelLogger, url)
        }

        return updateJob
    }

    fun stop(logViewModelLogger: LogViewModelLogger, updateJob: Job?) {

        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                "...Stop..."
            )
        )

        if (updateJob == null) {
            logViewModelLogger.Log(
                LogEntry(
                    LoggingEventType.Error,
                    "Update job is null!"
                )
            )
        }

        updateJob?.cancel()
    }

    private suspend fun CoroutineScope.updateLocationOnUi(
        googleMap: GoogleMap,
        context: Context?,
        logViewModelLogger: LogViewModelLogger,
        url: String?
    ) {
        while (isActive) {
            try {
                if (context != null) {
                    fetchLiveLocation.fetchLiveLocation(context, url, googleMap)
                } else {
                    logViewModelLogger.Log(LogEntry(LoggingEventType.Error, "Context is null"))
                }
            } catch (e: Exception) {
                logViewModelLogger.Log(LogEntry(LoggingEventType.Error, e.message, e))
            }

            val sharedPreferences = context?.getSharedPreferences(
                SharedPreferencesGlobal.Settings,
                Context.MODE_PRIVATE
            )
            val intervalString = sharedPreferences?.getString("requestUpdates", "30") ?: "30"
            var updateInterval = intervalString.toLongOrNull()?.times(1000) ?: 30_000L
            if (updateInterval < 10_000) {
                updateInterval = 10_000L
            }
            delay(updateInterval)

        }
    }
}