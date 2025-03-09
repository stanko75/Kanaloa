package com.milosev.kanaloa.location

import com.google.android.gms.location.LocationResult

interface ILocationResult {
    fun onLocationResult(locationResult: LocationResult)
}