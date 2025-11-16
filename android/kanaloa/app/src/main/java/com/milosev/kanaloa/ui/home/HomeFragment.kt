package com.milosev.kanaloa.ui.home

import android.app.Activity
import android.content.Context
import android.os.Build
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.activity.result.contract.ActivityResultContracts
import androidx.annotation.RequiresApi
import androidx.core.view.get
import androidx.fragment.app.Fragment
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.lifecycleScope
import com.google.android.gms.maps.CameraUpdateFactory
import com.google.android.gms.maps.GoogleMap
import com.google.android.gms.maps.MapView
import com.google.android.gms.maps.OnMapReadyCallback
import com.google.android.gms.maps.model.LatLng
import com.google.android.gms.maps.model.Marker
import com.google.android.gms.maps.model.MarkerOptions
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.milosev.kanaloa.Config
import com.milosev.kanaloa.R
import com.milosev.kanaloa.logger.LogViewModelLogger
import com.milosev.kanaloa.retrofit.CreateRetrofitBuilder
import com.milosev.kanaloa.retrofit.fetchlivelocation.FetchLiveLocation
import com.milosev.kanaloa.retrofit.fetchlivelocation.IGetLiveLocationApiService
import com.milosev.kanaloa.retrofit.loadkmlfromurl.ILoadKmlFromUrl
import com.milosev.kanaloa.retrofit.loadkmlfromurl.LoadKmlFromUrl
import com.milosev.kanaloa.retrofit.uploadimages.GsonConverter
import com.milosev.kanaloa.retrofit.uploadimages.IUploadImagesApiService
import com.milosev.kanaloa.retrofit.uploadimages.UploadImages
import com.milosev.kanaloa.retrofit.uploadimages.UploadImagesCallbacks
import com.milosev.kanaloa.ui.log.LogViewModel
import kotlinx.coroutines.Job
import kotlinx.coroutines.launch

class HomeFragment : Fragment(), OnMapReadyCallback {

    private var isMapReady: Boolean = false
    private var shouldStartLiveUpdater: Boolean = false
    private lateinit var mapView: MapView
    private lateinit var googleMap: GoogleMap
    private lateinit var logViewModel: LogViewModel
    private lateinit var logViewModelLogger: LogViewModelLogger
    private lateinit var liveUpdater: LiveLocationUpdater
    private lateinit var loadKmlFromUrl: ILoadKmlFromUrl
    private lateinit var bottomNavigationView: BottomNavigationView
    private lateinit var fetchLiveLocation: FetchLiveLocation
    private var updateJob: Job? = null

    private var kmlUpdateJob: Job? = null

    private var uploadPictures: UploadPictures? = null
    private val galleryLauncher =
        this.registerForActivityResult(ActivityResultContracts.GetMultipleContents()) { images ->
            uploadPictures?.uploadImages(images)
        }

    private var marker: Marker? = null

    @RequiresApi(Build.VERSION_CODES.O)
    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val rootView = inflater.inflate(R.layout.fragment_home, container, false)
        val activity = activity as Activity // Safe cast to AppCompatActivity if needed
        logViewModel = ViewModelProvider(requireActivity())[LogViewModel::class.java]
        logViewModelLogger = LogViewModelLogger(logViewModel)
        bottomNavigationView = rootView.findViewById(R.id.bottom_navigation_view)

        bottomNavigationView.menu.setGroupCheckable(0, true, true)
        bottomNavigationView.menu[1].isChecked = true

        fetchLiveLocation = FetchLiveLocation(
            CreateRetrofitBuilder().createRetrofitBuilder(Config(context).webHost)
                .create(IGetLiveLocationApiService::class.java), logViewModelLogger
        )
        loadKmlFromUrl = LoadKmlFromUrl(logViewModelLogger)
        liveUpdater = LiveLocationUpdater(fetchLiveLocation)

        uploadPictures = context?.let {
            UploadPictures(
                UploadImages(
                    CreateRetrofitBuilder().createRetrofitBuilder(
                        Config(it).webHost,
                        GsonConverter()
                    ).create(
                        IUploadImagesApiService::class.java
                    ), UploadImagesCallbacks(logViewModelLogger)
                ), it
            )
        }

        bottomNavigationView.setOnItemSelectedListener { item ->
            when (item.itemId) {
                R.id.navigation_start -> {

                    val kmlUrl = getKmlUrl()

                    kmlUpdateJob = lifecycleScope.launch {
                        loadKmlFromUrl.loadKmlFromUrl(kmlUrl, googleMap, context)
                    }

                    liveUpdater.marker = marker
                    updateJob = liveUpdater.start(
                        googleMap,
                        context,
                        logViewModelLogger,
                    )

                    val serviceStarter = StartForegroundService()
                    context?.let {
                        serviceStarter.startForegroundService(
                            it,
                            activity,
                            this.requireView()
                        )
                    }
                    true
                }

                R.id.navigation_stop -> {
                    kmlUpdateJob?.cancel()
                    liveUpdater.stop(logViewModelLogger, updateJob)
                    val serviceStopper = StopForegroundService()
                    context?.let { serviceStopper.stopForegroundService(it, activity) }
                    true
                }

                R.id.navigation_photo -> {


                    uploadPictures?.openGallery(galleryLauncher)
                    //Toast.makeText(context, "Notification Clicked", Toast.LENGTH_LONG).show()
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
            startLiveUpdaterNow(fetchLiveLocation)
        }
    }

    private fun startLiveUpdaterNow(fetchLiveLocation: FetchLiveLocation) {
        //ToDo SharedPreferences do not work in multi process mode
        //do it with ContentProviders
        val sharedPreferences =
            requireContext().getSharedPreferences(
                "foregroundTickServiceStatus",
                Context.MODE_PRIVATE
            )
        val foregroundTickServiceStatus = sharedPreferences.getString("status", "stopped")

        if (foregroundTickServiceStatus == "started") {
            bottomNavigationView.menu[0].isChecked = true
            liveUpdater.marker = marker
            updateJob =
                liveUpdater.start(googleMap, context,  logViewModelLogger)

            if (kmlUpdateJob?.isActive == true) {
                kmlUpdateJob?.cancel()

                val kmlUrl = getKmlUrl()
                kmlUpdateJob = lifecycleScope.launch {
                    loadKmlFromUrl.loadKmlFromUrl(kmlUrl, googleMap, context)
                }
            }

        } else {
            val url = context?.let { Config(it).webHost }
            lifecycleScope.launch {
                context?.let { fetchLiveLocation.fetchLiveLocation(it, url, googleMap) }
            }
        }
    }

    override fun onResume() {
        super.onResume()
        mapView.onResume()

        if (isMapReady) {
            startLiveUpdaterNow(fetchLiveLocation)
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