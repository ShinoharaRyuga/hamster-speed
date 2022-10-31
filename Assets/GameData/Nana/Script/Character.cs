using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character : MonoBehaviour
{

    void Start()
    {  

    }


    public void GameStart()
    {
        if (NetWark.stage1 == true)
        {
            Debug.Log("Stage1が選択された");
            SceneManager.LoadScene("StageScene");
        }
        if (NetWark.stage2 == true)
        {
            Debug.Log("Stage2が選択された");
            SceneManager.LoadScene("NanaScene");
        }
        if (NetWark.stage3 == true)
        {
            SceneManager.LoadScene("");
        }
    }
}
