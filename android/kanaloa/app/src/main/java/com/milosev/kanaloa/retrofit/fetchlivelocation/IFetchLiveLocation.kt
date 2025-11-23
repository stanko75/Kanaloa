package com.milosev.kanaloa.retrofit.fetchlivelocation

import android.content.Context
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.model.LatLng

interface IFetchLiveLocation {
    fun fetchLiveLocation(url: String?, googleMap: GoogleMap)
}