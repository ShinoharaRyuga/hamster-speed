using UnityEngine;

/// <summary>ノーツを移動させる </summary>
public class NotesScript : MonoBehaviour
{
    void Update()
    {
        transform.Translate(NotesManager.Speed, 0, 0);
    }
}
