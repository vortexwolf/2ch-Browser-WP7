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
            if (page.ApplicationBar == null)
            {
                return;
            }

            foreach (var button in page.ApplicationBar.Buttons.OfType<ApplicationBarIconButton>())
            {
                button.Text = Strings.ResourceManager.GetString(button.Text);
            }

            foreach (var menuItem in page.ApplicationBar.MenuItems.OfType<ApplicationBarMenuItem>())
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
