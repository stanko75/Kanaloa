package com.milosev.kanaloa.ui.settings

import android.content.Context
import android.content.Intent
import android.os.Build
import android.os.Bundle
import android.provider.Settings
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.milosev.kanaloa.R
import com.milosev.kanaloa.databinding.ActivitySettingsBinding
import androidx.core.content.edit

class SettingsActivity : AppCompatActivity() {

    private lateinit var binding: ActivitySettingsBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_settings)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        binding = ActivitySettingsBinding.inflate(layoutInflater)
        setContentView(binding.root)
        val sharedPreferences =
            getSharedPreferences("settings", Context.MODE_PRIVATE)

        val host = sharedPreferences.getString("host", "ftp.host.com")
        val user = sharedPreferences.getString("user", "user")
        val pass = sharedPreferences.getString("pass", "pass")
        val requestUpdates = sharedPreferences.getString("requestUpdates", "30")

        binding.editTextHost.setText(host)
        binding.editTextUser.setText(user)
        binding.editTextPass.setText(pass)
        binding.txtRequestUpdates.setText(requestUpdates)

        binding.btnSaveSettings.setOnClickListener {
            sharedPreferences.edit() {
                putString("host", binding.editTextHost.text.toString())
                putString("user", binding.editTextUser.text.toString())
                putString("pass", binding.editTextPass.text.toString())
                putString("requestUpdates", binding.txtRequestUpdates.text.toString())
            }
            finish()
        }

        binding.btnOpenBatteryOptimization.setOnClickListener {
            val intent = Intent()
            intent.action = Settings.ACTION_IGNORE_BATTERY_OPTIMIZATION_SETTINGS
            startActivity(intent)
        }
    }
}