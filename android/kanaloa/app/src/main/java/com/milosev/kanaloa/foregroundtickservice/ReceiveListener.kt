package com.milosev.kanaloa.foregroundtickservice

import android.content.Context
import android.content.Intent
import android.os.Build
import androidx.annotation.RequiresApi
import androidx.localbroadcastmanager.content.LocalBroadcastManager
import com.milosev.kanaloa.MainActivity

class ReceiveListener: IReceiveListener {
    override fun startForegroundTickService(context: Context) {
        val startBroadcastTickReceiverIntent =
            Intent(context, MainActivity::class.java).setAction(IntentAction.MAIN_ACTIVITY_RECEIVER)
        LocalBroadcastManager.getInstance(context)
            .sendBroadcast(startBroadcastTickReceiverIntent)
    }

    @RequiresApi(Build.VERSION_CODES.O)
    override fun restartForegroundTickService(context: Context) {
        val restartBroadcastTickReceiverIntent =
            Intent(context, ForegroundTickService::class.java)
        context.startForegroundService(restartBroadcastTickReceiverIntent)
    }

    @RequiresApi(Build.VERSION_CODES.O)
    override fun stopForegroundTickService(context: Context) {
        val stopForegroundTickServiceIntent = Intent(
            context,
            ForegroundTickService::class.java
        ).setAction(IntentAction.STOP_FOREGROUND_TICK_SERVICE)
        context.startForegroundService(stopForegroundTickServiceIntent)
    }

    override fun numOfTicks(context: Context, intent: Intent) {
        val mainActivityIntent = Intent(context, MainActivity::class.java).setAction(
            IntentAction.NUM_OF_TICKS
        )
        val numOfTicks = intent.getIntExtra (IntentExtras.NUM_OF_TICKS, 30)
        mainActivityIntent.putExtra(IntentExtras.NUM_OF_TICKS, numOfTicks)
        LocalBroadcastManager.getInstance(context).sendBroadcast(mainActivityIntent)
    }

    override fun retrofitOnResponse(context: Context, intent: Intent) {
        val mainActivityIntent = Intent(context, MainActivity::class.java).setAction(
            IntentAction.RETROFIT_ON_RESPONSE
        )
        val retrofitOnResponse = intent.getStringExtra (IntentExtras.RETROFIT_ON_RESPONSE)
        mainActivityIntent.putExtra (IntentExtras.RETROFIT_ON_RESPONSE, retrofitOnResponse)
        LocalBroadcastManager.getInstance(context).sendBroadcast(mainActivityIntent)
    }

    override fun kmlFileName(context: Context, intent: Intent) {
        val mainActivityIntent = Intent(context, MainActivity::class.java).setAction(
            IntentAction.KML_FILE_NAME
        )
        val kmlFileName = intent.getStringExtra(IntentExtras.KML_FILE_NAME)
        mainActivityIntent.putExtra(IntentExtras.KML_FILE_NAME, kmlFileName)
        LocalBroadcastManager.getInstance(context).sendBroadcast(mainActivityIntent)
    }
}