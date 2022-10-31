using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodGimmickEnd : MonoBehaviour
{
    PlayerController m_playerController = default;
    [SerializeField] Transform m_playerPos = default;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player" && other.GetComponent<PlayerController>().CurrentState == State.wood)
        {
            m_playerController = other.GetComponent<PlayerController>();
            ChengePlayerMode();
        }
    }

    private void ChengePlayerMode()
    {
        m_playerController.JumpFlag = true;
        m_playerController.CurrentState = State.normal;
        m_playerController.SetGravity(true);
        m_playerController.gameObject.transform.position = m_playerPos.position;
    }
}
