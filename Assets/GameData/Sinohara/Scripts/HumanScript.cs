using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  人ギミックでハムスターを
///  飛ばすスクリプト
/// </summary>
public class HumanScript : MonoBehaviour
{
    /// <summary>吹き飛ばす力</summary>
    [SerializeField, Header("吹き飛ばす力")] float m_power = 0f;
    /// <summary>スタン効果時間</summary>
    [SerializeField, Header("スタン効果時間")] float m_stanTime = 2f;
    Rigidbody rb;
    bool flag = false;

    private void Update()
    {
        if(flag == true)
        {
            Debug.Log(transform.root.name);
            rb.AddForce(transform.root.forward * m_power, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("hit");
            flag = true;
            rb = collision.gameObject.GetComponent<Rigidbody>();
            StartCoroutine(Setflag());
        }
    }


    private IEnumerator Setflag()
    {
        yield return new WaitForSeconds(2f);
        flag = false;
    }
}
