using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace AsusRouterApp.Class
{
    public class Utils
    {
        public class UI
        {
            public static AcrylicBrush GetAcrylicBrush(AcrylicBackgroundSource acrylicBackground,Color TintColor, Color FallbackColor,double TintOpacity)
            {
                var brush = new AcrylicBrush();
                brush.BackgroundSource = acrylicBackground;
                brush.TintColor = TintColor;
                brush.FallbackColor = FallbackColor;
                brush.TintOpacity = TintOpacity;
                return brush;
            }

            /// <summary>
            /// 循环查询用户主题色,变化时事件通知
            /// </summary>
            public class AccentColor
            {
                public Color accentColor;

                private TimeSpan updateSpan;

                private ThreadPoolTimer updateTimer;

                public delegate void AccentColorChangedDelegate(Color value);

                public event AccentColorChangedDelegate AccentColorChanged;

                public AccentColor()
                {
                    try
                    {
                        accentColor = Edi.UWP.Helpers.UI.GetAccentColor();
                    }
                    catch (Exception)
                    {
                        accentColor = Colors.Gray;
                    }
                    updateSpan = TimeSpan.FromMilliseconds(1000);
                    updateTimer = ThreadPoolTimer.CreatePeriodicTimer(TimerEvent,updateSpan);
                }

                private void TimerEvent(ThreadPoolTimer timer)
                {
                    try
                    {
                        var temp = Edi.UWP.Helpers.UI.GetAccentColor();
                        if (temp != accentColor)
                        {
                            accentColor = temp;
                            AccentColorChanged?.Invoke(accentColor);
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
        }

        public static class AppResources
        {
            private static ResourceLoader CurrentResourceLoader
            {
                get { return _loader ?? (_loader = ResourceLoader.GetForCurrentView("Resources")); }
            }

            private static ResourceLoader _loader;
            private static readonly Dictionary<string, string> ResourceCache = new Dictionary<string, string>();

            public static string GetString(string key)
            {
                string s;
                if (ResourceCache.TryGetValue(key, out s))
                {
                    return s;
                }
                else
                {
                    s = CurrentResourceLoader.GetString(key);
                    ResourceCache[key] = s;
                    return s;
                }
            }



            /// <summary>
            /// AppName
            /// </summary>
            public static string AppName
            {
                get
                {
                    return CurrentResourceLoader.GetString("AppName");
                }
            }
        }
    }
}
