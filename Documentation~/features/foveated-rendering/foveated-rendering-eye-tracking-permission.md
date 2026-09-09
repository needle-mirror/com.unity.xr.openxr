---
uid: openxr-foveated-rendering-eye-tracking-permission
---
# Request eye-tracking permission on Android

Declare and request the Android permission that gaze-based foveated rendering requires.

The Android platform requires the user to grant permission before your application can access eye-tracking data. Gaze-based foveated rendering requires eye-tracking permission.

## Prerequisites

Before you begin, [configure gaze-based foveated rendering](xref:openxr-foveated-rendering-gaze-based).

## Declare the permission in the Android manifest

Unity adds the eye tracking manifest entries for you when your project meets the following conditions:

* You enable the **Foveated Rendering** feature and its **Use Eye Tracking** setting. Unity enables **Use Eye Tracking** by default.
* You enable the **Meta Quest Support** feature and **Quest Pro** under its **Target Devices** section. Unity enables **Quest Pro** by default.

In the default configuration, you don't need to change the Android manifest yourself. For more information about the **Target Devices** section, refer to [Meta Quest Support](xref:openxr-meta-quest-support).

If your project doesn't meet these conditions, add `uses-feature` and `uses-permission` elements to your application's Android manifest file:

```xml
<manifest xmlns:android="http://schemas.android.com/apk/res/android" xmlns:tools="http://schemas.android.com/tools">
    <uses-feature android:name="oculus.software.eye_tracking" android:required="false" />
    <uses-permission android:name="com.oculus.permission.EYE_TRACKING" />
    ... the rest of the manifest elements ...
```

Keep `android:required="false"` unless your application requires eye-tracking hardware. A value of `false` declares eye tracking as optional, which keeps your application compatible with devices that don't support it.

Refer to [Declare permissions for an application](xref:um-android-permissions-declare) for instructions about how to add these and other custom elements to the Android manifest.

## Handle a denied permission

If the user denies permission, your application uses fixed foveated rendering instead. Refer to [Request runtime permissions](xref:um-android-requesting-permissions) for more information about handling Android permission issues.

## Additional resources

* [Configure gaze-based foveated rendering](xref:openxr-foveated-rendering-gaze-based)
* [Use the SRP foveation API](xref:openxr-foveated-rendering-srp-api)
* [Foveated rendering requirements reference](xref:openxr-foveated-rendering-requirements)
* [Eye Tracking Interaction](xref:openxr-eye-gaze-interaction)
