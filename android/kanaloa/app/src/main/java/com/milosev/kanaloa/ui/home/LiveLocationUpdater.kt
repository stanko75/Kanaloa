package com.milosev.kanaloa.ui.home

import android.content.Context
import androidx.fragment.app.FragmentActivity
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.model.Marker
import com.milosev.kanaloa.Config
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.logger.LoggingEventType
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Job
import kotlinx.coroutines.delay
import kotlinx.coroutines.isActive
import kotlinx.coroutines.launch
import com.milosev.kanaloa.retrofit.fetchlivelocation.IFetchLiveLocation
import com.milosev.kanaloa.retrofit.loadkmlfromurl.ILoadKmlFromUrl
import kotlinx.coroutines.Dispatchers


class LiveLocationUpdater(
    private val fetchLiveLocation: IFetchLiveLocation
    , private val loadKmlFromUrl: ILoadKmlFromUrl
) {
    var marker: Marker? = null

    fun start(
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity,
        logViewModelLogger: LogViewModelLogger,
        kmlUrl: String
    ): Job {

        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                "...Start..."
            )
        )

        val updateJob = Job()
        val url = context?.let { Config(it).webHost };
        CoroutineScope(Dispatchers.Main).launch(updateJob) {
                updateLocationOnUi(kmlUrl, googleMap, context, requireActivity, logViewModelLogger, url)
        }

        return updateJob
    }

    private suspend fun CoroutineScope.updateLocationOnUi(
        kmlUrl: String,
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity,
        logViewModelLogger: LogViewModelLogger,
        url: String?
    ) {
        while (isActive) {
            try {
                loadKmlFromUrl.loadKmlFromUrl(kmlUrl, googleMap, context, requireActivity)

                if (context != null) {
                    fetchLiveLocation.fetchLiveLocation(context, url, googleMap)
                } else {
                    logViewModelLogger.Log(LogEntry(LoggingEventType.Error, "Context is null"))
                }
            } catch (e: Exception) {
                logViewModelLogger.Log(LogEntry(LoggingEventType.Error, e.message, e))
            }

            val sharedPreferences = context?.getSharedPreferences("settings", Context.MODE_PRIVATE)
            val intervalString = sharedPreferences?.getString("requestUpdates", "30") ?: "30"
            var updateInterval = intervalString.toLongOrNull()?.times(1000) ?: 30_000L
            if (updateInterval < 10_000) {
                updateInterval = 10_000L
            }
            delay(updateInterval)

        }
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
}
