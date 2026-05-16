package com.milosev.kanaloa.ui.gallery

import android.app.Activity
import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.provider.OpenableColumns
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.activity.result.contract.ActivityResultContracts
import androidx.core.content.edit
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import com.google.gson.Gson
import com.google.gson.GsonBuilder
import com.milosev.kanaloa.SharedPreferencesGlobal
import com.milosev.kanaloa.databinding.FragmentGalleryBinding
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.retrofit.uploadtoblog.FtpModel
import com.milosev.kanaloa.retrofit.uploadtoblog.UploadToBlogCallbacks
import com.milosev.kanaloa.ui.UploadViewModel
import com.milosev.kanaloa.ui.log.LogViewModel

class GalleryFragment : Fragment() {

    private var _binding: FragmentGalleryBinding? = null

    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    private var uploadViewModel: UploadViewModel? = null

    private val galleryLauncher =
        this.registerForActivityResult(ActivityResultContracts.OpenMultipleDocuments()) { images ->
            if (images.isNotEmpty()) {
                val firstImage = images[0]
                
                // Persist access to the URI so it can be reloaded after app restart
                val takeFlags: Int = Intent.FLAG_GRANT_READ_URI_PERMISSION
                requireContext().contentResolver.takePersistableUriPermission(firstImage, takeFlags)

                _binding!!.ivOgImage.setImageURI(firstImage)
                _binding!!.editTextOgImage.setText("thumbs/${getFileName(firstImage)}")
                
                var fileName = _binding!!.editTextFileName.text.toString()
                if (!fileName.endsWith(".kml", ignoreCase = true)) {
                    fileName += ".kml"
                }
                val folderName = _binding!!.editTextFolderName.text.toString()

                val sharedPreferences = requireContext().getSharedPreferences(SharedPreferencesGlobal.Settings, Context.MODE_PRIVATE)
                sharedPreferences.edit(commit = true) {
                    putString("localOgImageUri.$folderName.$fileName", firstImage.toString())
                }

                uploadViewModel?.uploadPictures?.uploadImages(images)
            }
        }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentGalleryBinding.inflate(inflater, container, false)
        val root: View = binding.root

        val logViewModel = ViewModelProvider(requireActivity())[LogViewModel::class.java]
        uploadViewModel = ViewModelProvider(requireActivity())[UploadViewModel::class.java]
        uploadViewModel?.initialize(logViewModel)

        val sharedPreferences = requireContext().getSharedPreferences(SharedPreferencesGlobal.Settings, Context.MODE_PRIVATE)

        val folderNameInitial = sharedPreferences.getString("folderName", "default")
        var fileNameInitial = sharedPreferences.getString("kmlFileName", "default.kml")
        // Handle case where it might have been saved as "default" previously
        if (fileNameInitial == "default") fileNameInitial = "default.kml"

        val localOgImageUri = sharedPreferences.getString("localOgImageUri.$folderNameInitial.$fileNameInitial", null)
        if (localOgImageUri != null) {
            _binding!!.ivOgImage.setImageURI(Uri.parse(localOgImageUri))
        }

        var fileName = fileNameInitial
        val ogTitle = sharedPreferences.getString("ogTitle", "default")
        val ogImage = sharedPreferences.getString("ogImage", "default")
        val baseUrl = sharedPreferences.getString("baseUrl", "default")

        _binding!!.editTextFileName.setText(fileName)
        _binding!!.editTextFolderName.setText(folderNameInitial)
        _binding!!.editTextOgTitle.setText(ogTitle)
        _binding!!.editTextOgImage.setText(ogImage)
        _binding!!.editTextBaseUrl.setText(baseUrl)

        _binding!!.btnSaveSettings.setOnClickListener {

            fileName = binding.editTextFileName.text.toString()
            if (!fileName!!.endsWith(".kml", ignoreCase = true)) {
                fileName += ".kml"
            }

            sharedPreferences.edit {
                putString("kmlFileName", fileName)
                putString("folderName", binding.editTextFolderName.text.toString())
                putString("ogTitle", binding.editTextOgTitle.text.toString())
                putString("ogImage", binding.editTextOgImage.text.toString())
                putString("baseUrl", binding.editTextBaseUrl.text.toString())
            }
            requireActivity().onBackPressedDispatcher.onBackPressed()
        }

        _binding!!.ivOgImage.setOnClickListener {
            openGallery();
        }

        _binding!!.btnUpload.setOnClickListener {

            val builder = GsonBuilder()
            val gson: Gson = builder.create()
            val ftpModel = FtpModel()

            val ftpSharedPreferences =
                activity?.getSharedPreferences(SharedPreferencesGlobal.FtpSettings, Context.MODE_PRIVATE)

            val fileAndFolderNameSharedPreferences =
                context?.getSharedPreferences(SharedPreferencesGlobal.Settings, Context.MODE_PRIVATE)

            ftpModel.host = ftpSharedPreferences?.getString("host", "ftp.host.com")
            ftpModel.user = ftpSharedPreferences?.getString("user", "")
            ftpModel.pass = ftpSharedPreferences?.getString("pass", "")
            ftpModel.folderName = fileAndFolderNameSharedPreferences?.getString("folderName", "")
            ftpModel.kmlFileName = fileAndFolderNameSharedPreferences?.getString("kmlFileName","")
            ftpModel.ogTitle = fileAndFolderNameSharedPreferences?.getString("ogTitle", "")
            ftpModel.ogImage = fileAndFolderNameSharedPreferences?.getString("ogImage","")
            ftpModel.baseUrl = fileAndFolderNameSharedPreferences?.getString("baseUrl","")

            val activity = activity as Activity
            val uploadToBlogCallbacks =
                UploadToBlogCallbacks(LogViewModelLogger(ViewModelProvider(requireActivity())[LogViewModel::class.java]), activity, ftpModel.folderName);

            uploadViewModel?.uploadToBlog(gson.toJson(ftpModel), uploadToBlogCallbacks)
        }

        return root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }

    private fun getFileName(uri: Uri): String? {
        var name: String? = null

        val cursor = requireContext().contentResolver.query(uri, null, null, null, null)
        cursor?.use {
            val nameIndex = it.getColumnIndex(OpenableColumns.DISPLAY_NAME)
            if (it.moveToFirst() && nameIndex != -1) {
                name = it.getString(nameIndex)
            }
        }

        return name
    }

    private fun openGallery() {
        galleryLauncher.launch(arrayOf("image/*"))
    }
}
