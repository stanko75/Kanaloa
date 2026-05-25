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
import com.milosev.kanaloa.retrofit.uploadtoblog.BlogUploadModel
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
        val deleteLastKmlPoints = sharedPreferences.getString("deleteLastKmlPoints", "100")
        val deleteFirstKmlPoints = sharedPreferences.getString("deleteFirstKmlPoints", "100")

        _binding!!.editTextFileName.setText(fileName)
        _binding!!.editTextFolderName.setText(folderNameInitial)
        _binding!!.editTextOgTitle.setText(ogTitle)
        _binding!!.editTextOgImage.setText(ogImage)
        _binding!!.editTextBaseUrl.setText(baseUrl)
        _binding!!.editTextDeleteLastKmlPoints.setText(deleteLastKmlPoints)
        _binding!!.editTextDeleteFirstKmlPoints.setText(deleteFirstKmlPoints)

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
                putString("deleteLastKmlPoints", binding.editTextDeleteLastKmlPoints.text.toString())
                putString("deleteFirstKmlPoints", binding.editTextDeleteFirstKmlPoints.text.toString())
            }
            requireActivity().onBackPressedDispatcher.onBackPressed()
        }

        _binding!!.ivOgImage.setOnClickListener {
            openGallery();
        }

        _binding!!.btnUpload.setOnClickListener {

            val builder = GsonBuilder()
            val gson: Gson = builder.create()
            val blogUploadModel = BlogUploadModel()

            val ftpSharedPreferences =
                activity?.getSharedPreferences(SharedPreferencesGlobal.FtpSettings, Context.MODE_PRIVATE)

            val fileAndFolderNameSharedPreferences =
                context?.getSharedPreferences(SharedPreferencesGlobal.Settings, Context.MODE_PRIVATE)

            val joomlaSharedPreferences =
                context?.getSharedPreferences(SharedPreferencesGlobal.JoomlaSettings, Context.MODE_PRIVATE)

            blogUploadModel.host = ftpSharedPreferences?.getString("host", "ftp.host.com")
            blogUploadModel.user = ftpSharedPreferences?.getString("user", "")
            blogUploadModel.pass = ftpSharedPreferences?.getString("pass", "")
            blogUploadModel.folderName = fileAndFolderNameSharedPreferences?.getString("folderName", "")
            blogUploadModel.kmlFileName = fileAndFolderNameSharedPreferences?.getString("kmlFileName","")
            blogUploadModel.ogTitle = fileAndFolderNameSharedPreferences?.getString("ogTitle", "")
            blogUploadModel.ogImage = fileAndFolderNameSharedPreferences?.getString("ogImage","")
            blogUploadModel.baseUrl = fileAndFolderNameSharedPreferences?.getString("baseUrl","")
            blogUploadModel.deleteLastKmlPoints = fileAndFolderNameSharedPreferences?.getString("deleteLastKmlPoints","")
            blogUploadModel.deleteFirstKmlPoints = fileAndFolderNameSharedPreferences?.getString("deleteFirstKmlPoints","")

            blogUploadModel.joomlaCategoryId = joomlaSharedPreferences?.getString("categoryId", "")
            blogUploadModel.joomlaLoginUrl = joomlaSharedPreferences?.getString("loginUrl", "")
            blogUploadModel.joomlaPostUrl = joomlaSharedPreferences?.getString("postUrl", "")
            blogUploadModel.joomlaUserName = joomlaSharedPreferences?.getString("userName", "")
            blogUploadModel.joomlaPass = joomlaSharedPreferences?.getString("pass", "")

            val activity = activity as Activity
            val uploadToBlogCallbacks =
                UploadToBlogCallbacks(LogViewModelLogger(ViewModelProvider(requireActivity())[LogViewModel::class.java]), activity, blogUploadModel.folderName);

            uploadViewModel?.uploadToBlog(gson.toJson(blogUploadModel), uploadToBlogCallbacks)
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
