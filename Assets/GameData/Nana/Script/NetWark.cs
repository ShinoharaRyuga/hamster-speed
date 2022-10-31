using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class NetWark : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private InputField playerName;

    [SerializeField]
    private Dropdown stage;

    public static bool stage1 , stage2 , stage3 ;
    //public bool stage1, stage2, stage3;

    #region Public変数定義


    #endregion

    #region Private変数

    //ゲームのバージョン。仕様が異なるバージョンとなったときはバージョンを変更しないとエラーが発生する。
    string _gameVersion = "test";


    #endregion

    #region Public Methods

    void Start()
    {
        //マスターサーバーへ接続
        PhotonNetwork.ConnectUsingSettings();
        playerName = playerName.GetComponent<InputField>();
    }

    //ステージを選択したとき
    public void ChangeStage()
    {
        PhotonNetwork.JoinRandomRoom();
        if (stage.value == 1)
        {
            stage1 = true;
        }
        if (stage.value == 2)
        {
            stage2 = true;
        }
        if (stage.value == 3)
        {
            stage3 = true;
        }
    }

    #endregion

    #region Photonコールバック

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        // プレイヤー自身の名前を"Player"に設定する
        PhotonNetwork.NickName = playerName.text;
        Debug.Log(stage1 +""+ stage2 + "" + stage3);
        SceneManager.LoadScene("CharacterSelection");
       
    }
    #endregion
}
