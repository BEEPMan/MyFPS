using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static bool GetRandomResult(int prob)
    {
        int result = 100 - Random.Range(0, 100);
        return result <= prob;
    }
}
