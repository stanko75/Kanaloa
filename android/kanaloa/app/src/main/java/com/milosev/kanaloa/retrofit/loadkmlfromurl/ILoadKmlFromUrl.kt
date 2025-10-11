package com.milosev.kanaloa.retrofit.loadkmlfromurl

import android.content.Context
import androidx.fragment.app.FragmentActivity
import com.google.android.gms.maps.GoogleMap

interface ILoadKmlFromUrl {
    fun loadKmlFromUrl(
        url: String,
        googleMap: GoogleMap,
        context: Context?,
        requireActivity: FragmentActivity
    )
}