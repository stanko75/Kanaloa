package com.milosev.kanaloa.ui.settings

import android.content.Context
import android.os.Bundle
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
            getSharedPreferences("ftpSettings", Context.MODE_PRIVATE)

        val host = sharedPreferences.getString("host", "ftp.host.com")
        val user = sharedPreferences.getString("user", "user")
        val pass = sharedPreferences.getString("pass", "pass")

        binding.editTextHost.setText(host)
        binding.editTextUser.setText(user)
        binding.editTextPass.setText(pass)

        binding.btnSaveSettings.setOnClickListener {
            sharedPreferences.edit() {
                putString("host", binding.editTextHost.text.toString())
                putString("user", binding.editTextUser.text.toString())
                putString("pass", binding.editTextPass.text.toString())
            }
            finish()
        }
    }
}