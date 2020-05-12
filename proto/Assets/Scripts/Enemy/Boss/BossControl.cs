using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : MonoBehaviour
{
    private bool m_randomRound;
    public bool randomRound { get { return m_randomRound; } set { m_randomRound = value; } }
    public Queue<int> phaseRandQueue;

    public virtual void Initialize()
    {

    }

    public void SetRandQueue(int patternCount)
    {
        if (phaseRandQueue.Count > 0)
            return;

        int[] randArr = Utility.SetRandArry(patternCount);
        phaseRandQueue.Clear();

        int index = 0;
        while (phaseRandQueue.Count < patternCount)
        {
            phaseRandQueue.Enqueue(randArr[index]);
            index++;
        }
    }
}
