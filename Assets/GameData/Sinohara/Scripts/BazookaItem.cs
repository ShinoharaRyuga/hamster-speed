using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アイテムを回転させる
/// バズーカを使用可能にする
/// </summary>
public class BazookaItem : ItemBase
{
    [Tooltip("回転速度")]
    [SerializeField] float m_rotateSpeed = 0f;
    public override void Use()
    {
        FindObjectOfType<PlayerController>().ShootHookFlag = true;
    }

    void Update()
    {
        this.transform.Rotate(0, -m_rotateSpeed, 0);
    }
}