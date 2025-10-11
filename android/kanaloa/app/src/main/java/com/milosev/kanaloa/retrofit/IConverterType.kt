package com.milosev.kanaloa.retrofit

import retrofit2.Converter

interface IConverterType {
    fun getFactory(): Converter.Factory
}