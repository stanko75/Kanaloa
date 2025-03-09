package com.milosev.kanaloa.ui.home

import android.content.Context
import android.widget.Toast

class StartForegroundService {
    fun startForegroundService(context: Context?) {
        Toast.makeText(context, "Home Clicked", Toast.LENGTH_LONG).show()
    }
}