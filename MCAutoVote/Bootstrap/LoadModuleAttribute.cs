using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace MCAutoVote.Bootstrap
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class LoadModuleAttribute : Attribute {

        public static void LoadAll()
        {
            foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(LoadModuleAttribute), false).Length > 0)
                {
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
            }
        }
    }
}
