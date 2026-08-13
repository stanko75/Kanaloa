package com.milosev.kanaloa

import android.content.Context
import java.util.Properties

class Config(val context: Context?) {
    private val properties = Properties()

    init {
        try {
            val inputStream = context?.resources?.openRawResource(R.raw.config)
            if (inputStream != null) {
                properties.load(inputStream)
            }
        } catch (e: Exception) {
            e.printStackTrace()
        }
    }

    val webHost: String
        get() = properties.getProperty("web.host") ?: ""
}

object SharedPreferencesGlobal {
    const val Settings = "settings"
    const val FtpSettings = "ftpSettings"
    const val Live = "live"
    const val JoomlaSettings = "joomlaSettings"
    const val PhpUploadSettings = "joomlaSettings"

}