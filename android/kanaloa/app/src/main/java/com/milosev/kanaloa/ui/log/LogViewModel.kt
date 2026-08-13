package com.milosev.kanaloa.ui.log

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

class LogViewModel : ViewModel() {

    private val _numOfTicks = MutableLiveData(0)
    val numOfTicks: LiveData<Int> = _numOfTicks

    fun setTicks(value: Int) {
        _numOfTicks.value = value
    }

    private val _log = MutableLiveData("")
    val log: LiveData<String> = _log

    fun appendLog(entry: String) {
/        synchronized(this) {
            val current = _log.value.orEmpty()
            _log.postValue(current + entry + "\n")
        }
    }
}