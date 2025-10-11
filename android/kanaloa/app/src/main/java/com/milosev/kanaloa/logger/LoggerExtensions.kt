package com.milosev.kanaloa.logger

import java.lang.Exception

class LoggerExtensions {
    fun log(logger: ILogger, message: String) {
        logger.Log(LogEntry(LoggingEventType.Information, message))
    }

    fun log(logger: ILogger, ex: Exception) {
        logger.Log(LogEntry(LoggingEventType.Error, ex.message, ex))
    }
}