using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雷の当たり判定のスクリプト
/// </summary>
public class ThunderScript : MonoBehaviour
{
    /// <summary>当たり判定の範囲</summary>
    [SerializeField, Header("当たり判定の範囲")] Vector3 m_hitRange;
    /// <summary>当たり判定の半径</summary>
    [SerializeField, Header("当たり判定の半径")] float m_hitRadius = 0f;
    /// <summary>吹き飛ばす力</summary>
    [SerializeField, Header("吹き飛ばす力")] float m_hitPower = 0f;
    /// <summary>消えるまでの時間</summary>
    [SerializeField, Header("消えるまでの時間")] float m_lifeTime = 2f;
    /// <summary>スタン効果時間</summary>
    [SerializeField, Header("スタン効果時間")] float m_stanTime = 2f;
    Rigidbody rb;
    GameObject player;
    bool flag = false;
    void Start()
    {
        StartCoroutine(WaitTime());
        StartCoroutine(Setflag());
    }
    private void Update()
    {
        if (flag == true) //力を加える
        {
            rb.AddForce(-player.transform.forward * m_hitPower, ForceMode.Impulse);
        }
        else if (flag == false)
        {
            Hit();
        }
    }
    /// <summary>
    /// 当たり判定の中心を取得する
    /// </summary>
    /// <returns></returns>
    private Vector3 HitRangeCenter()
    {
        Vector3 center = this.transform.position + this.transform.right * m_hitRange.x
            + this.transform.up * m_hitRange.y
            + this.transform.forward * m_hitRange.z;
        return center;
    }

    /// <summary>
    /// プレイヤーの情報を取得
    /// </summary>
    private void Hit()
    {
        var cols = Physics.OverlapSphere(HitRangeCenter(), m_hitRadius);
        foreach (var c in cols)
        {
            if (c.gameObject.tag == "Player")
            {
                Debug.Log("当たる");
                player = c.gameObject;
                PlayerController controller = player.GetComponent<PlayerController>();
                rb = player.GetComponent<Rigidbody>();
                player.GetComponent<PlayerController>().CurrentState = State.stan;
                StartCoroutine(controller.ReleaseStan(m_stanTime));
                flag = true;
                StartCoroutine(Setflag());
            }
        }
    }

    /// <summary>ギズモを描画</summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(HitRangeCenter(), m_hitRadius);
    }

    /// <summary>
    /// 力を加えるのを止めて
    /// 雷オブジェクトを消す
    /// </summary>
    /// <returns></returns>
    private IEnumerator Setflag()
    {
        yield return new WaitForSeconds(10f);
        flag = false;
        Destroy(this.gameObject);
        ThunderGenerator.FallFlag = true;
    }

    /// <summary>
    /// 少し遅らせてから雷の当たり判定を出す
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(1f);
        Hit();
    }
}