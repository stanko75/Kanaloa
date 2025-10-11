package com.milosev.kanaloa.retrofit.uploadimages

import android.content.Context
import android.net.Uri
import android.os.Build
import androidx.annotation.RequiresApi
import com.google.gson.JsonObject
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response
import java.util.Base64

class UploadImages(
    private var uploadImagesApiService: IUploadImagesApiService,
    var uploadImagesCallbacks: IUploadImagesCallbacks
) : IUploadImages {
    @RequiresApi(Build.VERSION_CODES.O)
    override fun uploadImage(imgUri: Uri, imageFileName: String, folderName: String, kmlFileName: String, context: Context): String? {

        val base64Image = convertImageToBase64(context, imgUri)
        val jsonValue = JsonObject().apply {
            addProperty("base64Image", base64Image)
            addProperty("imageFileName", imageFileName)
            addProperty("folderName", folderName)
            addProperty("kmlFileName", kmlFileName)
        }
        val webApiRequest = uploadImagesApiService.uploadImage(jsonValue)

        webApiRequest.enqueue(object : Callback<UploadImagesResponse> {
            override fun onResponse(
                call: Call<UploadImagesResponse>,
                response: Response<UploadImagesResponse>
            ) {
                uploadImagesCallbacks.onResponse(call, response)
            }

            override fun onFailure(call: Call<UploadImagesResponse>, t: Throwable) {
                uploadImagesCallbacks.onFailure(call, t)
            }
        })

        return null
    }

    @RequiresApi(Build.VERSION_CODES.O)
    fun convertImageToBase64(context: Context, imgUri: Uri): String {
        val inputStream = context.contentResolver.openInputStream(imgUri)
        val imageBytes = inputStream.use { input ->
            input?.readBytes()
        } ?: return "" // Handle null input stream or read failure

        return Base64.getEncoder().encodeToString(imageBytes)
    }
}