using UnityEngine;

/// <summary>
/// SpeedUpアイテムの処理スクリプト
/// 抽象クラス
/// </summary>
public class SpeedUpItem : ItemBase
{
    [Tooltip("アイテム取得後のPlayerのスピード 元の値より大きくして下さい")]
    [SerializeField] float m_chageSpeed = 0f;
    [Tooltip("アイテムの回転速度")]
    [SerializeField] float m_rotateSpeed = 0f;
    
    public override void Use()
    {
        FindObjectOfType<PlayerController>().SpeedUp(m_chageSpeed);
    }

    void Update()
    {
        this.transform.Rotate(0, -m_rotateSpeed, 0);
    }
}
