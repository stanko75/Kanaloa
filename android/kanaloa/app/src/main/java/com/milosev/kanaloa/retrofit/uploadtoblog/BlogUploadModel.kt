package com.milosev.kanaloa.retrofit.uploadtoblog

import com.milosev.kanaloa.location.FileFolderLocationModel

class BlogUploadModel: FileFolderLocationModel() {
    var baseUrl: String? = null
    var ogImage: String? = null
    var ogTitle: String? = null
    var host: String? = null
    var user: String? = null
    var pass: String? = null
    var deleteFirstKmlPoints: String? = null
    var deleteLastKmlPoints: String? = null
    var joomlaCategoryId: String? = null
    var joomlaLoginUrl: String? = null
    var joomlaPostUrl: String? = null
    var joomlaUserName: String? = null
    var joomlaPass: String? = null
}