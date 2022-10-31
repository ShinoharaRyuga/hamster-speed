using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GoalDirection : MonoBehaviour
{

    // Start is called before the first frame update
    [SerializeField,Header("ゴールのタグ")] public string m_goalTag = null;
    [SerializeField,Header("ミニマップのカメラ")] private Camera m_minimapCamera;
    //[SerializeField] private Transform m_iconTarget;
    [SerializeField, Header("表示範囲")] private float m_rangeRadiusOffset = 1.0f;
    private GameObject m_goal;

    private SpriteRenderer spriteRenderer;

    private float m_minimapRangeRadius;
    private float m_defaultPosY;
    const float m_normalAlpha = 1.0f;
    const float m_outRangeAlpha = 0.5f;
    
    private void Start()
    {
        m_goal = GameObject.FindGameObjectWithTag(m_goalTag);
        m_minimapRangeRadius = m_minimapCamera.orthographicSize;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_defaultPosY = transform.position.y;
    }

    // Update is called once per frame
    private void Update()
    {
        //DispIcon();
        var m_iconPos = new Vector3(m_goal.transform.position.x, m_defaultPosY, m_goal.transform.position.z);

        if (CheckInsideMap())
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, m_normalAlpha);
            transform.position = m_iconPos;
            return;
        }
        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, m_outRangeAlpha);
        var centerPos = new Vector3(m_minimapCamera.transform.position.x, m_defaultPosY, m_minimapCamera.transform.position.z);
        var offset = m_iconPos - centerPos;
        transform.position = centerPos + Vector3.ClampMagnitude(offset, m_minimapRangeRadius - m_rangeRadiusOffset);
    }

    /*private void DispIcon()
    {
               
    }*/

    private bool CheckInsideMap()
    {
        var m_cameraPos = m_minimapCamera.transform.position;
        var m_targetPos = m_goal.transform.position;

        // 直線距離で判定するため、yは0扱いにする
        m_cameraPos.y = m_targetPos.y = 0;

        return Vector3.Distance(m_cameraPos, m_targetPos) <= m_minimapRangeRadius - m_rangeRadiusOffset;
    }
}
