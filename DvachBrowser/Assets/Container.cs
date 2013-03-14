using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DvachBrowser.Assets
{
    public class Container
    {
        private static readonly Dictionary<Type, Type> Registrations = new Dictionary<Type, Type>();
        private static readonly Dictionary<Type, object> Instances = new Dictionary<Type, object>();

        public static void Register<TInterface, TImplementation>() where TImplementation : TInterface, new()
        {
            Register(typeof(TInterface), typeof(TImplementation));
        }

        public static void Register(Type interfaceType, Type implementationType)
        {
            if (!Registrations.ContainsKey(interfaceType))
            {
                Registrations.Add(interfaceType, implementationType);
            } 
            else
            {
                Registrations[interfaceType] = implementationType;
            }
        }

        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public static object Resolve(Type type)
        {
            if (!Registrations.ContainsKey(type))
            {
                return null;
            }

            var registration = Registrations[type];

            if (Instances.ContainsKey(type))
            {
                return Instances[type];
            }
            else
            {
                var instance = CreateInstance(registration);
                Instances.Add(type, instance);

                return instance;
            }
        }

        private static object CreateInstance(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];
            ParameterInfo[] parameters = constructor.GetParameters();

            var arguments = parameters.Select(p => Resolve(p.ParameterType)).ToArray();

            var instance = constructor.Invoke(arguments);

            return instance;
        }
    }
}
