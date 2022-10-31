using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using NCMB;

/// <summary>
/// ロビーの機能のスクリプト
///　UIの変更やハムスターのスキン変更など
/// </summary>
public class LobbyManager : MonoBehaviour
{
    [Tooltip("選択されている間のボタン色")]
    [SerializeField] Color m_selectedColor = default;
    [Tooltip("UIを変えるボタン")]
    [SerializeField] Button[] m_tabButton = default;
    [Tooltip("表示するUI")]
    [SerializeField] GameObject[] m_UI = default;
    [Tooltip("playUIの時に表示するハムスターの位置")]
    [SerializeField] Transform m_playHamSpawner = default;
    [Tooltip("characterUIの時に表示するハムスターの位置")]
    [SerializeField] Transform m_characterHamSpawner = default;
    [Tooltip("エンドクレジット")]
    [SerializeField] GameObject m_endCreditUI = default;
    [Tooltip("ヘルプ画像を表示するオブジェクト")]
    [SerializeField] Image m_imageObj = default;
    [Tooltip("0から順に表示する")]
    [SerializeField] Sprite[] m_helpImages = default;
    [Tooltip("ヘルプ画像を切り替える時に使う右ボタン")]
    [SerializeField] GameObject m_rightBtn = default;
    [Tooltip("ヘルプ画像を切り替える時に使う左ボタン")]
    [SerializeField] GameObject m_leftBtn = default;
    [Tooltip("チュートリアルを非表示にするボタン")]
    [SerializeField] GameObject m_BackButton = default;
    [Tooltip("何ページ目のチュートリアルを表示している示す画像")]
    [SerializeField] Image[] m_currentPositions = default;
    [Tooltip("戦績UIで自分が使用しているハムスターの画像になるもの")]
    [SerializeField] Image m_recordImage = default;
    [Tooltip("このハムスターのマテリアルを変更していく")]
    [SerializeField] GameObject m_humster = default;

    /// <summary>Playerが使用するハム</summary>
    GameObject m_selectedHam = default;
    /// <summary>ハムのマテリアルが入っているメッシュの親オブジェクト</summary>
    GameObject m_mesh = default;

    ConfigurationManager m_configurationManager = default;
    /// <summary>プレイヤーが使用しているハムスターの画像 </summary>
    static Sprite m_selectedHumImage = default;
    /// <summary>エンドクレジットの表示か非表示にするかを決めるフラグ true=表示　false=非表示<summary>
    bool m_endCreditFlag = false;
    /// <summary>ヘルプ画像を表示しているかどうか　true=表示　false=非表示</summary>
    bool m_helpFlag = false;
    /// <summary>現在表示しているヘルプ画像の添え字</summary>
    int m_currentImageIndex = 0;
    /// <summary>キャラクターマテリアル</summary>
    string face, hum;

    int m_uiIndex = 0;

