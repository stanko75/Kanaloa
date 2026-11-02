package com.milosev.kanaloa.retrofit.loadgeojsonfromurl

import okhttp3.ResponseBody
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Url

interface IGetGeoJson {
    @GET
    suspend fun getGeoJson(@Url url: String): Response<ResponseBody>
}