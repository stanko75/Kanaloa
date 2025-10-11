package com.milosev.kanaloa.foregroundtickservice

import android.content.Context

interface ISendBroadcastTickReceiver {
    fun execute(context: Context, action: String, message: String)
    fun execute(context: Context, action: String, value: Int)
}