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

    public static Vector3 GetDirection(float angle)
    {
        Vector3 dir = Vector3.forward;
        var quat = Quaternion.Euler(0.0f, angle, 0.0f);
        Vector3 newDir = quat * dir;
        newDir.y = 0.0f;
        newDir = newDir.normalized;

        return newDir;
    }
}
