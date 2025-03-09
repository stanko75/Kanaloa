package com.milosev.kanaloa.location

import android.content.Context

interface ILocationResultHandling {
    fun execute(context: Context, lat: String, lng: String)
}