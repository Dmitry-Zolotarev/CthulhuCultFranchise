using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PluralEnding
{
    public static string GetEnding(int count)
    {
        if (count % 10 == 1)
        {
            return "à";
        }
        else if (count % 10 > 1 && count % 10 < 5 && count / 10 % 10 != 1)
        {
            return "";
        }
        else return ("è");
    }
}
