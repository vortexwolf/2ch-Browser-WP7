using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DvachBrowser.Assets
{
    public class ThemeManager
    {
        private bool? _isCurrentDarkTheme;

        public ThemeManager()
        {
            this.Resources = Application.Current.Resources;
        }

        public ResourceDictionary Resources { get; set; }

        public void ApplyTheme()
        {
            bool isDarkTheme = (Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible;
            if (this._isCurrentDarkTheme == isDarkTheme)
            {
                return;
            }

            this._isCurrentDarkTheme = isDarkTheme;

            Color background = this.GetColor("Background", isDarkTheme);
            Color threadTitle = this.GetColor("ThreadTitle", isDarkTheme);
            Color primaryText = this.GetColor("PrimaryText", isDarkTheme);
            Color itemInfo = this.GetColor("ItemInfo", isDarkTheme);
            Color listSeparator = this.GetColor("ListSeparator", isDarkTheme);
            Color postBackground = this.GetColor("PostBackground", isDarkTheme);
            Color postBorder = this.GetColor("PostBorder", isDarkTheme);
            Color postnumber = this.GetColor("PostNumber", isDarkTheme);
            Color linkForeground = this.GetColor("LinkForeground", isDarkTheme);

            this.SetColor("ThemeBackground", background);
            this.SetColor("ThemeThreadTitle", threadTitle);
            this.SetColor("ThemePrimaryText", primaryText);
            this.SetColor("ThemeItemInfo", itemInfo);
            this.SetColor("ThemeListSeparator", listSeparator);
            this.SetColor("ThemePostBorder", postBorder);
            this.SetColor("ThemePostBackground", postBackground);
            this.SetColor("ThemePostNumber", postnumber);
            this.SetColor("ThemeLinkForeground", linkForeground);
        }

        private Color GetColor(string key, bool isDarkTheme)
        {
            string prefix = isDarkTheme ? "Neutron" : "Photon";

            return this.GetColor(prefix + key);
        }

        private Color GetColor(string key)
        {
            return ((SolidColorBrush)this.Resources[key]).Color;
        }

        private void SetColor(string key, Color color)
        {
            ((SolidColorBrush)this.Resources[key]).Color = color;
        }
    }
}
