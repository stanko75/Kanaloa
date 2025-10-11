package com.milosev.kanaloa.retrofit.coordinates

import android.content.Context
import retrofit2.Response

interface IUpdateCoordinatesOnWebCallbacks {
    fun onResponse(response: Response<String>, context: Context)
    fun onFailure(t: Throwable, context: Context)
}