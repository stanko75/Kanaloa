package com.milosev.kanaloa.ui.home

import android.app.Activity
import android.content.Context
import android.os.Build
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.annotation.RequiresApi
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.google.android.gms.maps.CameraUpdateFactory
import com.milosev.kanaloa.R
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.MapView
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.Marker
import com.google.android.gms.maps.model.MarkerOptions
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.milosev.kanaloa.Config
import com.milosev.kanaloa.foregroundtickservice.ForegroundServiceBroadcastReceiver
import com.milosev.kanaloa.foregroundtickservice.ForegroundServiceBroadcastReceiverOnReceive
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.ui.log.LogViewModel
import okhttp3.OkHttpClient
import androidx.core.view.get
import kotlinx.coroutines.launch

class HomeFragment : Fragment(), OnMapReadyCallback {

    private var isMapReady: Boolean = false
    private var shouldStartLiveUpdater: Boolean = false
    private lateinit var mapView: MapView
    private lateinit var googleMap: GoogleMap
    private lateinit var viewModel: LogViewModel
    private lateinit var logViewModelLogger: LogViewModelLogger
    private val client = OkHttpClient()
    private lateinit var liveUpdater: LiveLocationUpdater
    private lateinit var bottomNavigationView: BottomNavigationView

    private var marker: Marker? = null

    @RequiresApi(Build.VERSION_CODES.O)
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val rootView = inflater.inflate(R.layout.fragment_home, container, false)
        val activity = activity as Activity // Safe cast to AppCompatActivity if needed
        viewModel = ViewModelProvider(requireActivity())[LogViewModel::class.java]
        logViewModelLogger = LogViewModelLogger(viewModel)
        bottomNavigationView = rootView.findViewById<BottomNavigationView>(R.id.bottom_navigation_view)

        bottomNavigationView.menu.setGroupCheckable(0, true, true)
        bottomNavigationView.menu[1].isChecked = true

        val broadCastReceiver = ForegroundServiceBroadcastReceiver(
            ForegroundServiceBroadcastReceiverOnReceive(logViewModelLogger)
        )

        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.navigation_home -> {

                    val kmlUrl = getKmlUrl()

                    liveUpdater.marker = marker;
                    liveUpdater.start(googleMap, context, requireActivity(), logViewModelLogger, kmlUrl)

                    val serviceStarter = StartForegroundService()
                    context?.let { serviceStarter.startForegroundService(it, activity, this.requireView(), broadCastReceiver) }
                    true
                }
                R.id.navigation_dashboard -> {
                    val serviceStopper = StopForegroundService()
                    context?.let { serviceStopper.stopForegroundService(it, activity, broadCastReceiver) }
                    liveUpdater.stop()
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

    private fun getKmlUrl(): String {
        val sharedPreferences =
            requireContext().getSharedPreferences("settings", Context.MODE_PRIVATE)
        val fileName = sharedPreferences.getString("kmlFileName", "default.kml")
        val folderName = sharedPreferences.getString("folderName", "default")
        val webHost = context?.let { Config(it).webHost }
        val kmlUrl = "$webHost/$folderName/$fileName"
        return kmlUrl
    }

    override fun onMapReady(map: GoogleMap) {
        googleMap = map
        isMapReady = true

        // Initialize map settings and move the camera to a default location
        val defaultLocation = LatLng(37.3489817, -122.0661283)
        marker = googleMap.addMarker(
            MarkerOptions()
                .position(defaultLocation)
                .title("Live Marker")
        )
        googleMap.moveCamera(CameraUpdateFactory.newLatLngZoom(defaultLocation, 15f))

        if (shouldStartLiveUpdater) {
            startLiveUpdaterNow()
        }
    }

    private fun startLiveUpdaterNow() {
        val sharedPreferences =
            requireContext().getSharedPreferences(
                "foregroundTickServiceStatus",
                Context.MODE_PRIVATE
            )
        val foregroundTickServiceStatus = sharedPreferences.getString("status", "stopped")

        val kmlUrl = getKmlUrl()
        liveUpdater = LiveLocationUpdater(googleMap, client, lifecycleScope)
        if (foregroundTickServiceStatus == "started") {
            bottomNavigationView.menu[0].isChecked = true
            liveUpdater.marker = marker;
            liveUpdater.start(googleMap, context, requireActivity(), logViewModelLogger, kmlUrl)
        } else {
            liveUpdater.loadKmlFromUrl(
                kmlUrl,
                googleMap,
                context,
                this.requireActivity(),
                logViewModelLogger
            )
            val url = context?.let { Config(it).webHost };
            lifecycleScope.launch {
                liveUpdater.loadLocationFromUrlAndMoveCamera(url, googleMap)
            }
        }
    }

    override fun onResume() {
        super.onResume()
        mapView.onResume()

        if (isMapReady) {
            startLiveUpdaterNow()
        } else {
            shouldStartLiveUpdater = true
        }
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