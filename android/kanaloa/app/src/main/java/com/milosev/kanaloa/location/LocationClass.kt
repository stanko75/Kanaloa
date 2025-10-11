package com.milosev.kanaloa.location

import android.Manifest
import android.content.Context
import android.content.pm.PackageManager
import android.os.Looper
import androidx.core.app.ActivityCompat
import com.google.android.gms.location.FusedLocationProviderClient
import com.google.android.gms.location.LocationCallback
import com.google.android.gms.location.LocationRequest
import com.google.android.gms.location.LocationResult
import com.google.android.gms.location.LocationServices


class LocationClass
    (context: Context, private val locationResult: ILocationResult, private var locationRequest: LocationRequest): ILocationClass {

    private var fusedLocationClient: FusedLocationProviderClient
    private lateinit var locationCallback: LocationCallback

    init {
        fusedLocationClient = LocationServices.getFusedLocationProviderClient(context)
    }

    override fun requestLocationUpdates(context: Context) {
        if (ActivityCompat.checkSelfPermission(
                context,
                Manifest.permission.ACCESS_FINE_LOCATION
            ) != PackageManager.PERMISSION_GRANTED && ActivityCompat.checkSelfPermission(
                context,
                Manifest.permission.ACCESS_COARSE_LOCATION
            ) != PackageManager.PERMISSION_GRANTED
        ) {
            return
        }

        locationCallback = object : LocationCallback() {
            override fun onLocationResult(locationResultLocal: LocationResult) {
                locationResult.onLocationResult(locationResultLocal)
            }
        }

        fusedLocationClient = LocationServices.getFusedLocationProviderClient(context)

        fusedLocationClient.requestLocationUpdates(
            locationRequest, locationCallback, Looper.getMainLooper()
        )
    }

}