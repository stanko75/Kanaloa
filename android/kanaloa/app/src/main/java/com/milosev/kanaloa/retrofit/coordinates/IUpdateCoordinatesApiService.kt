package com.milosev.kanaloa.retrofit.coordinates

import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.Headers
import retrofit2.http.POST

interface IUpdateCoordinatesApiService {
    @Headers("Content-Type: text/json")
    //@POST("/api/UpdateCoordinates")
    @POST("/api/UpdateCoordinates/PostFileFolder")
    fun postMethod(@Body value: String): Call<String>
}