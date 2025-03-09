package com.milosev.kanaloa.location

import com.google.android.gms.location.Granularity
import com.google.android.gms.location.LocationRequest
import com.google.android.gms.location.Priority
import java.util.concurrent.TimeUnit

class CreateLocationRequestBuilder: ICreateLocationRequestBuilder {

    override fun buildLocationRequest(interval: Long?): LocationRequest {
        var localInterval: Long = 0

        if (interval != null) {
            localInterval = interval
        }
        return LocationRequest.Builder(
            Priority.PRIORITY_HIGH_ACCURACY, TimeUnit.SECONDS.toMillis(localInterval)
        ).apply {
            setGranularity(Granularity.GRANULARITY_PERMISSION_LEVEL)
            setDurationMillis(TimeUnit.MINUTES.toMillis(Long.MAX_VALUE))
            setWaitForAccurateLocation(true)
            setMaxUpdates(Int.MAX_VALUE)
            setIntervalMillis(TimeUnit.SECONDS.toMillis(localInterval))
            setMinUpdateIntervalMillis(TimeUnit.SECONDS.toMillis(localInterval))
            setMinUpdateDistanceMeters(0F)
        }.build()
    }
}