package com.milosev.kanaloa.retrofit

import retrofit2.Retrofit

interface ICreateRetrofitBuilder {
    fun createRetrofitBuilder(baseUrl: String, converterType: IConverterType = ScalarsConverter()): Retrofit
}