using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodGimmick : MonoBehaviour
{
    /// <summary>木登りが出来る状態になるまでの時間</summary>
    [Tooltip("木登りが出来る状態になるまでの時間")]
    [SerializeField] float m_ChangeTime = 0f;
    /// <summary>登り中に減っていくスタミナ量</summary>
    [Tooltip("木登り中に減っていくスタミナ")]
    [SerializeField] int m_useStamina = 0;
    /// <summary>スタミナが減っていく間隔/summary>
    [Tooltip("スタミナが減っていく間隔")]
    [SerializeField] float m_useTime = 0f;
    [Tooltip("登らせたい木を設定してください")]
    /// <summary>playerが登る対象の木</summary>
    [SerializeField] Transform m_wood;
    PlayerController m_playerController = default;
    float m_woodTime = 0f;
    float m_time = 0f;
    bool m_woodFlag = false;
    bool m_changeFlag = false;
    bool m_useStaminaFlag = false;

    public bool UseStaminaFlag { get => m_useStaminaFlag; set => m_useStaminaFlag = value; }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (m_woodTime >= m_ChangeTime && !m_woodFlag)
        {
            ChangeWoodFlag();
            m_woodFlag = true;
            m_useStaminaFlag = true;
            m_woodTime = 0;
            m_changeFlag = false;
        }

        if (m_useStaminaFlag)
        {
            m_time += Time.deltaTime;
            UseStamina();
        }

        if (m_woodFlag && m_playerController.CurrentStamina == 0)
        {
            ReSetWoodFlag();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log(m_woodFlag);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_playerController = other.GetComponent<PlayerController>();
            m_playerController.JumpFlag = false;
            m_changeFlag = true;
            m_woodFlag = false;
            m_playerController.SetWoodManager(this, m_wood);
            m_playerController.ChangeCamera(2);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_playerController != null && Input.GetButton("Jump") && m_changeFlag)
        {
            m_woodTime += Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            m_playerController.JumpFlag = true;
        }
    }

    /// <summary>playerを木登り状態にする</summary>
    private void ChangeWoodFlag()
    { 
        m_playerController.CurrentState = State.wood;
    }

    /// <summary>playerの木登り状態を解除する</summary>
    private void ReSetWoodFlag()
    {
        m_playerController.JumpFlag = true;
        m_playerController.CurrentState = State.normal;
        m_useStaminaFlag = false;
        m_woodFlag = false;
        m_playerController.SetGravity(true);
    }

    /// <summary>木登り中にスタミナを減らす関数</summary>
    private void UseStamina()
    {
        if (m_time >= m_useTime && ReduceStamina() >= 0)
        {
            m_time = 0f;
        }
        else if(m_time >= m_useTime)
        {
            m_time = 0f;
        }
    }

    private int ReduceStamina()
    {
        int Stamina = Mathf.Max(m_playerController.CurrentStamina -= m_useStamina, 0);
        m_playerController.CurrentStamina = Stamina;
        return Stamina;
    }
}
