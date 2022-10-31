using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererScript : MonoBehaviour
{
    [SerializeField, Header("プレイヤーのタグ")] public string m_playerTag = null;
    [SerializeField, Header("ラインのタグ")] public string m_LineRendererObject = null;
    [SerializeField, Header("ゴールのタグ")] public string m_goalTag = null;
    private GameObject m_goal;
    // Start is called before the first frame update
    void Start()
    {
        m_goal = GameObject.FindGameObjectWithTag(m_goalTag);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject m_player = GameObject.FindGameObjectWithTag(m_playerTag);
        LineRenderer lineY = GameObject.FindGameObjectWithTag(m_LineRendererObject).GetComponent<LineRenderer>();
        lineY.SetPosition(0, m_player.transform.position);
        lineY.SetPosition(1, m_goal.transform.position);
    }
}
