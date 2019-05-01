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
}
