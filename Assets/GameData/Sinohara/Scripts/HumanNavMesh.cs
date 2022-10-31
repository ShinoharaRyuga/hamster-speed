using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanNavMesh : MonoBehaviour
{
    NavMeshAgent m_agent;

    [SerializeField] Transform m_Endpos;
    [SerializeField] Transform m_Stertpos;

    bool m_flag = false;
    
    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_agent.SetDestination(m_Endpos.position);
        StartCoroutine(SetGoal());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void LateUpdate()
    {
        if (m_agent.remainingDistance <= 1f && m_flag) //ゴールについた　ゴールに行く
        {
            m_agent.SetDestination(m_Stertpos.position);
            StartCoroutine(SetStart());
        }

        if (m_agent.remainingDistance <= 1f && !m_flag)
        {
            m_agent.SetDestination(m_Endpos.position);
            StartCoroutine(SetGoal());
        }
    }

    IEnumerator SetGoal()
    {
        yield return new WaitForSeconds(1f);
        m_flag = true;
    }

    IEnumerator SetStart() //コルーチン
    {
        yield return new WaitForSeconds(1f);
        m_flag = false;
    }
}
