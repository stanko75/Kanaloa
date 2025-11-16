package com.milosev.kanaloa.retrofit.loadkmlfromurl

import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Response
import retrofit2.http.GET
import retrofit2.http.Url

interface IGetKml {
    @GET
    suspend fun getKml(@Url url: String): Response<ResponseBody>
}