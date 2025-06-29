package com.milosev.kanaloa.retrofit.uploadimages

import retrofit2.Call
import retrofit2.Response

interface IUploadImagesCallbacks {
    fun onResponse(call: Call<UploadImagesResponse>, response: Response<UploadImagesResponse>)
    fun onFailure(call: Call<UploadImagesResponse>, t: Throwable)
}