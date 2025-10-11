package com.milosev.kanaloa.logger

import android.annotation.SuppressLint
import android.app.Activity
import android.view.View
import android.widget.TextView
import com.milosev.kanaloa.R
import java.text.SimpleDateFormat
import java.util.Date

class ActivityLogger(private val activity: Activity): ILogger {
    @SuppressLint("SimpleDateFormat")
    override fun Log(entry: LogEntry?) {
        TODO("Not yet implemented")
        /*
        val log: TextView =
            activity.findViewById<View>(R.id.editTextTextMultiLineLog) as TextView
        val newLine = System.lineSeparator()

        val sdf = SimpleDateFormat("dd/M/yyyy hh:mm:ss")
        val currentDate = sdf.format(Date())

        log.append("${newLine}${newLine}${currentDate}: ${entry?.Message}")
         */
    }
}