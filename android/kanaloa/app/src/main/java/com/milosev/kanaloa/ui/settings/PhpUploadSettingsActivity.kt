package com.milosev.kanaloa.ui.settings

import android.os.Bundle
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.edit
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.milosev.kanaloa.SharedPreferencesGlobal
import com.milosev.kanaloa.databinding.ActivityPhpUploadSettingsBinding

class PhpUploadSettingsActivity : AppCompatActivity() {

    private lateinit var binding: ActivityPhpUploadSettingsBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        binding = ActivityPhpUploadSettingsBinding.inflate(layoutInflater)
        setContentView(binding.root)
        ViewCompat.setOnApplyWindowInsetsListener(binding.main) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        val sharedPreferences = getSharedPreferences(SharedPreferencesGlobal.JoomlaSettings, MODE_PRIVATE)
        binding.etPhpUploadUrl.setText(sharedPreferences.getString("phpUploadUrl", ""))

        binding.btnSave.setOnClickListener {
            sharedPreferences.edit {
                putString("phpUploadUrl", binding.etPhpUploadUrl.text.toString())
            }
            finish()
        }

        binding.btnCancel.setOnClickListener {
            finish()
        }
    }
}