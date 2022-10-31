using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Playerの動きや所持アイテムの管理を
/// するスクリプト Playerに関してのスクリプト
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("前後の動き 値が大きいと前後移動の速度が上がる 最低は１です")]
    [SerializeField] float m_zSpeed = 0;
    [Tooltip("左右の動き 値が大きいと左右移動の速度が上がる 最低は１です")]
    [SerializeField] float m_xSpeed = 0;
    [Tooltip("値が大きいとジャンプ力が上がる")]
    [SerializeField] float m_jumpSpeed = 0;
    [Tooltip("値が大きいとダッシュ時の速度が上がる　最低は１です")]
    [SerializeField] float m_dashSpeed = 0;
    [Tooltip("スタミナの上限")]
    [SerializeField] int m_maxStamina = 0;
    [Tooltip("ダッシュする為に必要なスタミナ")]
    [SerializeField] int m_useDashStamina = 0;
    [Tooltip("時間経過で回復するスタミナの量")]
    [SerializeField] int m_staminaAmount = 0;
    [Tooltip("スタミナが回復するまでの時間 (一定間隔)")]
    [SerializeField] float m_staminaTime = 0f;
    [Tooltip("ダッシュをしてる時間")]
    [SerializeField] float m_dashTime = 0f;
    [Tooltip("スピードアップアイテムの効果時間")]
    [SerializeField] float m_speedUpEffectTime = 0f;
    [Tooltip("木登り中の登りスピード")]
    [SerializeField] float m_woodSpeed = 0f;
    [Tooltip("プレイヤー視点のカメラ")]
    [SerializeField] GameObject m_playerCamera = default;
    [Tooltip("各視点のカメラ")]
    [SerializeField] CinemachineVirtualCameraBase[] m_cameras = default;
    [SerializeField] GameObject m_playerUI = default;
    [SerializeField] GameObject m_mapCamera = default;
    [SerializeField] GameObject m_mapdir = default;
    /// <summary>スタミナを表示するテキスト</summary>
    Text m_staminaText = default;
    Image m_aimImage = default;
    /// <summary>最初の前後移動の速度</summary>
    float m_zfirstSpeed = 0f;
    /// <summary>最初の左右移動の速度</summary>
    float m_xfirstSpeed = 0f;
    /// <summary>時間経過</summary>
    float m_time = 0f;
    /// <summary>スピードアップアイテムを使用してから経過時間</summary>
    float m_speedUpElapsedTime = 0f;
    /// <summary>現在のスタミナ量</summary>
    int m_currentStamina = 20;
    /// <summary>接地判定</summary>
    bool m_isGround = true;
    /// <summary>ダッシュをしたかどうか</summary>
    bool m_dashFlag = false;
    /// <summary>スピードアップアイテムを使用したかどうか</summary>
    bool m_useSpeedUpFlag = false;
    /// <summary>アイテムを所持しているかどうか true = 未所持　false = 所持</summary>
    bool m_checkResult = true;
    /// <summary>フックを発射出来るかどうか</summary>
    bool m_shootHookFlag = false;
    /// <summary>trueだったらジャンプ出来る　woodギミック用</summary>
    bool m_jumpFlag = true;
    /// <summary>水平方向の入力</summary>
    float h = 0;
    /// <summary>垂直方向の入力</summary>
    float v = 0;
    /// <summary>マウスの水平方向入力</summary>
    float m_mouseX = 0;
    /// <summary>マウスの垂直方向入力</summary>
    float m_mosueY = 0;
    /// <summary>入力された方向の XZ 平面でのベクトル</summary>
    Vector3 m_dir;
    Vector3 m_woodDir = default;
    /// <summary>プレイヤーの状態が入っている</summary>
    State m_currentState = State.normal;

    /// <summary>所持アイテムのリスト</summary>
    List<ItemBase> m_itemList = new List<ItemBase>();

    Rigidbody m_rb = default;
    Animator m_anim = default;
    GameManager m_managerScript = default;
    AudioSource m_audioSource = default;
    PhotonView photonView;
    WoodGimmick m_woodGimmick;
    ItemDisplay m_itemDisplay = default;
    /// <summary>ルーム退出のUI </summary>
    GameObject m_leaveUI = default;
    /// <summary>木ギミックで登る木</summary>
    Transform m_wood;
    /// <summary>0=ジャンプSE 1=足音 2=ダッシュ</summary>
    [SerializeField] AudioClip[] m_audioClips = default;
    /// <summary>現在の使っているカメラの添え字</summary>
    int m_currentCameraIndex = 1;
    /// <summary>アイテムを所持しているかどうか true = 未所持　false = 所持</summary>
    public bool CheckResult { get => m_checkResult; }
    /// <summary>trueだったらジャンプ出来る　woodギミック用</summary>
    public bool JumpFlag { get => m_jumpFlag; set => m_jumpFlag = value; }
    /// <summary>現在のスタミナ量</summary>
    public int CurrentStamina { get => m_currentStamina; set => m_currentStamina = value; }
    public int MaxStamina { get => m_maxStamina; set => m_maxStamina = value; }
    /// <summary>フックを発射出来るかどうか</summary>
    public bool ShootHookFlag { get => m_shootHookFlag; set => m_shootHookFlag = value; }
    /// <summary>プレイヤーの状態が入っている</summary>
    public State CurrentState { get => m_currentState; set => m_currentState = value; }
    public Image AimImage { get => m_aimImage; }
    public int CurrentCameraIndex { get => m_currentCameraIndex; }

    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_anim = GetComponent<Animator>();
        m_audioSource = this.gameObject.AddComponent<AudioSource>();
        photonView = GetComponent<PhotonView>();
        m_audioSource.volume = ConfigurationManager.SeVolume;
        m_managerScript = FindObjectOfType<GameManager>();
        m_anim.SetBool("MoveFlag", false);
        m_anim.SetBool("JumpFlag", false);
        m_currentStamina = m_maxStamina;
        //var pov = m_camera[0].GetCinemachineComponent(CinemachineCore.Stage.Aim).GetComponent<CinemachinePOV>();
        // pov.m_HorizontalAxis.m_MaxSpeed = ConfigurationManager.SensitivityValue;
        // pov.m_VerticalAxis.m_MaxSpeed = ConfigurationManager.SensitivityValue;
        Cursor.lockState = CursorLockMode.Locked;
        m_xfirstSpeed = m_xSpeed;
        m_zfirstSpeed = m_zSpeed;
        m_itemDisplay = GetComponent<ItemDisplay>();
   
        if (photonView.IsMine)
        {
            Instantiate(m_mapCamera, this.transform);
            Instantiate(m_mapdir, this.transform);
            GameObject go = Instantiate(m_playerUI, this.transform);
            m_staminaText = go.transform.GetChild(1).gameObject.GetComponent<Text>();
            m_aimImage = go.transform.GetChild(3).gameObject.GetComponent<Image>();
            m_itemDisplay.ItemDisPlay = go.transform.GetChild(2).GetComponent<Image>();
            PhotonNetwork.CurrentRoom.IsOpen = true;    //ロードが終了したのでルーム参加を許可する

            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        }
        else
        {
            Destroy(m_playerCamera);
            Array.ForEach(m_cameras, c => Destroy(c));
        }

        this.gameObject.name = photonView.Owner.NickName;
    }

    void Update()
    {
        if (photonView.IsMine) //Updateに書かれていたものをif文の中に入れました。
        {
            //移動に対しての入力
            if (m_currentState == State.normal || m_currentState == State.wood)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");     
            }
           
            if (m_currentState != State.wood && m_currentState != State.action)
            {
                //マウスの移動量
                m_mouseX = Input.GetAxis("Mouse X");
                m_mosueY = Input.GetAxis("Mouse Y");
            }
           
            //接地判定関数の結果を代入
            m_isGround = IsGround();
            //アイテムを所持しているかどうかの結果を代入
            m_checkResult = CheckItem();
            // カメラの切り替えの入力の受け付ける
            if (Input.GetButtonDown("Fire3"))
            {
                m_currentCameraIndex++;
                ChangeCamera(m_currentCameraIndex % m_cameras.Length);
            }

            if (m_managerScript.MoveFlag == true && m_currentCameraIndex != 0)
                transform.Rotate(0, m_mouseX, 0); //Playerの回転
            
            if (h != 0 || v != 0 && m_managerScript.MoveFlag == true) //アニメーションの再生
            {
                m_anim.SetBool("MoveFlag", true);
            }
            else
            {
                m_anim.SetBool("MoveFlag", false);
                m_anim.SetBool("JumpFlag", false);
            }

            if (m_currentState == State.wood)//木登り時の移動方法
            {
                transform.RotateAround(m_wood.position, Vector3.up, -h);
                m_woodDir = new Vector3(0, v * m_woodSpeed, 0);
                Vector3 vector3 = m_wood.position - this.transform.position;
                vector3.y = 0f;
                Quaternion quaternion = Quaternion.LookRotation(vector3);
                this.transform.rotation = quaternion;
                SetGravity(false);
            }

            if (Input.GetButtonDown("Jump") && m_currentState == State.wood)
            {
                m_jumpFlag = true;
                m_currentState = State.normal;
                m_woodGimmick.UseStaminaFlag = false;
                SetGravity(true);
                
            }

            //ジャンプ判定
            if (Input.GetButtonDown("Jump") && m_isGround && m_jumpFlag && m_currentState != State.action)
            {
                m_rb.AddForce(transform.up * m_jumpSpeed, ForceMode.Impulse);
                m_anim.Play("Jump");
                m_audioSource.PlayOneShot(m_audioClips[0]);
                m_isGround = false;
            }

            //ダッシュをする判定
            if (Input.GetButtonDown("Dash") && m_currentStamina >= m_useDashStamina && !m_dashFlag)
            {
                StartCoroutine(Dash(m_dashTime));
                m_audioSource.PlayOneShot(m_audioClips[2]);
            }

            //アイテムを使う
            if (Input.GetButtonDown("Item"))
            {
                if (m_itemList.Count > 0)
                {
                    ItemBase item = m_itemList[0];
                    item.Use();
                    m_itemList.RemoveAt(0);
                    Destroy(item.gameObject);
                }
            }

            //経過時間によってスタミナが徐々に回復する
            m_time += Time.deltaTime; //経過時間を測る
            if (m_time >= m_staminaTime)
            {
                m_currentStamina = Math.Min(m_currentStamina + m_staminaAmount, m_maxStamina); //スタミナ追加
                m_time = 0f;
            }

            //スタミナをテキストに表示
            m_staminaText.text = m_currentStamina.ToString() + "/" + m_maxStamina.ToString();
            //スピードアップアイテムを使用してからの経過時間を計測
            if (m_useSpeedUpFlag)
                m_speedUpElapsedTime += Time.deltaTime;
            

            //スピードアップアイテムを使用してからの経過時間が効果時間を超えたら元に戻る
            if (m_speedUpElapsedTime >= m_speedUpEffectTime)
            {
                m_useSpeedUpFlag = false;
                m_zSpeed = m_zfirstSpeed;
                m_xSpeed = m_xfirstSpeed;
                m_speedUpElapsedTime = 0f;
            }

            if (m_currentCameraIndex == 0) //一人称時の移動方法
            {
                m_dir = Vector3.forward * v + Vector3.right * h;
                // カメラのローカル座標系を基準に dir を変換する
                m_dir = Camera.main.transform.TransformDirection(m_dir);
                // カメラは斜め下に向いているので、Y 軸の値を 0 にして「XZ 平面上のベクトル」にする
                m_dir.y = 0;
                // キャラクターを「現在の（XZ 平面上の）進行方向」に向ける
                Vector3 forward = m_rb.velocity;
                forward.y = 0;

                if (forward != Vector3.zero)
                {
                    this.transform.forward = forward;
                }

                Vector3 playerDir = Camera.main.transform.forward;
                playerDir.y = 0;
                this.transform.forward = playerDir;

                Debug.Log("一人称視点");
            }
        }
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (m_managerScript.MoveFlag && m_currentState != State.action && m_currentState != State.stan)
            {
                if (m_currentState != State.wood) //木登りをしていない時の動き
                {
                    if (!m_dashFlag) //ダッシュをしていない時
                    {
                        if (m_currentCameraIndex == 0)//一人称時
                        {
                            m_rb.AddForce(m_dir.normalized * m_zSpeed, ForceMode.Force);
                        }
                        else
                        {
                            m_rb.AddForce(transform.forward * (v * m_zSpeed), ForceMode.Force); //前後移動

                        }
                    }
                    else if (m_dashFlag) //ダッシュをしている時
                    {
                        if (m_currentCameraIndex == 0)//一人称時
                        {
                            m_rb.AddForce(m_dir.normalized * m_zSpeed * m_dashSpeed, ForceMode.Force);
                        }
                        else
                        {
                            m_rb.AddForce(transform.forward * (v * m_zSpeed) * m_dashSpeed, ForceMode.Force); //前後移動

                        }
                    }

                    if (m_currentCameraIndex != 0)
                    {
                        m_rb.AddForce(transform.right * (h * m_xSpeed), ForceMode.Force); //左右移動
                    }
                }
                else if (m_currentState == State.wood) //木登り時
                {
                    m_rb.useGravity = false;
                    m_rb.velocity = m_woodDir;
                }

            }
        }
    }

    /// <summary>接地判定をRayを使った bool型で結果を返す</summary>
    /// <returns></returns>
    bool IsGround()
    {
        var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down);

        return Physics.Raycast(ray, 0.3f);
    }

    /// <summary>ダッシュしている時間を測る関数 一定の時間を過ぎたら普通の速度に戻る</summary>
    /// <returns></returns>
    public IEnumerator Dash(float dashTime)
    {
        m_dashFlag = true;
        m_anim.SetBool("MoveFlag", false);
        m_anim.SetBool("DashFlag", true);
      //  if(m_currentState != State.action)
            m_currentStamina -= m_useDashStamina;
        yield return new WaitForSeconds(dashTime);
        m_dashFlag = false;
        m_anim.SetBool("DashFlag", false);
        m_anim.SetBool("MoveFlag", true);
    }

    /// <summary>
    /// アイテムを取得する
    /// </summary>
    /// <param name="item"></param>
    public void GetItem(ItemBase item)
    {
        m_itemList.Add(item);
    }

    /// <summary>SpeedUpアイテムを取得した時に呼ばれる</summary>
    /// <param name="speed"> アイテム終了後のPlayerの移動速度</param>
    public void SpeedUp(float speed)
    {
        m_useSpeedUpFlag = true;
        m_zSpeed = speed;
        m_xSpeed = speed;
    }

    /// <summary>Playerがアイテムを所持しているか調べる関数</summary>
    public bool CheckItem()
    {
        if (m_itemList.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>カメラの切り替えをする関数</summary>
    /// <param name="currentmode">現在のカメラのモード</param>
    public void ChangeCamera(int mode)
    {
        switch (mode)
        {
            case 0:
                m_cameras[0].Priority = 20;
                m_cameras[1].Priority = 10;
                m_cameras[2].Priority = 10;
                break;
            case 1:
                m_cameras[0].Priority = 10;
                m_cameras[1].Priority = 20;
                m_cameras[2].Priority = 10;
                break;
            case 2:
                m_cameras[0].Priority = 10;
                m_cameras[1].Priority = 10;
                m_cameras[2].Priority = 20;
                break;
        }
    }

    /// <summary>true=重力あり false=重力なし</summary>
    /// <param name="flag"></param>
    public void SetGravity(bool flag)
    {
        if (flag)
        {
            m_rb.useGravity = true;
        }
        else
        {
            m_rb.useGravity = false;
            StopMove();
        }
    }

    /// <summary>
    /// 木登りギミックの設定をする関数
    /// </summary>
    /// <param name="woodGimmick"></param>
    /// <param name="wood"></param>
    public void SetWoodManager(WoodGimmick woodGimmick, Transform wood)
    {
        m_woodGimmick = woodGimmick;
        m_wood = wood;
    }
    
    /// <summary>
    /// スタンを状態にして経過時間で解除する
    /// </summary>
    /// <param name="stanTime">解除するまでの時間</param>
    /// <returns></returns>
    public IEnumerator ReleaseStan(float stanTime)
    {
        m_rb.velocity = Vector3.zero;
        h = 0;
        v = 0;
        yield return new WaitForSeconds(stanTime);
        m_currentState = State.normal;
    }

    /// <summary>プレイヤーの動きを止める</summary>
     public void StopMove()
    {
        m_rb.velocity = Vector3.zero;
        h = 0;
        v = 0;
        m_mosueY = 0;
        m_mouseX = 0;
    }

    /// <summary>
    /// 勝利したハムスターのマテリアル名を
    /// ルームにいる他のプレイヤーに伝える
    /// </summary>
    /// <param name="face">顔マテリアル名</param>
    /// <param name="body">体マテリアル名</param>
    [PunRPC]
    void SetChampionMaterial(string face, string body)
    {
        ResultManager.Body = body.Substring(0, 8);
        ResultManager.Face = face.Substring(0, 6);
    }
}

/// <summary>プレイヤーの状態</summary>
public enum State
{
    /// <summary>通常時　ギミックに挑戦していない時　走り回る</summary>
    normal,
    /// <summary>木ギミックに挑戦している時</summary>
    wood,
    /// <summary>行動不能</summary>
    stan,
    /// <summary>アクションゲージを使うギミックに挑戦時</summary>
    action,
    /// <summary>人ギミックに飛ばされた </summary>
    human
}

