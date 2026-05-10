package com.milosev.kanaloa.ui

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import com.milosev.kanaloa.Config
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.retrofit.CreateRetrofitBuilder
import com.milosev.kanaloa.retrofit.uploadimages.GsonConverter
import com.milosev.kanaloa.retrofit.uploadimages.IUploadImagesApiService
import com.milosev.kanaloa.retrofit.uploadimages.UploadImages
import com.milosev.kanaloa.retrofit.uploadimages.UploadImagesCallbacks
import com.milosev.kanaloa.retrofit.uploadtoblog.IUploadToBlogApiService
import com.milosev.kanaloa.retrofit.uploadtoblog.UploadToBlog
import com.milosev.kanaloa.retrofit.uploadtoblog.IUploadToBlogCallbacks
import com.milosev.kanaloa.ui.home.UploadPictures
import com.milosev.kanaloa.ui.log.LogViewModel

class UploadViewModel(application: Application) : AndroidViewModel(application) {
    var uploadPictures: UploadPictures? = null
        private set

    private var uploadToBlogApiService: IUploadToBlogApiService? = null
    
    fun initialize(logViewModel: LogViewModel) {
        if (uploadPictures != null) return

        val context = getApplication<Application>()
        val logViewModelLogger = LogViewModelLogger(logViewModel)

        uploadPictures = UploadPictures(
            UploadImages(
                CreateRetrofitBuilder().createRetrofitBuilder(
                    Config(context).webHost,
                    GsonConverter()
                ).create(
                    IUploadImagesApiService::class.java
                ), UploadImagesCallbacks(logViewModelLogger)
            ), context
        )

        uploadToBlogApiService = CreateRetrofitBuilder().createRetrofitBuilder(Config(context).webHost)
            .create(IUploadToBlogApiService::class.java)
    }

    fun uploadToBlog(jsonModel: String, callbacks: IUploadToBlogCallbacks) {
        uploadToBlogApiService?.let {
            UploadToBlog(it, callbacks).uploadToBlogHttpPost(jsonModel)
        }
    }
}
