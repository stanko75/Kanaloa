package com.milosev.kanaloa.ui.home

import android.app.Activity
import android.content.ComponentName
import android.content.Context
import android.content.Intent
import android.content.pm.PackageManager
import android.os.Build
import android.view.View
import android.view.inputmethod.InputMethodManager
import androidx.annotation.RequiresApi
import com.milosev.kanaloa.foregroundtickservice.BroadcastTickReceiver
import com.milosev.kanaloa.foregroundtickservice.ForegroundTickService
import com.milosev.kanaloa.foregroundtickservice.IntentAction
import com.milosev.kanaloa.foregroundtickservice.IntentExtras

class StartForegroundService {
    @RequiresApi(Build.VERSION_CODES.O)
    fun startForegroundService(context: Context, activity: Activity, view: View) {

        val intentStartForegroundTickService =
            Intent(context, ForegroundTickService::class.java)
        intentStartForegroundTickService.action = IntentAction.START_FOREGROUND_TICK_SERVICE
        intentStartForegroundTickService.putExtra(
            IntentExtras.NUM_OF_SECONDS_FOR_TICK,
            0
        )

        val inputMethodManager =
            context.getSystemService(Activity.INPUT_METHOD_SERVICE) as InputMethodManager
        inputMethodManager.hideSoftInputFromWindow(view.windowToken, 0)

        val component = ComponentName(context, BroadcastTickReceiver::class.java)
        activity.packageManager.setComponentEnabledSetting(
            component,
            PackageManager.COMPONENT_ENABLED_STATE_ENABLED,
            PackageManager.DONT_KILL_APP
        )

        val sharedPreferences = context.getSharedPreferences("settings", Context.MODE_PRIVATE)
        val fileName = sharedPreferences.getString("kmlFileName", "default")
        val folderName = sharedPreferences.getString("folderName", "default")

        intentStartForegroundTickService.putExtra(
            IntentExtras.KML_FILE_NAME,
            fileName
        )

        intentStartForegroundTickService.putExtra(
            IntentExtras.FOLDER_NAME,
            folderName
        )

        activity.startForegroundService(intentStartForegroundTickService)
    }
}