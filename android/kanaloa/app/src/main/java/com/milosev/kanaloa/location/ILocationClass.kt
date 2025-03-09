package com.milosev.kanaloa.location

import android.content.Context

interface ILocationClass {
    fun requestLocationUpdates(context: Context)
}