    void Start()
    {
        SetUI(0);
        m_configurationManager = GetComponent<ConfigurationManager>();
        m_selectedHam = Instantiate(m_humster, m_playHamSpawner.position, Quaternion.Euler(0f, 180f, 0f));  //ハムスターを生成
        m_mesh = m_selectedHam.transform.GetChild(1).gameObject;
        face = PlayerDataTest.FaceMaterialName;
        hum = PlayerDataTest.BodyMaterialName;
        m_mesh.transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load(PlayerDataTest.FaceMaterialName, typeof(Material)) as Material;
        m_mesh.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load(PlayerDataTest.BodyMaterialName, typeof(Material)) as Material;
        m_selectedHam.transform.localScale = new Vector3(6f, 6f, 6f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (m_uiIndex != 4)
        {
            m_configurationManager.EndCredit.SetActive(false);
        }
    }

    /// <summary>
    /// タブボタンを押した時に表示するUIを変える関数 (モデルを表示するしたり名前を変数したりもする)
    /// ボタンで呼び出し
    /// </summary>
    /// <param name="number">UIの番号</param>
    public void ChangeUI(int number)
    {
        m_uiIndex = number;

        switch (number)
        {
            case 1: //PlayUIを表示している時
                SetUI(0);
                m_selectedHam.transform.position = m_playHamSpawner.position;
                m_selectedHam.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                break;
            case 2:  //キャラクター選択UIを表示している時
                SetUI(1);
                m_selectedHam.transform.position = m_characterHamSpawner.position;
                m_selectedHam.transform.rotation = Quaternion.Euler(0f, -150f, 0f);
                m_selectedHam.transform.localScale = new Vector3(6f, 6f, 6f);
                break;
            case 3: //戦績UIを表示している時
                SetUI(2);
                break;
            case 4: //設定UIを表示している時
                SetUI(3);
                break;
        }
    }

    /// <summary>番号に応じてUIを変える</summary>
    /// <param name="set"></param>
    private void SetUI(int set)
    {
        for (var i = 0; i < m_tabButton.Length; i++)
        {
            if (set == i)
            {
                //ボタンの色を変える
                ColorBlock selectBtn = m_tabButton[i].colors;
                selectBtn.normalColor = m_selectedColor;
                selectBtn.pressedColor = m_selectedColor;
                selectBtn.highlightedColor = m_selectedColor;
                m_tabButton[i].colors = selectBtn;
                //UIを表示
                m_UI[i].SetActive(true);
            }
            else
            {
                //ボタンの色を白にする
                ColorBlock selectBtn = m_tabButton[i].colors;
                selectBtn.normalColor = Color.white;
                selectBtn.pressedColor = Color.white;
                selectBtn.highlightedColor = Color.white;
                m_tabButton[i].colors = selectBtn;
                //UIを非表示する
                m_UI[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// ハムスターのスキンを変更する
    /// ボタンで呼び出し　🐹🐹🐹
    /// </summary>
    /// <param name="humster">使いたいハムスター(スキン)</param>
    public void ChangeUseHam(GameObject humster)
    {
        GameObject mesh = humster.transform.GetChild(1).gameObject;
        m_selectedHam.transform.localScale = new Vector3(6f, 6f, 6f);
        hum = mesh.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().sharedMaterial.name;
        face = mesh.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().sharedMaterial.name;
        PlayerDataTest.FaceMaterialName = face;
        PlayerDataTest.BodyMaterialName = hum;
        m_mesh.transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load(face, typeof(Material)) as Material;
        m_mesh.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load(hum, typeof(Material)) as Material;
    }

    /// <summary>エンドクレジットを表示・非表示にする</summary>
    public void SetEndCreditUI()
    {
        if (!m_endCreditFlag)
        {
            m_endCreditUI.SetActive(true);
            m_endCreditFlag = true;
        }
        else
        {
            m_endCreditUI.SetActive(false);
            m_endCreditFlag = false;
        }
    }

    /// <summary>ヘルプ画像を表示する</summary>
    public void SetHelpImage()
    {
        if (!m_helpFlag)    //表示する
        {
            m_imageObj.gameObject.SetActive(true);
            Array.ForEach(m_currentPositions, i => i.gameObject.SetActive(true));
            ChangeCurrentImage();
            m_rightBtn.SetActive(true);
            m_helpFlag = true;
            m_currentImageIndex = 0;
            m_imageObj.sprite = m_helpImages[0];
        }
        else    //非表示にする
        {
            m_imageObj.gameObject.SetActive(false);
            Array.ForEach(m_currentPositions, i => i.gameObject.SetActive(false));
            m_rightBtn.SetActive(false);
            m_leftBtn.SetActive(false);
            m_BackButton.SetActive(false);
            m_helpFlag = false;
            m_currentImageIndex = 0;
            m_imageObj.sprite = m_helpImages[0];
        }
    }

    /// <summary>
    /// 表示しているヘルプ画像切り替える
    /// 引数によって処理を変える
    /// </summary>
    /// <param name="buttonNumber">0=左ボタン　１=右ボタン</param>
    public void ChangeHelpImage(int buttonNumber)
    {
        if (buttonNumber == 1 && m_currentImageIndex < m_helpImages.Length)  //右の画像に切り替える
            m_currentImageIndex++;

        if (buttonNumber == 0 && m_currentImageIndex > 0)   //左の画像に切り替える
            m_currentImageIndex--;

        m_imageObj.sprite = m_helpImages[m_currentImageIndex];  //画像をセット

        if (m_currentImageIndex != 0)   //表示している画像が最初の画像だったら左に進めるボタンを消す
            m_leftBtn.SetActive(true);
        else
            m_leftBtn.SetActive(false);

        if (m_currentImageIndex != m_helpImages.Length - 1) 　//表示している画像が最後の画像だったら右に進めるボタンを消す
            m_rightBtn.SetActive(true);
        else
        {
            m_rightBtn.SetActive(false);
            m_BackButton.SetActive(true);
        }

        ChangeCurrentImage();
    }

    /// <summary>
    /// 表示中のページを示す画像の色を変える
    /// </summary>
    private void ChangeCurrentImage()
    {
        foreach (var image in m_currentPositions)
        {
            image.color = Color.white;
        }

        m_currentPositions[m_currentImageIndex].color = Color.yellow;
    }

    /// <summary>
    /// プレイヤーが選択したハムスターのマテリアル名を保存するし
    /// ゲームシーンに遷移する
    /// ボタンから呼び出し
    /// </summary>
    private void Save()
    {
        NCMBQuery<NCMBObject> test = new NCMBQuery<NCMBObject>("UserData");

        test.FindAsync((List<NCMBObject> list, NCMBException e) =>
        {
            if (e != null)
            {
                //検索失敗時の処理
                Debug.Log("失敗");
            }
            else
            {
                foreach (NCMBObject obj in list)
                {
                    string name = Convert.ToString(obj["Name"]);
                    if (PlayerDataTest.UserName == name)
                    {
                        obj["hummesh"] = hum;
                        obj["facemesh"] = face;
                        obj.SaveAsync();
                        break;
                    }
                }
            }
        });

        PUNManager.ChangeStage();
    }
}
