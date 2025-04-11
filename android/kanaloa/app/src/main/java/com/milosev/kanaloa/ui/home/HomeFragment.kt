package com.milosev.kanaloa.ui.home

import android.app.Activity
import android.os.Build
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.annotation.RequiresApi
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import com.milosev.kanaloa.R
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.MapView
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.MarkerOptions
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.maps.android.data.kml.KmlLayer
import com.milosev.kanaloa.foregroundtickservice.ForegroundServiceBroadcastReceiver
import com.milosev.kanaloa.foregroundtickservice.ForegroundServiceBroadcastReceiverOnReceive
import com.milosev.kanaloa.logger.LogEntry
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.logger.LoggingEventType
import com.milosev.kanaloa.ui.log.LogFragment
import com.milosev.kanaloa.ui.log.LogViewModel
import java.net.URL

class HomeFragment : Fragment(), OnMapReadyCallback {

    private lateinit var mapView: MapView
    private lateinit var googleMap: GoogleMap
    private lateinit var viewModel: LogViewModel
    private lateinit var logViewModelLogger: LogViewModelLogger

    @RequiresApi(Build.VERSION_CODES.O)
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val rootView = inflater.inflate(R.layout.fragment_home, container, false)
        val activity = activity as Activity // Safe cast to AppCompatActivity if needed
        viewModel = ViewModelProvider(requireActivity())[LogViewModel::class.java]
        logViewModelLogger = LogViewModelLogger(viewModel)
        val bottomNavigationView = rootView.findViewById<BottomNavigationView>(R.id.bottom_navigation_view)
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.navigation_home -> {

                    val broadCastReceiver = ForegroundServiceBroadcastReceiver(
                        ForegroundServiceBroadcastReceiverOnReceive(logViewModelLogger)
                    )

                    val serviceStarter = StartForegroundService()
                    context?.let { serviceStarter.startForegroundService(it, activity, this.requireView(), broadCastReceiver) }
                    true
                }
                R.id.navigation_dashboard -> {
                    Toast.makeText(context, "Navigation Clicked", Toast.LENGTH_LONG).show()
                    true
                }
                R.id.navigation_notifications -> {
                    Toast.makeText(context, "Notification Clicked", Toast.LENGTH_LONG).show()
                    true
                }
                else -> false
            }
        }

        mapView = rootView.findViewById(R.id.map_view)
        mapView.onCreate(savedInstanceState)
        mapView.getMapAsync(this)

        return rootView
    }

    override fun onMapReady(map: GoogleMap) {
        googleMap = map

        // Initialize map settings and move the camera to a default location
        val defaultLocation = LatLng(50.1140208, 8.6846595) // San Francisco, for milosev
        googleMap.addMarker(MarkerOptions().position(defaultLocation).title("Marker in San Francisco"))
        googleMap.moveCamera(CameraUpdateFactory.newLatLngZoom(defaultLocation, 15f))
        loadKmlFromUrl("http://www.milosev.com/kmlTestDelete/kml.kml")
    }

    private fun loadKmlFromUrl(url: String) {
        Thread {
            try {
                val inputStream = URL(url).openStream()
                val kmlLayer = KmlLayer(googleMap, inputStream, context)
                requireActivity().runOnUiThread {
                    kmlLayer.addLayerToMap()
                }
            } catch (e: Exception) {
                logViewModelLogger.Log(LogEntry(LoggingEventType.Error, e.message, e))
            }
        }.start()
    }

    override fun onResume() {
        super.onResume()
        mapView.onResume()
    }

    override fun onPause() {
        super.onPause()
        mapView.onPause()
    }

    override fun onDestroy() {
        super.onDestroy()
        mapView.onDestroy()
    }

    override fun onLowMemory() {
        super.onLowMemory()
        mapView.onLowMemory()
    }
}