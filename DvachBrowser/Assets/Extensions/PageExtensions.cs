using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using DvachBrowser.Assets.Resources;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DvachBrowser.Assets.Extensions
{
    public static class PageExtensions
    {
        public static void LocalizeAppBar(this PhoneApplicationPage page)
        {
            LocalizeAppBar(page, page.ApplicationBar);
        }

        public static void LocalizeAppBar(this PhoneApplicationPage page, IApplicationBar appBar)
        {
            if (appBar == null)
            {
                return;
            }

            foreach (var button in appBar.Buttons.OfType<ApplicationBarIconButton>())
            {
                button.Text = Strings.ResourceManager.GetString(button.Text);
            }

            foreach (var menuItem in appBar.MenuItems.OfType<ApplicationBarMenuItem>())
            {
                menuItem.Text = Strings.ResourceManager.GetString(menuItem.Text);
            }
        }

        public static void AddValidationBinding(this PhoneApplicationPage page)
        {
            page.BindingValidationError += (s, e) => VisualStateManager.GoToState((Control)e.OriginalSource, e.Action == ValidationErrorEventAction.Added ? "Invalid" : "Valid", false);
        }
    }
}
