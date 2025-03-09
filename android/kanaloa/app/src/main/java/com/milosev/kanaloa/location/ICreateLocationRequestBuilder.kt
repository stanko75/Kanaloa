package com.milosev.kanaloa.location

import com.google.android.gms.location.LocationRequest

interface ICreateLocationRequestBuilder {
    fun buildLocationRequest(interval: Long?): LocationRequest
}