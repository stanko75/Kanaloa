package com.milosev.kanaloa.ui.settings

import android.content.Context
import android.os.Bundle
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.edit
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.milosev.kanaloa.R
import com.milosev.kanaloa.SharedPreferencesGlobal
import com.milosev.kanaloa.databinding.ActivityJoomlaSettingsBinding

class JoomlaSettingsActivity : AppCompatActivity() {

    private lateinit var binding: ActivityJoomlaSettingsBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        binding = ActivityJoomlaSettingsBinding.inflate(layoutInflater)
        setContentView(binding.root)
        ViewCompat.setOnApplyWindowInsetsListener(binding.main) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        val sharedPreferences = getSharedPreferences(SharedPreferencesGlobal.JoomlaSettings, Context.MODE_PRIVATE)

        binding.etCategoryId.setText(sharedPreferences.getString("categoryId", ""))
        binding.etLoginUrl.setText(sharedPreferences.getString("loginUrl", ""))
        binding.etPostUrl.setText(sharedPreferences.getString("postUrl", ""))
        binding.etUserName.setText(sharedPreferences.getString("userName", ""))
        binding.etPass.setText(sharedPreferences.getString("pass", ""))

        binding.btnSave.setOnClickListener {
            sharedPreferences.edit {
                putString("categoryId", binding.etCategoryId.text.toString())
                putString("loginUrl", binding.etLoginUrl.text.toString())
                putString("postUrl", binding.etPostUrl.text.toString())
                putString("userName", binding.etUserName.text.toString())
                putString("pass", binding.etPass.text.toString())
            }
            finish()
        }

        binding.btnCancel.setOnClickListener {
            finish()
        }
    }
}