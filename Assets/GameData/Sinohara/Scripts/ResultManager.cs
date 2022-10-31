using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using NCMB;
using System;
using System.Collections.Generic;

/// <summary>
/// リザルト画面に表示するオブジェクトやUI
/// の管理するスクリプト
/// </summary>
public class ResultManager : MonoBehaviour
{
    [Tooltip("ゴールタイムを表示するテキスト")]
    [SerializeField] Text m_goalTimeText = default;
    [Tooltip("優勝者の名前を表示するテキスト")]
    [SerializeField] Text m_championNameText = default;
    [Tooltip("優勝ハムスターのマテリアルを貼り付け")]
    [SerializeField] GameObject m_humster = default;

    /// <summary>優勝したプレイヤーの名前 </summary>
    static string m_championName = "";
    /// <summary>スタートからゴールまでの経過時間 </summary>
    float m_clearTime = 0f;

    static string m_face = "";

    static string m_body = "";
    /// <summary>優勝したハムスターの体マテリアル </summary>
    static Material m_championbody = default;
    /// <summary>優勝したハムスターの顔マテリアル </summary>
    static Material m_championFace = default;
    /// <summary>優勝したハムスターの体マテリアル </summary>
    public static Material Championbody { get => m_championbody; set => m_championbody = value; }
    /// <summary>優勝したハムスターの顔マテリアル </summary>
    public static Material ChampionFace { get => m_championFace; set => m_championFace = value; }
    /// <summary>優勝したプレイヤーの名前 </summary>
    public static string ChampionName { get => m_championName; set => m_championName = value; }
    public static string Face { get => m_face; set => m_face = value; }
    public static string Body { get => m_body; set => m_body = value; }

    private void Awake()
    {
        Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        m_clearTime = (float)properties["CT"];
    }

    void Start()
    {
        m_goalTimeText.text = m_clearTime.ToString("F2");
        m_championNameText.text = m_championName;
        GameObject mesh = m_humster.transform.GetChild(1).gameObject;
        mesh.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = Resources.Load(m_face, typeof(Material)) as Material; ;
        mesh.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material = Resources.Load(m_body, typeof(Material)) as Material;
        m_humster.transform.localScale = new Vector3(3, 3, 3); 
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Time" + m_clearTime);

    }

    /// <summary>ルームから退出</summary>
    public void LeaveRoom()
    {
        StartCoroutine(WaitForDisconnect());
    }

    /// <summary> ルームを退出したらサーバーから切断しロビーシーンに戻る</summary>
    private  IEnumerator WaitForDisconnect()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        Debug.Log(PhotonNetwork.IsConnected);
        while (PhotonNetwork.IsConnected)
            yield return null;
        Save();
        SceneManager.LoadScene("N.LobbyScene");
    }
    public void Save()
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
                        obj["Time"] = m_clearTime;
                        obj.SaveAsync();
                        break;
                    }
                }
            }
        });
    }
}
