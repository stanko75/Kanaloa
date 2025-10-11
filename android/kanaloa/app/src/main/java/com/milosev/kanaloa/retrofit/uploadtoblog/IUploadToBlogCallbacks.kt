package com.milosev.kanaloa.retrofit.uploadtoblog

import retrofit2.Response

interface IUploadToBlogCallbacks {
    fun onResponse(response: Response<String>)
    fun onFailure(t: Throwable)
}