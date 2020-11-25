using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Int16Iterator
{
    public static Int16 next = Int16.MinValue;

    public Int16 Pop()
    {
        Int16 returnValue = next;
        if (next == Int16.MaxValue)
        {
            next = Int16.MinValue;
        }
        else
        {
            next++;
        }
        return returnValue;
    }
}
