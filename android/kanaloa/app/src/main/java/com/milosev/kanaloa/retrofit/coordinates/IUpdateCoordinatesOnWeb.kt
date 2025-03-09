package com.milosev.kanaloa.retrofit.coordinates

import android.content.Context

interface IUpdateCoordinatesOnWeb {
    fun updateCoordinatesHttpPost(value: String, context: Context)
}