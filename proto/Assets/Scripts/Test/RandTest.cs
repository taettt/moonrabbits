using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandTest : MonoBehaviour
{
    private Queue<int> m_phaseRandQueue = new Queue<int>();

    void Start()
    {
        SetRandQueue(3);
    }

    private void SetRandQueue(int patternCount)
    {
        if (m_phaseRandQueue.Count > 0)
            return;

        int[] randArr = SetRandArry(3);
        m_phaseRandQueue.Clear();

        int index = 0;
        while (m_phaseRandQueue.Count < patternCount)
        {
            m_phaseRandQueue.Enqueue(randArr[index]);
            index++;
        }

        Debug.Log(m_phaseRandQueue.Dequeue());
        Debug.Log(m_phaseRandQueue.Dequeue());
        Debug.Log(m_phaseRandQueue.Dequeue());
    }

    private int[] SetRandArry(int patternCount)
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
