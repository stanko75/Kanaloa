package com.milosev.kanaloa.foregroundtickservice

import android.content.BroadcastReceiver
import android.content.Context
import android.content.Intent

class ForegroundServiceBroadcastReceiver(private val foregroundServiceBroadcastReceiverOnReceive: IForegroundServiceBroadcastReceiverOnReceive): BroadcastReceiver(), IForegroundServiceBroadcastReceiver {

    override fun onReceive(context: Context, intent: Intent) {
        foregroundServiceBroadcastReceiverOnReceive.onReceive(context, intent)
    }
}