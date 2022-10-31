using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// playerの移動制限やクリア判定を
/// するスクリプト ゲーム進行全般
/// </summary>
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Text m_clearText = default;
   
    TimeManager m_timeManager = default;

    /// <summary>ルームにいるプレイヤーの人数 </summary>
    static int m_playerCount = 0;
    /// <summary>PlayerがGoalしたらtrueになる</summary>
    bool m_clearFlag = false;
    /// <summary>trueだったらPlayerは移動出来る</summary>
    bool m_moveFlag = true;
    /// <summary>セーブが完了したかどうか </summary>
    bool m_endSaveFlag = false;
    /// <summary>プレイヤーが揃ったかどうか </summary>
    bool m_startFlag = false;
    /// <summary>PlayerがGoalしたらtrueになる</summary>
    public bool ClearFlag { get => m_clearFlag; set => m_clearFlag = value; }
    /// <summary>trueだったらPlayerは移動出来る</summary>
    public bool MoveFlag { get => m_moveFlag; set => m_moveFlag = value; }
    /// <summary>セーブが完了したかどうか </summary>
    public bool EndSaveFlag { get => m_endSaveFlag; set => m_endSaveFlag = value; }
    public bool StartFlag { get => m_startFlag; set => m_startFlag = value; }
    /// <summary>ルームにいるプレイヤーの人数 </summary>
    public static int PlayerCount { get => m_playerCount; set => m_playerCount = value; }

    void Start()
    {
        m_clearText.gameObject.SetActive(false);
        m_timeManager = GetComponent<TimeManager>();
    }

    void Update()
    {
        if (m_clearFlag)
        {
            m_clearText.gameObject.SetActive(true);
            m_timeManager.EndFlag = true;

            if (m_endSaveFlag && PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SceneChange());
                m_endSaveFlag = false;
            }

            m_clearFlag = false;
        }
    }
  
    /// <summary>リザルトシーンに遷移するまでの待機時間を作る </summary>
    IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(3f);
        PhotonNetwork.LoadLevel("ResultScene");
    }
}
