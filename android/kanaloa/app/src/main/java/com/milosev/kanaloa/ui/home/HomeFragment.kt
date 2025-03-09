package com.milosev.kanaloa.ui.home

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
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

class HomeFragment : Fragment(), OnMapReadyCallback {

    private lateinit var mapView: MapView
    private lateinit var googleMap: GoogleMap

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val homeViewModel =
            ViewModelProvider(this).get(HomeViewModel::class.java)

        val rootView = inflater.inflate(R.layout.fragment_home, container, false)

        val bottomNavigationView = rootView.findViewById<BottomNavigationView>(R.id.bottom_navigation_view)
        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.navigation_home -> {
                    val serviceStarter = StartForegroundService()
                    serviceStarter.startForegroundService(context)
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
        val defaultLocation = LatLng(37.7749, -122.4194) // San Francisco, for milosev
        googleMap.addMarker(MarkerOptions().position(defaultLocation).title("Marker in San Francisco"))
        googleMap.moveCamera(CameraUpdateFactory.newLatLngZoom(defaultLocation, 10f))
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