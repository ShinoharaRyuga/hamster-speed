using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NCMB;
using System;

public class PlayerData : MonoBehaviour
{
    /// <summary>勝利数</summary>
    int win;
    /// <summary>ロードするときのID</summary>
    string objId;
    /// <summary>勝利数のテキスト</summary>
    [SerializeField] Text winCount;
    /// <summary>勝利数の読み取ったもののリスト</summary>
    List<NCMBObject> m_objList = new List<NCMBObject>();
    /// <summary>顔マテリアルの読み取ったもののリスト</summary>
    List<NCMBObject> m_facemesh = new List<NCMBObject>();
    /// <summary>身体マテリアルの読み取ったもののリスト</summary>
    List<NCMBObject> m_hammesh = new List<NCMBObject>();

    void Start()
    {
       // Data();
    }

    /// <summary>データ検索</summary>
    public void Data()
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

                foreach (NCMBObject obj in objList)
                {
                  //  Debug.Log("取得成功");
                }
                m_objList = objList;
                Load();
            }
        });
        //query.FindAsync((List<NCMBObject> faceobjList, NCMBException e) =>
        //{
        //    if (e != null)
        //    {
        //        //検索失敗時の処理
        //        Debug.Log("顔マテリアル取得失敗");
        //    }
        //    else
        //    {
        //        //検索成功時の処理。リスト型データのセーブデータ一覧を作成する。
        //        Debug.Log("顔マテリアル取得成功");
        //        foreach (NCMBObject obj in faceobjList)
        //        {
        //            string face = System.Convert.ToString(obj["facemesh"]);
        //            //string faceobj = obj["facemesh"].ToString();
        //        }
        //        m_facemesh = faceobjList;
        //    }
        //});
        //query.FindAsync((List<NCMBObject> humobjList, NCMBException e) =>
        //{
        //    if (e != null)
        //    {
        //        //検索失敗時の処理
        //        Debug.Log("身体マテリアル取得失敗");
        //    }
        //    else
        //    {
        //        //検索成功時の処理。リスト型データのセーブデータ一覧を作成する。
        //        Debug.Log("身体マテリアル取得成功");
        //        foreach (NCMBObject obj in humobjList)
        //        {
        //            string hum = System.Convert.ToString(obj["hummesh"]);
        //            //string humobj = obj["hummesh"].ToString();

        //        }
        //        m_hammesh = humobjList;
        //    }
        //});
    }
    
    /// <summary>データのロード</summary>
    public void Load()
    {
        NCMBObject obj = new NCMBObject("UserData");
        foreach (var item in m_objList)
        {
            if (objId == obj.ObjectId)
            {
                obj = item;
                string a = obj["Win"].ToString();
                win = int.Parse(a);
                obj["Win"] = win;
               // winCount.text = "" + win;
            }
        }
    }
}
