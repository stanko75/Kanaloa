package com.milosev.kanaloa.retrofit.uploadimages

import com.google.gson.JsonObject
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.Headers
import retrofit2.http.POST

interface IUploadImagesApiService {
    @Headers("Content-Type: text/json")
    @POST("/api/UploadImages/UploadImage")
    fun uploadImage(@Body image: JsonObject): Call<UploadImagesResponse>
}