using NCMB;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

/// <summary>
/// Goal判定のスクリプト
/// </summary>
public class GoalScript : MonoBehaviour
{
    [SerializeField] GameManager m_gameManager = default;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PhotonView view = collision.gameObject.GetComponent<PhotonView>();
            ResultManager.ChampionName = collision.gameObject.name;
            m_gameManager.ClearFlag = true;
            m_gameManager.MoveFlag = false;
            if (view.IsMine)
            {
                GameObject mesh = collision.transform.GetChild(1).gameObject;
                ResultManager.ChampionFace = mesh.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material;
                ResultManager.Championbody = mesh.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
                view.RPC("SetChampionMaterial", RpcTarget.All, mesh.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.name, mesh.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name);
                Save();
            }
        }
    }

    /// <summary>
    /// 優勝したプレイヤーの勝利数を増やしセーブする
    /// </summary>
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
                        PlayerDataTest.WinCount++;
                        obj["Win"] = PlayerDataTest.WinCount;
                        obj.SaveAsync();
                        break;
                    }
                }
            }
        });

        m_gameManager.EndSaveFlag = true;
    }
}
