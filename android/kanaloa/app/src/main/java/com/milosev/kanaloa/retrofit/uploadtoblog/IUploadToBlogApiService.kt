package com.milosev.kanaloa.retrofit.uploadtoblog

import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.Headers
import retrofit2.http.POST

interface IUploadToBlogApiService {
    @Headers("Content-Type: text/json")
    @POST("/api/UploadToBlog/UploadToBlog")
    fun postMethod(@Body value: String): Call<String>
}