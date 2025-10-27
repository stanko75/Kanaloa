package com.milosev.kanaloa.ui.home

import android.app.Activity
import android.content.ComponentName
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Build
import androidx.annotation.RequiresApi
import com.milosev.kanaloa.foregroundtickservice.BroadcastTickReceiver
import com.milosev.kanaloa.foregroundtickservice.ForegroundTickService
import com.milosev.kanaloa.foregroundtickservice.IntentAction

class StopForegroundService {
    @RequiresApi(Build.VERSION_CODES.O)
    fun stopForegroundService(
        context: Context,
        activity: Activity
    ) {
        val component = ComponentName(context, BroadcastTickReceiver::class.java)
        context.packageManager.setComponentEnabledSetting(
            component,
            PackageManager.COMPONENT_ENABLED_STATE_DISABLED,
            PackageManager.DONT_KILL_APP
        )

        val intentStopForegroundTickService = Intent(context, ForegroundTickService::class.java)
        intentStopForegroundTickService.action = IntentAction.STOP_FOREGROUND_TICK_SERVICE
        activity.startForegroundService(intentStopForegroundTickService)
    }
}