// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

plugins {
    id("com.android.library")
    id("org.jetbrains.kotlin.android")
    id("org.jetbrains.kotlin.plugin.compose")
    id("maven-publish")
}

group = "com.github.AnointedAutomation.AnointedAutomation"
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

    publishing {
        singleVariant("release") {
            withSourcesJar()
        }
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

afterEvaluate {
    publishing {
        publications {
            register<MavenPublication>("release") {
                from(components["release"])
                groupId = project.group.toString()
                artifactId = "anointedsso"
                version = project.version.toString()
            }
        }
    }
}
