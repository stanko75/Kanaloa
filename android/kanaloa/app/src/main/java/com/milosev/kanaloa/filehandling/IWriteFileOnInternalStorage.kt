package com.milosev.kanaloa.filehandling

import java.io.File

interface IWriteFileOnInternalStorage {
    fun execute(path: File, fileName: String?, sBody: String?)
}