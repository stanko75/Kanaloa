package com.milosev.kanaloa.retrofit.loadkmlfromurl

import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.GET
import retrofit2.http.Url

interface IGetKml {
    @GET
    fun getKml(@Url url: String): Call<ResponseBody>
}