using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PUNManager : MonoBehaviourPunCallbacks
{
    #region Public変数定義
    //Public変数の定義はココで
    [Tooltip("マッチングを開始するボタン")]
    [SerializeField] GameObject m_matchingButton = default;
    #endregion

    #region Private変数
    //Private変数の定義はココで
    string _gameVersion = "test";   //ゲームのバージョン。仕様が異なるバージョンとなったときはバージョンを変更しないとエラーが発生する。

    #endregion

    #region Public Methods

    private void Start()
    {
        //マスターサーバーへ接続
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;    //マスタークライアントがシーン遷移したら他のプレイヤーも一緒に遷移出来るようにする
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターサーバー接続");
        m_matchingButton.SetActive(true);   //マスターサーバーに接続出来たらマッチング出来るようにする
    }

    /// <summary>ルーム作成/参加をする </summary>
    public static void ChangeStage()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    #endregion

    #region Photonコールバック
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    //部屋に入った時に呼ばれる
    public override void OnJoinedRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;   //ルームを閉じる（ルーム参加を許可しない)
        Debug.Log("ルームに入りました。");

        if (PhotonNetwork.IsMasterClient)   //マスタークライアントだったらカスタムプロパティを設定する
        {
            Hashtable props = new Hashtable();

            for (var i = 1; i <= 4; i++)
            {
                props.Add($"F{i}", $"{i}");
                props.Add($"B{i}", $"{i}");
                props.Add($"P{i}", $"{i}");
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            PhotonNetwork.CurrentRoom.SetStartTime(PhotonNetwork.ServerTimestamp);
        }

        SceneManager.LoadScene("StageScene");
    }
    #endregion
}
