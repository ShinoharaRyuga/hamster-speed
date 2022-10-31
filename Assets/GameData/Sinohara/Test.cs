using UnityEngine;
using Photon.Pun;

public class Test : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log(PhotonNetwork.PlayerList.Length - 1); //Plyerの人数
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log(PhotonNetwork.CountOfRooms); //ルームの数
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
