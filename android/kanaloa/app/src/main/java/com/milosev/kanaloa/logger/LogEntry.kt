package com.milosev.kanaloa.logger

data class LogEntry (var Severity: LoggingEventType, var Message: String?, var Exception: Exception? = null)
