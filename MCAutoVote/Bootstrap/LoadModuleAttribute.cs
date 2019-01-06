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
                    try
                    {
                        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                    }catch(Exception e)
                    {
                        if(e.InnerException != null)
                            throw Rethrow(e.InnerException);
                        throw;
                    }
                }
            }
        }

        private static Exception Rethrow(Exception ex)
        {
            typeof(Exception).GetMethod("PrepForRemoting",
                BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(ex, new object[0]);
            throw ex;
        }
    }
}
