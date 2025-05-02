package com.milosev.kanaloa.ui.gallery

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.content.edit
import androidx.fragment.app.Fragment
import com.milosev.kanaloa.databinding.FragmentGalleryBinding

class GalleryFragment : Fragment() {

    private var _binding: FragmentGalleryBinding? = null

    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        _binding = FragmentGalleryBinding.inflate(inflater, container, false)
        val root: View = binding.root

        /*
        val galleryViewModel =
            ViewModelProvider(this)[GalleryViewModel::class.java]
        val textView: TextView = binding.textGallery
        galleryViewModel.text.observe(viewLifecycleOwner) {
            textView.text = it
        }
         */

        val sharedPreferences = requireContext().getSharedPreferences("settings", Context.MODE_PRIVATE)

        var fileName = sharedPreferences.getString("kmlFileName", "default")
        val folderName = sharedPreferences.getString("folderName", "default")

        _binding!!.editTextFileName.setText(fileName)
        _binding!!.editTextFolderName.setText(folderName)

        _binding!!.btnSaveSettings.setOnClickListener {

            fileName = binding.editTextFileName.text.toString()
            if (!fileName!!.endsWith(".kml", ignoreCase = true)) {
                fileName += ".kml"
            }
                        
            sharedPreferences.edit {
                putString("kmlFileName", fileName)
                putString("folderName", binding.editTextFolderName.text.toString())
            }
            requireActivity().onBackPressedDispatcher.onBackPressed()
        }

        return root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}