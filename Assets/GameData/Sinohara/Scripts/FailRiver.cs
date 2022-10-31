using UnityEngine;

public class FailRiver : MonoBehaviour
{
    [SerializeField] NotesManager m_notesManager;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_notesManager.FailRiver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_notesManager.FailRiver();
        }
    }
}
