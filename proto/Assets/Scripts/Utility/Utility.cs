using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static int[] SetRandArry(int patternCount)
    {
        int[] tempArr = new int[patternCount];
        bool rand;

        for (int i = 0; i < patternCount; ++i)
        {
            while (true)
            {
                tempArr[i] = Random.Range(0, patternCount);
                rand = false;

                for (int j = 0; j < i; ++j)
                {
                    if (tempArr[j] == tempArr[i])
                    {
                        rand = true;
                        break;
                    }
                }

                if (!rand)
                    break;
            }
        }

        return tempArr;
    }
}
