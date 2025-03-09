package com.milosev.kanaloa.retrofit

import retrofit2.Converter
import retrofit2.converter.scalars.ScalarsConverterFactory

class ScalarsConverter: IConverterType {
    override fun getFactory(): Converter.Factory = ScalarsConverterFactory.create()
}