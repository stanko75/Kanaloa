package com.milosev.kanaloa.ui.home

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.os.Build
import androidx.annotation.RequiresApi
import com.milosev.kanaloa.foregroundtickservice.ForegroundTickService

class StartForegroundService {
    @RequiresApi(Build.VERSION_CODES.O)
    fun startForegroundService(context: Context?, activity: Activity) {

        val intentStartForegroundTickService =
            Intent(context, ForegroundTickService::class.java)

        activity.startForegroundService(intentStartForegroundTickService)
    }
}