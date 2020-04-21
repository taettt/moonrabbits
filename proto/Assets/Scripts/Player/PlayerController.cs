using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float m_hp;
    public float hp { get { return m_hp; } }
    private int m_life;
    public int life { get { return m_life; } }

    // absorp
    public Slider absorpSlider;
    [SerializeField]
    private int m_absorpValue = 0;
    public int absorpValue { get { return m_absorpValue; } set { m_absorpValue = value; } }
    private Queue<float> absorpQueue = new Queue<float>();
    private Queue<Color> absColorQueue = new Queue<Color>();

    // attacked
    private bool m_isAttacked;
    public bool isAttacked { get { return m_isAttacked; } set { m_isAttacked = value; } }
    private bool m_isLifeDown;
    public bool isLifeDown { get { return m_isLifeDown; } set { m_isLifeDown = value; } }

    public PlayerStateController sc;

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        UpdateUI();

        if(Input.GetKeyDown(KeyCode.F))
        {
            sc.curState = PlayerState.ABSORP;
        }
    }

    public void Initialize()
    {
        m_hp = 40.0f;
        m_life = 3;
        sc.curState = PlayerState.IDLE;

        absorpValue = 0;
        absorpQueue.Clear();

        m_isAttacked = false;
        m_isLifeDown = false;
    }

    private void UpdateUI()
    {
        absorpSlider.value = m_absorpValue / 30.0f;
    }

    public void DecreaseHP(float value)
    {
        if (sc.curState == PlayerState.ATTACKED || sc.curState == PlayerState.INVI
            || sc.curState == PlayerState.RETIRE)
            return;

        if (value < 8.0f)
        {
            sc.curState = PlayerState.ATTACKED;
        }
        else
        {
            sc.curState = PlayerState.NOCK;
        }
        m_isAttacked = true;

        m_hp -= value;
        if(m_hp<=0.0f)
        {
            m_life -= 1;
            m_hp = 40.0f;
            sc.curState = PlayerState.RETIRE;

            if (m_life <= 0)
            {
                sc.curState = PlayerState.DEATH;
            }
        }
    }

    // 일단 임시로 0-> 공증
    public void InputAbsorpBuff(float kind, float value, int abVal, Color color)
    {
        absorpValue += abVal;
        for (int i = 0; i < abVal; i++)
        {
            absorpQueue.Enqueue(value);
            absColorQueue.Enqueue(color);
        }
    }

    public float OutputAbsorpBuff()
    {
        if (absorpQueue.Count <= 0)
            return 0.0f;

        return absorpQueue.Dequeue();
    }

    public Color OutputAbsorpColor()
    {
        if (absColorQueue.Count <= 0)
            return new Color(0.0f, 0.0f, 0.0f, 0.0f);

        return absColorQueue.Dequeue();
    }

    public void AbsorpValueDown(int value)
    {
        m_absorpValue -= value;
    }
}
