package com.milosev.kanaloa.filehandling

import java.io.File
import java.io.FileWriter

class WriteFileOnInternalStorage: IWriteFileOnInternalStorage {
    override fun execute(path: File, fileName: String?, body: String?) {
        if (!path.exists()) {
            path.mkdir()
        }

        try {
            val file = fileName?.let { File(path, it) }
            val writer = FileWriter(file)
            writer.append(body)
            writer.flush()
            writer.close()
        } catch (e: Exception) {
            e.printStackTrace()
        }

    }
}