package com.milosev.kanaloa.retrofit.uploadtoblog

import android.app.Activity
import android.content.Intent
import android.net.Uri
import com.milosev.kanaloa.logger.ILogger
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LoggingEventType
import retrofit2.Response

class UploadToBlogCallbacks(var logViewModelLogger: ILogger, var activity: Activity, var folderName: String?): IUploadToBlogCallbacks {

    override fun onResponse(response: Response<String>) {
        if (response.isSuccessful) {
            val url =
                "http://www.milosev.com/gallery/allWithPics/travelBuddies/$folderName/www/index.html"
            val intent = Intent(Intent.ACTION_VIEW, Uri.parse(url))
            activity.startActivity(intent)

            logViewModelLogger.Log(
                LogEntry(
                    LoggingEventType.Information,
                    "Response message: ${response.message()}, body: ${response.body()}"
                )
            )
        } else {

            val sendResponse = "${response.code()}: "

            val errorMessage = response.errorBody()?.string()

            if (errorMessage != null && errorMessage != "") {
                logViewModelLogger.Log(
                    LogEntry(
                        LoggingEventType.Error,
                        "${sendResponse}${errorMessage}"
                    )
                )
            } else {
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
    }

    override fun onFailure(t: Throwable) {
        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Information,
                t.message.toString()
            )
        )
    }
}