using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Itemの基底クラス
/// </summary>
public abstract class ItemBase : MonoBehaviour
{
    /// <summary>アイテムの効果を発動する</summary>
    public abstract void Use();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller.CheckResult)
            {
                this.transform.position = new Vector3(0, -200, 0);
                this.gameObject.SetActive(false);
                controller.gameObject.GetComponent<PlayerController>().GetItem(this);
            }
        }
    }
}
