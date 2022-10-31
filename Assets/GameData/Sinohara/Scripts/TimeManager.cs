using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;


/// <summary>
/// カウントダウンとゲーム開始からの経過時間
/// を計測する。
/// </summary>
public class TimeManager : MonoBehaviour
{
    [Tooltip("カウントダウン時に使うイラスト　添え字 0=3秒時に出す 1=2秒時に出す 2=1秒時に出す 3=スタート")]
    [SerializeField] Sprite[] m_sprites;
    [Tooltip("扉のアニメーター")]
    [SerializeField] Animator[] m_anim = new Animator[4];
    [Tooltip("カウントダウンを表示するUI")]
    [SerializeField] Image m_countDownUI;
    [Tooltip("経過時間を表示させるテキスト")]
    [SerializeField] Text m_timeText = default;
    [Tooltip("待機中に表示するテキスト")]
    [SerializeField] GameObject m_waitText = default;
    [Tooltip("開始前のカウントダウンの秒数")]
    [SerializeField] float m_countDownTime = 4;
    [Tooltip("プレイヤーが揃わなかった時の時間")]
    [SerializeField] float m_startTime = 60;

    GameManager m_gameManager = default;
    /// <summary>ゲームが開始してからの経過時間 </summary>
    float m_elapsedTime = 0;

  　float m_clearTime = 0;
    /// <summary>カウントダウンをしているかどうか  false=ゲーム開始</summary>
    bool m_countDownFlag = false;
    /// <summary>ゴールタイムを記録する為の変数 </summary>
    bool m_endFlag = false;
    /// <summary>プレイヤーが集まったかどうか true=4人集まった</summary>
    bool m_isPlayer = false;
    /// <summary>タイマーを一度だけスタートさせる為の変数</summary>
    bool m_timestartFlag = false;
    /// <summary>プレイヤーがゴールに到着したらタイム計測をやめる</summary>
    bool m_timeFlag = true;

    int m_timestamp = 0;

    public bool EndFlag { get => m_endFlag; set => m_endFlag = value; }
    public bool IsPlayer { get => m_isPlayer; set => m_isPlayer = value; }


    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 4 && !m_timestartFlag)
            {
                PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
                PhotonNetwork.CurrentRoom.IsOpen = false;
                m_isPlayer = true;
                m_timestartFlag = true;
                m_countDownFlag = true;
            }
        }

        if (PhotonNetwork.InRoom)
        {
            if (!PhotonNetwork.CurrentRoom.TryGetStartTime(out int timestamp)) { return; }  // まだゲームの開始時刻が設定されていない場合は更新しない
            m_timestamp = timestamp;


            if (!m_gameManager.ClearFlag && m_timeFlag)
            {
                m_elapsedTime = Mathf.Max(0f, unchecked(PhotonNetwork.ServerTimestamp - m_timestamp) / 1000f);
            }

            if (!m_isPlayer)
            {
                m_elapsedTime = m_startTime - m_elapsedTime;
            }

            if (!m_isPlayer && m_elapsedTime <= 0)
            {
                PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
                PhotonNetwork.CurrentRoom.IsOpen = false;
                m_isPlayer = true;
                m_timeText.gameObject.SetActive(false);
                StartCoroutine(WaitCountDown());
            }

            if (m_countDownFlag)
            {
                m_elapsedTime = m_countDownTime - m_elapsedTime;
                CountDown();
            }

            if (m_elapsedTime <= 0)
            {
                m_gameManager.StartFlag = true;
            }

            if (m_endFlag)  //ゴールタイムを記録
            {
                m_clearTime = m_elapsedTime;
                // m_timeText.gameObject.SetActive(false);
                m_endFlag = false;
                m_timeFlag = false;
                SetClearTime();
            }

            m_timeText.text = m_elapsedTime.ToString("F2");
        }
    }

    /// <summary>
    ///  カウントダウンのUIを表示し
    ///  ゲームスタート時にプレイヤーの前にあるドアを開ける
    /// </summary>
    void CountDown()
    {
        if (m_elapsedTime <= 3f)
        {
            m_countDownUI.gameObject.SetActive(true);
            m_timeText.gameObject.SetActive(false);
            m_countDownUI.sprite = m_sprites[0];
        }

        if (m_elapsedTime <= 2f)
        {
            m_countDownUI.sprite = m_sprites[1];
        }

        if (m_elapsedTime <= 1f)
        {
            m_countDownUI.sprite = m_sprites[2];
        }

        if (m_elapsedTime <= 0f)    //開始
        {
            m_countDownUI.sprite = m_sprites[3];
            Array.ForEach(m_anim, a => a.Play("Open"));
            PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
            StartCoroutine(SetCountDownUI());
        }
    }

    /// <summary>
    /// ゲーム開始から1秒間スタートイラストを表示する
    /// </summary>
    /// <returns></returns>
    IEnumerator SetCountDownUI()
    {
        yield return new WaitForSeconds(1f);
        m_countDownUI.gameObject.SetActive(false);
        m_timeText.gameObject.SetActive(true);
        m_countDownFlag = false;
    }

    IEnumerator WaitCountDown()
    {
        yield return new WaitForSeconds(1f);
        m_countDownFlag = true;
        m_waitText.SetActive(false);
        m_timeText.gameObject.SetActive(true);
    }

    /// <summary> カスタムプロパティにゴールタイムを保存する　</summary>
    private void SetClearTime()
    {
        Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        if (properties["CT"] == null)
        {
            properties.Add("CT", m_clearTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
    }
}
