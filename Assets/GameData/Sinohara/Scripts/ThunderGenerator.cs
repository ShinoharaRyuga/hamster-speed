using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雷を落とすスクリプト
/// </summary>
public class ThunderGenerator : MonoBehaviour
{
    /// <summary>雷のプレハブ</summary>
    [SerializeField] GameObject m_thunderObj = default;
    /// <summary>雷を落とすまでの時間</summary>
    [Tooltip("設定した時間がランダムで選ばれます 最初は一番上の時間が設定されます。")]
    [SerializeField, Header("雷を落とすまでの時間(秒)")] float[] m_fallTime = default;
    /// <summary>雷を落とす範囲を決める</summary>
    [Tooltip("Ramndom:マップのどこかに落ちます　PlayerAround:プレイヤー周り設定した範囲内に落ちます")]
    [SerializeField, Header("雷を落とす範囲指定")] FallMode m_currentMode = FallMode.Random;
    /// <summary>プレイヤーのＸ軸の最大・最小</summary>
    [SerializeField, Header("プレイヤーのＸ軸の最大・最小")] float m_x = 0f;
    /// <summary>プレイヤーのＺ軸の最大・最小</summary>
    [SerializeField, Header("プレイヤーのＺ軸の最大・最小")] float m_z = 0f;
    /// <summary>参加してるプレイヤーのオブジェクト</summary>
    GameObject[] m_playersObj = default;
    /// <summary>本体の本体</summary>
    GameObject[] m_playersBody = default;
    /// <summary>次に雷を落とすまでの時間</summary>
    [SerializeField] float m_currentFallTime = 0f;
    float m_time = 0f;
    /// <summary>天候が雷の時だけ落とすようにする為のフラグ</summary>
    private static bool m_fallFlag = true;
    /// <summary>天候が雷の時だけ落とすようにする為のフラグ</summary>
    public static bool FallFlag { get => m_fallFlag; set => m_fallFlag = value; }

    // Start is called before the first frame update
    void Start()
    {
        SetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_fallFlag)
        {
            m_time += Time.deltaTime;
        }

        if (m_time >= m_currentFallTime)
        {
            FallThunder(m_currentMode);
            m_time = 0f;
        }
    }

    /// <summary>雷を落とす関数</summary>
    public void FallThunder(FallMode mode)
    {
        if (mode == FallMode.PlayerAround)
        {
            int playerIndex = Random.Range(0, m_playersObj.Length);
            float x = Random.Range(-m_x, m_x + 1f); //位置指定
            float z = Random.Range(-m_z, m_z + 1f); //位置指定
            GameObject targetPlayer = m_playersBody[playerIndex]; //雷が落ちるプレイヤー
            Vector3 fallPos = new Vector3(targetPlayer.transform.position.x + x, targetPlayer.transform.position.y, targetPlayer.transform.position.z + z); //落とす位置
            Instantiate(m_thunderObj, fallPos, Quaternion.identity);
        }
        else
        {
            float x = Random.Range(0, 951);
            float z = Random.Range(0, 1000);
            Instantiate(m_thunderObj, new Vector3(x, 0, z), Quaternion.identity);
            FallFlag = false;
        }

        //int index = Random.Range(0, m_fallTime.Length);
        //m_currentFallTime = m_fallTime[index];

    }

    /// <summary>プレイヤーを取得する関数</summary>
    public void SetPlayer()
    {
        m_playersObj = GameObject.FindGameObjectsWithTag("PlayerObj");
        m_playersBody = new GameObject[m_playersObj.Length];
        for (var i = 0; i < m_playersObj.Length; i++)
        {
            m_playersBody[i] = m_playersObj[i].transform.GetChild(2).gameObject;
        }
        m_currentFallTime = m_fallTime[0];
    }
}

/// <summary>雷の落とし方を設定出来るようにする為の列挙型</summary>
public enum FallMode
{
    /// <summary>マップのどこかに落ちる</summary>
    Random,
    /// <summary>プレイヤーの周り設定した範囲内に落ちる</summary>
    PlayerAround,
}
