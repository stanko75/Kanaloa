package com.milosev.kanaloa.ui.log

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import com.milosev.kanaloa.databinding.FragmentLogBinding

class LogFragment : Fragment() {

    private var _binding: FragmentLogBinding? = null

    // This property is only valid between onCreateView and
    // onDestroyView.
    private val binding get() = _binding!!

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val logViewModel =
            ViewModelProvider(requireActivity())[LogViewModel::class.java]

        logViewModel.numOfTicks.observe(viewLifecycleOwner) { ticks ->
            binding.textViewNumberOfTicks.text = ticks.toString()
        }

        logViewModel.log.observe(viewLifecycleOwner) { logText ->
            binding.editTextTextMultiLineLog.append(logText)
        }

        _binding = FragmentLogBinding.inflate(inflater, container, false)
        val root: View = binding.root

        return root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }
}