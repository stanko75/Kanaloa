package com.milosev.kanaloa.retrofit.uploadimages

import retrofit2.Converter

interface IConverterType {
    fun getFactory(): Converter.Factory
}