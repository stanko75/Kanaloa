package com.milosev.kanaloa.retrofit.coordinates

import android.content.Context
import com.milosev.kanaloa.foregroundtickservice.ISendBroadcastTickReceiver
import com.milosev.kanaloa.foregroundtickservice.IntentAction
import retrofit2.Response

class UpdateCoordinatesOnWebCallbacks(private val broadcastTickReceiver: ISendBroadcastTickReceiver):
    IUpdateCoordinatesOnWebCallbacks {
    override fun onResponse(response: Response<String>, context: Context) {
        if (response.isSuccessful) {
            broadcastTickReceiver.execute(
                context,
                IntentAction.RETROFIT_ON_RESPONSE,
                response.message()
            )
        } else {
            val sendResponse = "${response.code()}: "
            if (response.errorBody()?.charStream() != null
                && response.errorBody()?.charStream()?.readText() != null
                && response.errorBody()!!.charStream().readText().isNotBlank()
            ) {
                broadcastTickReceiver.execute(
                    context,
                    IntentAction.RETROFIT_ON_RESPONSE,
                    "${sendResponse}${response.errorBody()!!.charStream().readText()}"
                )
            }
            else {
                broadcastTickReceiver.execute(
                    context,
                    IntentAction.RETROFIT_ON_RESPONSE,
                    "${sendResponse}${response.message()}"
                )
            }
        }
    }

    override fun onFailure(t: Throwable, context: Context) {
        broadcastTickReceiver.execute(
            context,
            IntentAction.RETROFIT_ON_RESPONSE,
            t.message.toString()
        )
    }
}