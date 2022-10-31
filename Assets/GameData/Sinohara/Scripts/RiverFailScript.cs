using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverFailScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Notes")
        {
            NotesManager.FailFlag = true;
            Destroy(collision.gameObject);
        }
    }
}
