package com.milosev.kanaloa.foregroundtickservice

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent
import android.os.Build
import androidx.annotation.RequiresApi

class BroadcastTickReceiver : BroadcastReceiver() {

    private val receiveListener: IReceiveListener = ReceiveListener()

    @RequiresApi(Build.VERSION_CODES.O)
    override fun onReceive(context: Context, intent: Intent) {
        when (intent.action) {
            IntentAction.START_FOREGROUND_TICK_SERVICE -> {
                receiveListener.startForegroundTickService(context)
            }

            IntentAction.RESTART_FOREGROUND_TICK_SERVICE -> {
                receiveListener.restartForegroundTickService(context)
            }

            IntentAction.STOP_FOREGROUND_TICK_SERVICE -> {
                receiveListener.stopForegroundTickService(context)
            }

            IntentAction.NUM_OF_TICKS -> {
                receiveListener.numOfTicks(context, intent)
            }

            IntentAction.KML_FILE_NAME -> {
                receiveListener.numOfTicks(context, intent)
                receiveListener.kmlFileName(context, intent)
            }

            IntentAction.RETROFIT_ON_RESPONSE -> {
                receiveListener.retrofitOnResponse(context, intent)
            }
        }
    }
}