package com.milosev.kanaloa.retrofit.uploadtoblog

import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class UploadToBlog(
    private var uploadToBlogApiService: IUploadToBlogApiService,
    var uploadToBlogCallbacks: IUploadToBlogCallbacks
) {
        private lateinit var webApiRequest: Call<String>

        fun uploadToBlogHttpPost(value: String) {
            webApiRequest = uploadToBlogApiService.postMethod(value)

            webApiRequest.enqueue(object : Callback<String> {
                override fun onResponse(call: Call<String>, response: Response<String>) {
                    uploadToBlogCallbacks.onResponse(response)
                }

                override fun onFailure(call: Call<String>, t: Throwable) {
                    uploadToBlogCallbacks.onFailure(t)
                }
            })
        }
}