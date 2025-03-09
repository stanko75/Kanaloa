package com.milosev.kanaloa.location.gsonhandling

import com.milosev.kanaloa.location.LocationModel

interface ICreateGsonLocationModel {
    fun execute(lat: String, lng: String): String?
    fun <T : LocationModel> serializeToJSON(model: T): String
}