
using CommunityToolkit.Maui;
using Microsoft.Maui.LifecycleEvents;
using System.Diagnostics;

namespace PKHeXMAUI;

public static class MauiProgram
{
        public static MauiApp CreateMauiApp()
        {
                var builder = MauiApp.CreateBuilder();
                builder
                        .UseMauiApp<App>()
                        .UseMauiCommunityToolkit()
                        .ConfigureLifecycleEvents(events =>
                        {
#if ANDROID
                            events.AddAndroid(android => android.OnCreate((activity, bundle) => MakeStatusBarTranslucent(activity)));

                            static void MakeStatusBarTranslucent(Android.App.Activity activity)
                            {
                                activity.Window?.SetFlags(Android.Views.WindowManagerFlags.LayoutNoLimits, Android.Views.WindowManagerFlags.LayoutNoLimits);

                                activity.Window?.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus);
                            }
#endif
                        })
                        .ConfigureFonts(fonts =>
                        {
                                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                        });

                return builder.Build();
        }
}
