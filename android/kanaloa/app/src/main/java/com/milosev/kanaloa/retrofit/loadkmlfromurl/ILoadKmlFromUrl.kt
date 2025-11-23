package com.milosev.kanaloa.retrofit.loadkmlfromurl

import android.content.Context
import com.google.android.gms.maps.GoogleMap

interface ILoadKmlFromUrl {
    suspend fun loadKml(
        url: String?,
        googleMap: GoogleMap,
        context: Context?
    )
}