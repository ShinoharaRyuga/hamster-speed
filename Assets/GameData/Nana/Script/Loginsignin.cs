using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using NCMB;
using System.Collections;

/// <summary>
///　アカウントを管理する
///　ログインや新規登録をする
///　プレイヤー情報を取得
/// </summary>
public class Loginsignin : MonoBehaviourPunCallbacks
{
    public InputField UserName;
    public InputField PassWord;
    [SerializeField] Text Debug_Text = null; // Textオブジェクト
    [Tooltip("タイトル画像")]
    [SerializeField] GameObject m_title = default;
    private bool firstsign = false;
    string playerTime = "";

    public void Start()
    {
        if (SceneManager.GetActiveScene().name == "LogIn")
        {
            StartCoroutine(TitleChange());
        }
    }

    public void Scene()
    {
        // プレイヤー自身の名前を保存する
        PhotonNetwork.NickName = UserName.text;
        //battleシーンをロード
        PhotonNetwork.LoadLevel("N.LobbyScene");
    }

    public void Login()
    {
        //NCMBUserのインスタンス作成 
        NCMBUser user = new NCMBUser();
        // オブジェクトからTextコンポーネントを取得
        Text debug_text = Debug_Text.GetComponent<Text>();
        // ユーザー名とパスワードでログイン
        NCMBUser.LogInAsync(UserName.text, PassWord.text, (NCMBException e) =>
        {
            if (e != null)
            {
                Debug.Log("ログインに失敗: " + e.ErrorMessage);
                ;// debug_text.text = "ログインに失敗: " + "\n" + e.ErrorMessage;
                debug_text.text = "ログインに失敗: " + "\n" + "名前またはパスワードが違います";
            }
            else
            {
                Debug.Log("ログインに成功！");
                // テキストの表示を入れ替える
                debug_text.text = "ログインに成功";
                PlayerDataTest.UserName = UserName.text;
                //初回ログインなら、初期パラメーターを設定する関数へ飛ばす
                if (firstsign)
                {
                    SetFirstparam();
                }
                DataLoad();
            }
        });
    }

    public void Signin()
    {
        if (UserName.text.Length <= 12 && ChaeckPasswordText())
        {
            //NCMBUserのインスタンス作成 
            NCMBUser user = new NCMBUser();

            //ユーザ名とパスワードの設定
            user.UserName = UserName.text;
            user.Password = PassWord.text;
            // オブジェクトからTextコンポーネントを取得
            Text debug_text = Debug_Text.GetComponent<Text>();
            //会員登録を行う
            user.SignUpAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    Debug.Log("新規登録に失敗: " + e.ErrorMessage);
                    debug_text.text = "新規登録に失敗: " + e.ErrorMessage;
                }
                else
                {
                    Debug.Log("新規登録に成功");
                    // テキストの表示を入れ替える
                    debug_text.text = "新規登録に成功";

                    //会員登録に成功したのでそのままログインする。初回ログイン判定をTrueにする
                    firstsign = true;
                    Login();
                }
            });
        }
        else
        {
            Debug_Text.text = "パスワードの条件を満たしていません";
        }
    }

    void SetFirstparam()
    {
        //ログイン中の会員データを保管する変数
        NCMBUser currUser = NCMBUser.CurrentUser;

        //「UserData」クラスを選択している変数（この時点ではデータの取得はしていない）
        NCMBObject obj = new NCMBObject("UserData");
        obj["Name"] = UserName.text; //ここでは名前=ログイン時に使うユーザーネームということにします。
        obj["Win"] = 0;
        obj["facemesh"] = "Face01";
        obj["hummesh"] = "Hamsto00";
        obj["Time"] = 0;

        string selfID;

        //ACL = Access Control Listの略 アクセス権を制御するリスト
        NCMBACL acl = new NCMBACL();

        //全員のアクセス権を一旦「読み込み可, 書き込み禁止」に変更する
        acl.PublicReadAccess = true;
        acl.PublicWriteAccess = false;
        //例外として、自分だけは「読み込み可, 書き込み可」にする
        acl.SetReadAccess(currUser.ObjectId, true);
        acl.SetWriteAccess(currUser.ObjectId, true);

        //オブジェクトにACLを設定
        obj.ACL = acl;

        //「UserData」クラスを保存する関数です
        obj.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                //エラー処理
                Debug.Log("UserDataの保存に失敗しました。");
            }
            else
            {
                //成功時の処理

                //UserDataクラスに登録した自分のセーブデータのオブジェクトID（管理番号）をselfIDに格納します。
                selfID = obj.ObjectId;
                //selfIDを会員データの「UserDataID」に格納します。
                currUser["UserDataID"] = selfID;

                //変更した会員データをサーバーに保存します。
                currUser.SaveAsync((NCMBException ee) =>
                {
                    if (ee != null)
                    {
                        //エラー処理
                        Debug.Log("ユーザーID設定失敗");

                    }
                    else
                    {
                        //成功時の処理
                        Debug.Log("ユーザーID設定成功");
                    }
                });
            }
        });
    }

    /// <summary>
    /// パスワードが半角英数字で作成されているかどうか
    /// チェックする
    /// </summary>
    /// <returns>false=条件を満たしていない</returns>
    private bool ChaeckPasswordText()
    {
        if (!Regex.IsMatch(PassWord.text, @"^[\da-z]+$") || PassWord.text.Length < 8)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ユーザー情報を取得後ロビーシーンに切り替える
    /// </summary>
    public void DataLoad()
    {
        NCMBQuery<NCMBObject> data = new NCMBQuery<NCMBObject>("UserData");

        data.FindAsync((List<NCMBObject> list, NCMBException e) =>
        {
            if (e != null)
            {
                //検索失敗時の処理
                Debug.Log("顔マテリアル取得失敗");
            }
            else
            {
                //検索成功時の処理
                foreach (NCMBObject obj in list)
                {
                    string name = Convert.ToString(obj["Name"]);
                    if (PlayerDataTest.UserName == name)    //ログインしたアカウント名と取得した名前が同じだったらデータ取得
                    {
                        PlayerDataTest.BodyMaterialName = Convert.ToString(obj["hummesh"]);
                        PlayerDataTest.FaceMaterialName = Convert.ToString(obj["facemesh"]);
                        PlayerDataTest.WinCount = Convert.ToInt32(obj["Win"]);
                        string playerTime = Convert.ToString(obj["Time"]);
                        float time = 0f;
                        time = float.Parse(playerTime);
                        PlayerDataTest.PlayerTime = time;
                        //PlayerDataTest.PlayerTime = Convert.(obj["Time"]);
                        break;
                    }
                }
            }
        });

        Scene();
    }

    /// <summary> 一定時間後にタイトルの画像を見えなくする</summary>
    IEnumerator TitleChange()
    {
        yield return new WaitForSeconds(3f);
        m_title.SetActive(false);

    }
}
