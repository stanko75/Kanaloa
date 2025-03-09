package com.milosev.kanaloa.foregroundtickservice

import android.content.Context
import com.milosev.kanaloa.R
import java.util.Properties

class Config(val context: Context) {
    private val properties = Properties()

    init {
        val inputStream = context.resources.openRawResource(R.raw.config)
        properties.load(inputStream)
    }

    val webHost: String
        get() = properties.getProperty("web.host")
}