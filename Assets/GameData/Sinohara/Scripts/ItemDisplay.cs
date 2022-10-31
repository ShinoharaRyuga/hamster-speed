using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// playerが所持しているアイテムを
/// UIに表示するスクリプト
/// </summary>
public class ItemDisplay : MonoBehaviour
{
    /// <summary>所持しているアイテムの画像 (画像を切り替える)</summary>
    Image m_itemDisPlay = default;
    [SerializeField] Sprite[] m_itmesprites = default;

    public Image ItemDisPlay { get => m_itemDisPlay; set => m_itemDisPlay = value; }

    private void Start()
    {
        m_itemDisPlay.enabled = false;
    }
    void Update()
    {
        if (Input.GetButtonDown("Item"))
        {
            m_itemDisPlay.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Himawari")
        {
            ChengeHimawari();
        }
    }

    /// <summary>UIをひまわりの種に変える</summary>
    public void ChengeHimawari()
    {
        m_itemDisPlay.enabled = true;
        m_itemDisPlay.sprite = m_itmesprites[0];
    }
}
