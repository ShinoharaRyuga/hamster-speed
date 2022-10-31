using UnityEngine;

/// <summary>
/// フック・ワイヤーを飛ばして
/// ハムスターを移動させる
/// </summary>
public class ShootHookScript : MonoBehaviour
{
    [Tooltip("フック")]
    [SerializeField] Transform m_grapplingHook;
    [Tooltip("プレイヤーの位置を移動させる為に取得")]
    [SerializeField] Transform m_player;
    [Tooltip("ワイヤーの終着点")]
    [SerializeField] Transform m_grapplingEndPoint;
    [Tooltip("フックが刺さるレイヤー")]
    [SerializeField] LayerMask m_grapplingLayer;
    [Tooltip("アイテムを使える最大距離")]
    [SerializeField] float m_maxGrappleDistance;
    [Tooltip("引っ張られる速度")]
    [SerializeField] float m_hookSpeed;
    [Tooltip("グラップリングを終わらせる場所を決める")]
    [SerializeField] Vector3 m_offset;

    private bool m_isShooting;
    private bool m_isGrappling;
    /// <summary>Rayが当たった場所</summary>
    private Vector3 m_hookPoint;

    LineRenderer m_lr;
    PlayerController m_controller;
    private void Start()
    {
        m_lr = GetComponent<LineRenderer>();
        m_controller = m_player.gameObject.GetComponent<PlayerController>();
        m_isGrappling = false;
        m_isShooting = false;
        m_lr.enabled = false;
    }

    private void Update()
    {
        if (m_grapplingHook.parent == this.transform)
            m_grapplingHook.localPosition = Vector3.zero;

        if (m_controller.ShootHookFlag)
        {
            m_controller.AimImage.gameObject.SetActive(true);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, m_maxGrappleDistance, m_grapplingLayer))　//レティクルの色を変える
                m_controller.AimImage.color = Color.red;
            else
                m_controller.AimImage.color = Color.white;

        }

        if (Input.GetButtonDown("Fire1") && m_controller.ShootHookFlag) //フックを飛ばす
        {
            m_grapplingHook.gameObject.SetActive(true);
            m_controller.AimImage.gameObject.SetActive(false);
            m_grapplingHook.rotation = Quaternion.Euler(-90, 0, 0);
            ShootHook();
        }

        if (m_isGrappling)
        {
            m_grapplingHook.position = Vector3.Lerp(m_grapplingHook.position, m_hookPoint, m_hookSpeed * Time.deltaTime); //フックを移動させる
            if (Vector3.Distance(m_grapplingHook.position, m_hookPoint) < 0.5f)
            {
                m_player.position = Vector3.Lerp(m_player.position, m_hookPoint - m_offset, m_hookSpeed * Time.deltaTime);

                if (Vector3.Distance(this.transform.position, m_hookPoint - m_offset) < 2f)　//プレイヤーの移動
                {
                    m_isGrappling = false;
                    m_lr.enabled = false;
                    m_controller.ShootHookFlag = false;
                    m_grapplingHook.gameObject.SetActive(false);
                    m_grapplingHook.SetParent(this.transform);
                    m_controller.AimImage.gameObject.SetActive(false);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (m_lr.enabled) //ワイヤーを描画する
        {
            m_lr.SetPosition(0, m_grapplingEndPoint.position);
            m_lr.SetPosition(1, this.transform.position);
        }
    }

    /// <summary>
    /// 移動先を決める
    /// </summary>
    private void ShootHook()
    {
        if (m_isShooting || m_isGrappling) return;


        m_isShooting = true;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, m_maxGrappleDistance, m_grapplingLayer))
        {
            m_hookPoint = hit.point;
            m_isGrappling = true;
            m_grapplingHook.parent = null;
            m_grapplingHook.LookAt(m_hookPoint);
            m_lr.enabled = true;
        }

        m_isShooting = false;
    }
}
