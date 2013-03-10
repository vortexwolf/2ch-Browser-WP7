using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace DvachBrowser.Assets
{
    public class Container
    {
        private static readonly Dictionary<Type, Type> _registrations = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();

        public static void Register<T, U>() where U : T, new()
        {
            if (!_registrations.ContainsKey(typeof(T)))
            {
                _registrations.Add(typeof(T), typeof(U));
            }
        }

        public static T Resolve<T>()
        {
            if (_objects.ContainsKey(typeof(T)))
            {
                return (T)_objects[typeof(T)];
            }
            else
            {
                var registration = _registrations[typeof(T)];
                var instance = Activator.CreateInstance(registration);
                _objects.Add(typeof(T), instance);

                return (T)instance;
            }
        }
    }
}
