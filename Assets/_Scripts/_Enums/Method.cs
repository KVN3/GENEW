using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Collection of methods shared by many different files
public static class Method
{
    // If index == -1, index hasn't been found
    public static bool IndexFound(int index)
    {
        if (index == -1)
            return false;
        else
            return true;
    }

    //public static T[] RemoveAt<T>(this T[] source, int index)
    //{
    //    T[] dest = new T[source.Length - 1];
    //    if (index > 0)
    //        Array.Copy(source, 0, dest, 0, index);

    //    if (index < source.Length - 1)
    //        Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

    //    return dest;
    //}
}
