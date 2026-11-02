package com.milosev.kanaloa.retrofit.loadgeojsonfromurl

import android.content.Context
import com.google.android.gms.maps.GoogleMap

interface ILoadGeoJson {
    suspend fun loadGeoJson(
        url: String?,
        googleMap: GoogleMap,
        context: Context?
    )
}