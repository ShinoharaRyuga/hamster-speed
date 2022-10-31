using UnityEngine;
using UnityEngine.SceneManagement;

public class SigninScene : MonoBehaviour
{
    public void signinScene()
    {
        SceneManager.LoadScene("NewAcountScene");
    }
    public void backScene()
    {
        SceneManager.LoadScene("Login");
    }
}
