using System.Collections;
using System.Collections.Generic;

public class LazySingleton<T> where T : new()
{
    private static object instLock = new object();
    private static T instance;

    public static T GetInstance()
    {
        if(instance == null)
        {
            lock (instLock)
            {
                if (instance == null)
                    instance = new T();
            }
        }
        return instance;
    }
}
