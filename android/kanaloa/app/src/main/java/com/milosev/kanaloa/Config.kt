package com.milosev.kanaloa

import android.content.Context
import java.util.Properties

class Config(val context: Context?) {
    private val properties = Properties()

    init {
        val inputStream = context?.resources?.openRawResource(R.raw.config)
        properties.load(inputStream)
    }

    val webHost: String
        get() = properties.getProperty("web.host")
}

object SharedPreferencesGlobal {
    const val Settings = "settings"
    const val FtpSettings = "ftpSettings"

}