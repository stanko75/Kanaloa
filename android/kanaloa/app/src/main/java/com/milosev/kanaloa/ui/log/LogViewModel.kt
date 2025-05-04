package com.milosev.kanaloa.ui.log

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

class LogViewModel : ViewModel() {

    private val _numOfTicks = MutableLiveData<Int>().apply {
        value = 0
    }
    val numOfTicks: LiveData<Int> = _numOfTicks

    fun setTicks(value: Int) {
        _numOfTicks.value = value
    }

    private val _log = MutableLiveData<String>().apply {
        value = ""
    }
    val log: LiveData<String> get() = _log

    fun appendLog(entry: String) {
        _log.postValue(entry)
    }
}