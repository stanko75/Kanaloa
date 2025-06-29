package com.milosev.kanaloa.retrofit.uploadimages

import android.content.Context
import android.net.Uri

interface IUploadImages {
    fun uploadImage(imgUri: Uri, imageFileName: String, folderName: String, kmlFileName: String, context: Context): String?
}