package com.milosev.kanaloa.foregroundtickservice

import android.content.Context
import android.content.Intent

class SendBroadcastTickReceiver: ISendBroadcastTickReceiver {
    override fun execute(context: Context, action: String, message: String) {
        val serviceIntent = Intent(context, BroadcastTickReceiver::class.java).setAction(action)
        serviceIntent.putExtra(action, message)
        context.sendBroadcast(serviceIntent)
    }

    override fun execute(context: Context, action: String, value: Int) {
        val serviceIntent = Intent(context, BroadcastTickReceiver::class.java).setAction(action)
        serviceIntent.putExtra(action, value)
        context.sendBroadcast(serviceIntent)
    }
}