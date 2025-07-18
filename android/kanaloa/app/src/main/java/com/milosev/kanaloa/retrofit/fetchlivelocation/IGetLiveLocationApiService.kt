package com.milosev.kanaloa.retrofit.fetchlivelocation

import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.GET
import retrofit2.http.Url

interface IGetLiveLocationApiService {
    @GET
    fun getLiveLocation(@Url url: String): Call<ResponseBody>
}