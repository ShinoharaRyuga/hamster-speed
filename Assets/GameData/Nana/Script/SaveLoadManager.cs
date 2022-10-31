using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NCMB;

public class SaveLoadManager : MonoBehaviour
{
    List<NCMBObject> m_objList = new List<NCMBObject>();
    void Start()
    {
        DataLoad();
    }

    public void DataLoad()//ロード
    {
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("UserData");
        
        //データストアでの検索を行う
        query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
        {
            if (e != null)
            {
                //検索失敗時の処理
                Debug.Log("取得失敗");
            }
            else
            {
                //検索成功時の処理。リスト型データのセーブデータ一覧を作成する。
                Debug.Log("取得成功");
                foreach (NCMBObject obj in objList)
                {
                    string n = System.Convert.ToString(obj["Win"]);
                    string a = obj["Win"].ToString();
                }
                m_objList = objList;
            }
        });

    }
    public void Save()//セーブ
    {
        NCMBObject obj = new NCMBObject("UserData");
        foreach (var item in m_objList)
        {
            Debug.Log((string)item["Name"]);
            Debug.Log(PhotonNetwork.NickName);
            if ((string)item["Name"] == PhotonNetwork.NickName)
            {
                obj = item;
            }
        }

        string a = obj["Win"].ToString();
        //win = int.Parse(a);
        //win++;
        //obj["Win"] = win;
        string selfID;

        obj.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                Debug.Log(e);

                Debug.Log("セーブ失敗");
                //エラー処理
            }
            else
            {
                Debug.Log("セーブ成功");
                //成功時の処理
                selfID = obj.ObjectId;
            }
        });
    }
}
