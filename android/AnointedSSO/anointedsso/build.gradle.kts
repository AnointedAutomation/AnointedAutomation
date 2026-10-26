// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

plugins {
    id("com.android.library")
    id("org.jetbrains.kotlin.android")
    id("org.jetbrains.kotlin.plugin.compose")
    id("com.vanniktech.maven.publish")
}

group = "com.anointedautomation"
version = file("../VERSION").readText().trim()

android {
    namespace = "com.anointedautomation.sso"
    compileSdk = 35

    defaultConfig {
        minSdk = 24
        consumerProguardFiles("consumer-rules.pro")
    }

    buildFeatures {
        compose = true
    }

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }

    kotlinOptions {
        jvmTarget = "17"
    }
}

dependencies {
    api("androidx.browser:browser:1.8.0")

    // Optional: only needed if you use AnointedSignInButton. Your app already has Compose if it uses the button.
    compileOnly(platform("androidx.compose:compose-bom:2024.12.01"))
    compileOnly("androidx.compose.foundation:foundation")
    compileOnly("androidx.compose.material3:material3")

    testImplementation("junit:junit:4.13.2")
    // The Compose compiler plugin also compiles the unit tests, and it refuses to run without the Compose runtime on
    // that classpath (compileOnly does not reach it).
    testImplementation(platform("androidx.compose:compose-bom:2024.12.01"))
    testImplementation("androidx.compose.runtime:runtime")
}

// Maven Central (Central Portal), published by .github/workflows/maven-publish.yml on master. Credentials and the
// signing key come from ORG_GRADLE_PROJECT_* secrets in CI; a local publishToMavenLocal needs neither.
mavenPublishing {
    configure(
        com.vanniktech.maven.publish.AndroidSingleVariantLibrary(
            variant = "release",
            sourcesJar = true,
            publishJavadocJar = true,
        ),
    )
    publishToMavenCentral(com.vanniktech.maven.publish.SonatypeHost.CENTRAL_PORTAL, automaticRelease = true)
    signAllPublications()
    coordinates("com.anointedautomation", "anointedsso", project.version.toString())

    pom {
        name.set("AnointedSSO")
        description.set("Sign in with Anointed Automation for Android apps (OAuth 2.0 + PKCE via Custom Tabs).")
        url.set("https://github.com/AnointedAutomation/AnointedAutomation")
        licenses {
            license {
                name.set("MIT License")
                url.set("https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE")
            }
        }
        developers {
            developer {
                id.set("anointedautomation")
                name.set("Anointed Automation Corporation")
                url.set("https://www.anointedautomation.net")
            }
        }
        scm {
            url.set("https://github.com/AnointedAutomation/AnointedAutomation")
            connection.set("scm:git:https://github.com/AnointedAutomation/AnointedAutomation.git")
            developerConnection.set("scm:git:ssh://git@github.com/AnointedAutomation/AnointedAutomation.git")
        }
    }
}
