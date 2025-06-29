package com.milosev.kanaloa.retrofit.uploadimages

import com.milosev.kanaloa.logger.ILogger
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LoggingEventType
import retrofit2.Call
import retrofit2.Response

class UploadImagesCallbacks(var logViewModelLogger: ILogger): IUploadImagesCallbacks {
    override fun onResponse(call: Call<UploadImagesResponse>, response: Response<UploadImagesResponse>) {
        if (response.isSuccessful) {
            logViewModelLogger.Log(
                LogEntry(
                    LoggingEventType.Information, "Response message: ${response.message()}, body: ${response.body()}"
                )
            )
        }
        else {
            val sendResponse = "${response.code()}: "

            val errorMessage = response.errorBody()?.string()

            if (errorMessage != null) {
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

    override fun onFailure(call: Call<UploadImagesResponse>, t: Throwable) {
        logViewModelLogger.Log(
            LogEntry(
                LoggingEventType.Error,
                t.message.toString(),
            )
        )
    }
}