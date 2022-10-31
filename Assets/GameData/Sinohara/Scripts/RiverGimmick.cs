using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGimmick : MonoBehaviour
{
    /// <summary>ノーツが判定をするの位置にあるかどうか</summary>
    bool m_notesHit = false;
    /// <summary>playerの入力があるかどうか</summary>
    bool m_hit = false;

    string m_buttonName = "W";

    public string ButtonName { get => m_buttonName; set => m_buttonName = value; }

    void Update()
    {
        if (Input.GetButtonDown(m_buttonName) && m_notesHit)
        {
            m_hit = true;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Notes") m_notesHit = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Notes" && m_hit)
        {
            Destroy(collision.gameObject);
            NotesManager.NextNotes = true;
            NotesManager.CurrentNotesNumber++;
            m_notesHit = false;
            m_hit = false;
        } 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Notes")
        {
            m_notesHit = false;
            m_hit = false;
        }
    }
}
