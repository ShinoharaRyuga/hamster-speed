using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// アクションゲージを出すスクリプト
/// 木登り以外のギミックの成否を管理
/// </summary>
public class NotesManager : MonoBehaviourPunCallbacks
{
    [SerializeField, Header("ギミックを設定")] Gimmick m_useGimmick = Gimmick.river;
    [Tooltip("値が大きければ速く流れて来ます")]
    [SerializeField, Range(5f, 10f), Header("ノーツが流れてくる最高速度")] float m_notesMaxSpeed = 0.01f;
    [Tooltip(" 値が大きければ速く流れて来ます")]
    [SerializeField, Range(5f,10f), Header("ノーツが流れてくる最低速度")] float m_notesMinSpeed = 0.01f;
    [Tooltip("値が大きければ大きいです")]
    [SerializeField, Range(1, 10), Header("ノーツの大きさ")] float m_notesSize = 1;
    [Tooltip("全て流れ終わったら川を渡れる")]
    [SerializeField, Header("流れてくるノーツの総数")] int m_totalNotes = 0;
    [SerializeField] GameObject m_notes = default;
    [Tooltip("ギミックをクリアした時に移動している場所 川の向こう岸です")]
    [SerializeField, Header("川ギミックの成功時のプレイヤーの位置")] Transform m_goalPosition = default;
    [SerializeField, Header("カラスギミック成功の加速が続く効果時間")] float m_effectTime = 5f;
    [SerializeField, Header("カラスギミックが成功した時の場所")] Transform m_crowpos;
    [SerializeField, Header("蜘蛛の巣ギミック成功時の位置")] Transform m_spiderPos = default;
    [SerializeField] float m_textTime = 0.5f;
    [SerializeField] GameObject m_fadeUI = default;
    [SerializeField, Header("蜘蛛の巣ギミック失敗時に吹き飛ばす力")] float m_spiderPower = 40f;
    [Tooltip("カラスギミックを失敗した時の位置")]
    [SerializeField] Transform m_crowStartPos = default;
    /// <summary>ノーツの生成場所</summary>
    Transform m_generationPos = default;
    RiverGimmick m_riverGimmick = default;
    GameObject m_player = default;
    PlayerController m_controller = default;
    GameObject m_playerUI = default;
    SphereCollider m_sc;
    Text m_buttonText = default;
    Text m_judgementText = default;
    Canvas m_canvas = default;
    [SerializeField] GameObject m_actionUI = default;

    private bool m_gimmickStartFlag = false;

    private bool m_clearFlag = false;

    private bool m_onlyFlag = false;

    private bool m_setGravity = false;
    private float m_currentPosY = 0f;
    
    /// <summary>現在流れて終わったノーツの数</summary>
    private static int m_currentNotesNumber = 0;
    
    /// <summary>playerが失敗したかどうか</summary>
    private static bool m_failFlag = false;
    
    /// <summary>次のノーツを作成するフラグ</summary>
    private static bool m_nextNotes = true;
    
    /// <summary>スクリプトアクセス用の変数 ノーツが流れるスピード</summary>
    private static float m_speed = 0;
    
    /// <summary>スクリプトアクセス用の変数 ノーツが流れるスピード</summary>
    public static float Speed { get => m_speed; set => m_speed = value; }
    
    /// <summary>次のノーツを作成するフラグ</summary>
    public static bool NextNotes { get => m_nextNotes; set => m_nextNotes = value; }
    
    /// <summary>playerが失敗したかどうか</summary>
    public static bool FailFlag { get => m_failFlag; set => m_failFlag = value; }
    
    /// <summary>現在流れて終わったノーツの数</summary>
    public static int CurrentNotesNumber { get => m_currentNotesNumber; set => m_currentNotesNumber = value; }

