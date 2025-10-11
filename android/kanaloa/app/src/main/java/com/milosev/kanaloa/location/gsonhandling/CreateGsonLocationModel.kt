package com.milosev.kanaloa.location.gsonhandling

import com.google.gson.Gson
import com.google.gson.GsonBuilder
import com.milosev.kanaloa.location.LocationModel

class CreateGsonLocationModel: ICreateGsonLocationModel {
    override fun execute(lat: String, lng: String): String? {
        val builder = GsonBuilder()
        val gson: Gson = builder.create()

        val locationModel = LocationModel()
        locationModel.Latitude = lat
        locationModel.Longitude = lng

        return gson.toJson(locationModel)
    }

    override fun <T : LocationModel> serializeToJSON(model: T): String {
        val builder = GsonBuilder()
        val gson: Gson = builder.create()
        return gson.toJson(model)
    }
}