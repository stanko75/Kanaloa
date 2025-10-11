package com.milosev.kanaloa.retrofit.uploadtoblog

import com.milosev.kanaloa.location.FileFolderLocationModel

class FtpModel: FileFolderLocationModel() {
    var baseUrl: String? = null
    var ogImage: String? = null
    var ogTitle: String? = null
    var host: String? = null
    var user: String? = null
    var pass: String? = null
}