package com.milosev.kanaloa.logger

import android.annotation.SuppressLint
import com.milosev.kanaloa.ui.log.LogViewModel
import java.text.SimpleDateFormat
import java.util.Date

class LogViewModelLogger(private val viewModel: LogViewModel): ILogger {
    @SuppressLint("SimpleDateFormat")
    override fun Log(entry: LogEntry?) {

        val newLine = System.lineSeparator()

        val sdf = SimpleDateFormat("dd/M/yyyy hh:mm:ss")
        val currentDate = sdf.format(Date())

        viewModel.appendLog("${newLine}${newLine}${currentDate}: ${entry?.Message}");
    }

    fun setTicks(numOfTicks: Int) {
        viewModel.setTicks(numOfTicks)
    }
}