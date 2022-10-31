using UnityEngine;

/// <summary>
/// プレイヤーがステージ外に行ってしまったら
/// ステージ内に戻す
/// </summary>
public class ReSetPlayerPos : MonoBehaviour
{
    [Tooltip("戻す位置")]
    [SerializeField] Transform m_resetPos = default;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.position = m_resetPos.position;
        }
    }
}
