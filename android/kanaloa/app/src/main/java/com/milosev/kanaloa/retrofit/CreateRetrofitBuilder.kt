package com.milosev.kanaloa.retrofit

import android.util.Log
import com.milosev.kanaloa.MainActivity
import okhttp3.OkHttpClient
import retrofit2.Retrofit
import java.security.cert.X509Certificate
import java.util.concurrent.TimeUnit
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

class CreateRetrofitBuilder: ICreateRetrofitBuilder {

    override fun createRetrofitBuilder(baseUrl: String, converterType: IConverterType): Retrofit {
        return Retrofit.Builder()
            .baseUrl(baseUrl)
            .client(trustAllCertificates())
            .addConverterFactory(converterType.getFactory())
            .build()
    }

    private fun trustAllCertificates(): OkHttpClient {
        val trustAllCerts = arrayOf<TrustManager>(object : X509TrustManager {
            override fun checkClientTrusted(chain: Array<out X509Certificate>?, authType: String?) {
                Log.i(MainActivity::class.simpleName, "checkClientTrusted")
            }

            override fun checkServerTrusted(chain: Array<out X509Certificate>?, authType: String?) {
                Log.i(MainActivity::class.simpleName, "checkServerTrusted")
            }

            override fun getAcceptedIssuers() = arrayOf<X509Certificate>()
        })
        val sslContext = SSLContext.getInstance("SSL")
        sslContext.init(null, trustAllCerts, java.security.SecureRandom())

// Create an ssl socket factory with our all-trusting manager
        val sslSocketFactory = sslContext.socketFactory

// connect to server
        return OkHttpClient.Builder().connectTimeout(120, TimeUnit.SECONDS).readTimeout(120, TimeUnit.SECONDS).sslSocketFactory(sslSocketFactory, trustAllCerts[0] as X509TrustManager).hostnameVerifier{ _, _ -> true }.build()
    }
}