    void Update()
    {
        if (m_gimmickStartFlag == true)
        {
            if (m_setGravity == false && m_useGimmick == Gimmick.river)
            {
                m_controller.SetGravity(false);
                m_setGravity = true;
            }
            
            if (m_nextNotes && m_currentNotesNumber < m_totalNotes && !m_failFlag)
            {
                NotesGeneration();
                m_nextNotes = false;

                if (m_useGimmick != Gimmick.spider) //ハムスターを移動させる
                {
                    GimmickMove();
                }

                if (m_currentNotesNumber != 0)
                {
                    StartCoroutine(judgment());
                }
            }
            else if (m_failFlag && !m_onlyFlag) //ギミックを失敗
            {
                m_judgementText.text = "失敗!!";
                Fail();
            }

            if (m_currentNotesNumber == m_totalNotes && !m_clearFlag) //ギミックを成功
            {
                m_judgementText.text = "成功!!";
                SuccessGimmick();
                SetPlayerControl();
                ResetGimmick();
            }
        }
    }

    /// <summary>ノーツの作成 </summary>
    public void NotesGeneration() 
    {
        GameObject notes = Instantiate(m_notes, m_generationPos.position, Quaternion.identity);
        notes.transform.parent = m_canvas.transform;
        notes.transform.SetSiblingIndex(1);
        Vector3 notesSize = notes.transform.localScale;
        if (m_useGimmick != Gimmick.spider)
            SelectButton();
        else
            SpiderGimmick();
        m_notesSize = Random.Range(0.8f, m_notesSize);
        m_speed = Random.Range(m_notesMinSpeed, m_notesMaxSpeed);
        m_speed = -m_speed;
        notesSize.x = m_notesSize;
        notes.transform.localScale = notesSize;
    }

    /// <summary>押させるボタンを決める </summary>
    private void SelectButton()
    {
        int number = Random.Range(0, 5);
        switch (number)
        {
            case 0:
                m_buttonText.text = "W";
                m_riverGimmick.ButtonName = "W";
                break;
            case 1:
                m_buttonText.text = "A";
                m_riverGimmick.ButtonName = "A";
                break;
            case 2:
                m_buttonText.text = "S";
                m_riverGimmick.ButtonName = "S";
                break;
            case 3:
                m_buttonText.text = "D";
                m_riverGimmick.ButtonName = "D";
                break;
            case 4:
                m_buttonText.text = "Space";
                m_riverGimmick.ButtonName = "Space";
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && other.GetComponent<PhotonView>().IsMine)
        {
            m_player = other.gameObject;
            if(m_useGimmick != Gimmick.spider)
                m_player.transform.LookAt(m_goalPosition);
            m_controller = other.GetComponent<PlayerController>();
            m_controller.StopMove();
            m_controller.CurrentState = State.action;
            m_playerUI = Instantiate(m_actionUI);
            m_playerUI.transform.parent = m_player.transform;
            m_canvas = m_playerUI.GetComponent<Canvas>();
            m_generationPos = m_playerUI.transform.GetChild(2).gameObject.GetComponent<Transform>();
            m_buttonText = m_playerUI.transform.GetChild(1).gameObject.GetComponent<Text>();
            m_riverGimmick = m_playerUI.transform.GetChild(4).gameObject.GetComponent<RiverGimmick>();
            m_judgementText = m_playerUI.transform.GetChild(5).gameObject.GetComponent<Text>();
            m_buttonText.text = "";
            m_judgementText.text = " ";
            m_gimmickStartFlag = true;
            m_controller.ChangeCamera(3);
            Vector3 startPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            m_player.transform.position = startPos;
            if (m_useGimmick == Gimmick.river)
            {
                m_sc = m_player.GetComponent<SphereCollider>();
                m_sc.isTrigger = true;
            }
        }
    }

    /// <summary>
    /// ギミックを再度使えるようにする
    /// </summary>
    private void ResetGimmick()
    {
        m_gimmickStartFlag = false;
        m_failFlag = false;
        m_nextNotes = true;
        m_clearFlag = false;
        m_onlyFlag = false;
        m_currentNotesNumber = 0;
        m_setGravity = false;
    }

