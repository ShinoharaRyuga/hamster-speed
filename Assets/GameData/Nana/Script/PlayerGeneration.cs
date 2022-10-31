using UnityEngine;
using Photon.Pun;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerGeneration : MonoBehaviourPunCallbacks
{
    [Tooltip("生成位置")]
    [SerializeField] Transform[] m_startPos = default;

    GameObject[] m_players;

    bool m_setSkin = true;

    private void Awake()
    {
        Generation();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(PhotonNetwork.CurrentRoom.Players.Count);
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        }

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 4 && m_setSkin)
            {
                StartCoroutine(WaitTime());
                m_setSkin = false;
            }
        }
    }

    /// <summary>操作するハムスターを生成する </summary>
    private void Generation()
    {
        Transform pos = m_startPos[PhotonNetwork.CurrentRoom.PlayerCount - 1].GetChild(9).transform;      //生成位置を決める
        GameObject go = PhotonNetwork.Instantiate("Player", pos.position, Quaternion.identity);  //操作するハムスターを生成する
        GameObject mesh = go.transform.GetChild(1).gameObject;
        mesh.transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load(PlayerDataTest.FaceMaterialName, typeof(Material)) as Material;
        mesh.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load(PlayerDataTest.BodyMaterialName, typeof(Material)) as Material;
        SetCustomProperties();
    }

    /// <summary> 各プレイヤーが選択しているハムスタースキンを変える</summary>
    private void SetPlayersSkin()
    {
        Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;  //カスタムプロパティを取得

        m_players = GameObject.FindGameObjectsWithTag("Player");         //ルームにいるハムスターを取得

        for (var i = 0; i < m_players.Length; i++)     //m_playersから一つハムスターを選択する
        {
            for (var j = 1; j <= m_players.Length; j++)   //カスタムプロパティにあるプレイヤーの名前を取得しハムスターオブジェクトの名前と同じ名前のものを見つける
            {
                string name = (string)properties[$"P{j}"];

                if (name != m_players[i].name)
                {
                    continue;
                }

                //同じものが見つかったらカスタムプロパティからスキン名を取得しハムスターに貼り付けをする　🐹
                string face = (string)properties[$"F{j}"];
                string body = (string)properties[$"B{j}"];
                GameObject mesh = m_players[i].transform.GetChild(1).gameObject;
                mesh.transform.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load(face, typeof(Material)) as Material;
                mesh.transform.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().material = Resources.Load(body, typeof(Material)) as Material;
            }
        }
    }

    /// <summary>
    /// カスタムプロパティにプレイヤーの名前と使用しているスキンの
    /// マテリアル名を保存する
    /// </summary>
    private　void SetCustomProperties()
    {
        Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;

        properties[$"F{PhotonNetwork.CurrentRoom.PlayerCount}"] = PlayerDataTest.FaceMaterialName;
        properties[$"B{PhotonNetwork.CurrentRoom.PlayerCount}"] = PlayerDataTest.BodyMaterialName;
        properties[$"P{PhotonNetwork.CurrentRoom.PlayerCount}"] = PlayerDataTest.UserName;

        for (var i = 1; i <= 4; i++)
        {
            string face = (string)properties[$"F{i}"];
            string body = (string)properties[$"B{i}"];
            string name = (string)properties[$"P{i}"];
            properties[$"F{i}"] = face;
            properties[$"B{i}"] = body;
            properties[$"P{i}"] = name;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
    }

    /// <summary>
    /// SetPlayersSkin()を呼び出す前に待機時間を作り出す為の関数
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(5f);
        SetPlayersSkin();
    }
}
