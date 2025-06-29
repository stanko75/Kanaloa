package com.milosev.kanaloa.retrofit.uploadimages

import retrofit2.Converter
import retrofit2.converter.gson.GsonConverterFactory

class GsonConverter: com.milosev.kanaloa.retrofit.IConverterType {
    override fun getFactory(): Converter.Factory = GsonConverterFactory.create()
}