    /// <summary>成否判定のテキストを消す</summary>
    /// <returns></returns>
    IEnumerator judgment()
    {
        m_judgementText.text = "ヒット！！";
        yield return new WaitForSeconds(m_textTime);
        m_judgementText.text = " ";
    }

    /// <summary>
    /// ギミック失敗時に起こす動作を設定
    /// </summary>
    private void Fail()
    {
        m_controller.SetGravity(true);

        switch (m_useGimmick)
        {
            case Gimmick.crow:
                //GameObject fade = Instantiate(m_fadeUI);
                //fade.transform.parent = m_player.transform;
                m_player.transform.position = m_crowStartPos.position;
                break;
            case Gimmick.spider:
                Rigidbody rb = m_player.GetComponent<Rigidbody>();
                rb.AddForce(-m_player.transform.forward * m_spiderPower, ForceMode.Impulse);
                break;
        }

        SetPlayerControl();
        StartCoroutine(LateReset());
    }

    /// <summary>プレイヤーの入力を受け付けてハムスターを移動させる</summary>
    private void SetPlayerControl()
    {
        Destroy(m_playerUI);
        m_controller.SetGravity(true);
        m_controller.CurrentState = State.normal;
        m_clearFlag = true;
        m_controller.ChangeCamera(m_controller.CurrentCameraIndex);
    }

    /// <summary>
    /// ギミックに失敗時プレイヤーの位置を戻す
    /// </summary>
    /// <returns></returns>
    IEnumerator SetPlayerPosition()
    {
        yield return new WaitForSeconds(2.5f);
        if(m_useGimmick == Gimmick.river)
            m_player.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 10f);
        else
            //スタート地点に移動
        SetPlayerControl();
        ResetGimmick();
    }

    /// <summary>蜘蛛の巣ギミック挑戦時アクションボタンをSpaceボタンに固定する</summary>
    public void SpiderGimmick()
    {
        m_buttonText.text = "Space";
        m_riverGimmick.ButtonName = "Space";
    }

    /// <summary>川・カラスギミック時にハムスターを移動させる</summary>
    private void GimmickMove()
    {
        m_player.transform.Translate(Vector3.forward * Vector3.Distance(this.transform.position, m_goalPosition.position) / (m_totalNotes + 1));
    }

    /// <summary>ギミック成功時を設定する</summary>
    private void SuccessGimmick()
    {
        switch (m_useGimmick)
        {
            case Gimmick.river:
                m_player.transform.position = m_goalPosition.position;
                m_sc.isTrigger = false;
                break;
            case Gimmick.crow:
                m_player.transform.position = m_crowpos.position;
                PlayerController controller = m_player.GetComponent<PlayerController>();
                StartCoroutine(controller.Dash(m_effectTime));
                break;
            case Gimmick.spider:
                m_player.transform.position = m_spiderPos.position;
                Destroy(this.gameObject);
                break;
        }
    }

    public void FailRiver()
    {
        if (m_player.GetComponent<PhotonView>().IsMine)
        {
            GameObject go = Instantiate(m_fadeUI);
            go.transform.parent = m_player.transform;
            StartCoroutine(SetPlayerPosition());
            m_onlyFlag = true;
            m_sc.isTrigger = false;
        }
    }

    /// <summary>
    /// 遅延をしてからギミックを再度使用可能にする
    /// </summary>
    /// <returns></returns>
    private IEnumerator LateReset()
    {
        yield return new WaitForSeconds(0.5f);
        ResetGimmick();
    }
}

public enum Gimmick
{
    /// <summary>川ギミック</summary>
    river,
    /// <summary>カラスギミック</summary>
    crow,
    /// <summary>蜘蛛の巣ギミック</summary>
    spider
}
