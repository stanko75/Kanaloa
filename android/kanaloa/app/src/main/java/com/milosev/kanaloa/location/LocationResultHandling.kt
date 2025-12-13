package com.milosev.kanaloa.location

import android.content.Context
import com.milosev.kanaloa.filehandling.IWriteFileOnInternalStorage
import com.milosev.kanaloa.location.gsonhandling.ICreateGsonLocationModel
import com.milosev.kanaloa.retrofit.coordinates.IUpdateCoordinatesOnWeb
import java.io.File
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

class LocationResultHandling(
    private val createGsonLocationModel: ICreateGsonLocationModel
    , private val writeFileOnInternalStorage: IWriteFileOnInternalStorage
    , private val updateCoordinatesOnWeb: IUpdateCoordinatesOnWeb
    , private val fileFolderLocationModel: FileFolderLocationModel
): ILocationResultHandling {
    override fun execute(context: Context, lat: String, lng: String, alt: String) {
        val sdf = SimpleDateFormat("ddMyyyyhhmmss", Locale.GERMANY)
        val currentDate = sdf.format(Date())
        val path = File(context.getExternalFilesDir(null), "locations")
        val fileName = "test$currentDate"

        fileFolderLocationModel.Latitude = lat
        fileFolderLocationModel.Longitude = lng
        fileFolderLocationModel.Altitude = alt

        val json = createGsonLocationModel.serializeToJSON(fileFolderLocationModel)
        writeFileOnInternalStorage.execute(path, fileName, json)
        updateCoordinatesOnWeb.updateCoordinatesHttpPost(json, context)
    }
}