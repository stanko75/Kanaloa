package com.milosev.kanaloa.foregroundtickservice

import android.content.Context
import android.content.Intent
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.logger.LoggerExtensions

class ForegroundServiceBroadcastReceiverOnReceive(private val logViewModelLogger: LogViewModelLogger): IForegroundServiceBroadcastReceiverOnReceive {

    override fun onReceive(context: Context, intent: Intent) {
        when (intent.action) {
            IntentAction.NUM_OF_TICKS -> {
                val numOfTicks = intent.getIntExtra(IntentExtras.NUM_OF_TICKS, 30)
                onNumOfTicksReceived(numOfTicks)
            }
            IntentAction.RETROFIT_ON_RESPONSE -> {
                val retrofitMessage = intent.getStringExtra(IntentExtras.RETROFIT_ON_RESPONSE)
                onRetrofitResponseReceived(retrofitMessage, context)
            }
            else -> {
                onUnknownActionReceived(context)
            }
        }
    }

    override fun onNumOfTicksReceived(numOfTicks: Int) {
        logViewModelLogger.setTicks(numOfTicks)
    }

    override fun onRetrofitResponseReceived(retrofitMessage: String?, context: Context) {
        if (retrofitMessage != null) {
            LoggerExtensions().log(logViewModelLogger, retrofitMessage)
        }
    }

    override fun onUnknownActionReceived(context: Context) {
        LoggerExtensions().log(logViewModelLogger, "else")
    }
}