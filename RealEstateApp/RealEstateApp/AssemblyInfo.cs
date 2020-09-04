using Android.App;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

[assembly: ExportFont("fa-solid-900.ttf", Alias = "FA-solid")]

// Geolocation (Permission for location)
[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesFeature("android.hardware.location", Required = false)]
[assembly: UsesFeature("android.hardware.location.gps", Required = false)]
[assembly: UsesFeature("android.hardware.location.network", Required = false)]

// Connectivity (Permission for network state)
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]


// Vibrate
[assembly: UsesPermission(Android.Manifest.Permission.Vibrate)]

// Battery
[assembly: UsesPermission(Android.Manifest.Permission.BatteryStats)]

// Flashlight & Camera (Both are needed)
[assembly: UsesPermission(Android.Manifest.Permission.Flashlight)]
[assembly: UsesPermission(Android.Manifest.Permission.Camera)]