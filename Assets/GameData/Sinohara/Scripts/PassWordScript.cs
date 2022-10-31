using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// パスワードを表示したり非表示にする為
/// のスクリプト
/// </summary>
public class PassWordScript : MonoBehaviour
{
    [Tooltip("目画像 0=非表示 1=表示")]
    [SerializeField] Sprite[] m_eyesSprites = default;
    [Tooltip("パスワード欄でよく見る目を表示する画像")]
    [SerializeField] Image m_lookImage = default;
    /// <summary>パスワード入力欄 </summary>
    InputField m_passoword = default;

    /// <summary>パスワードが見える状態であるか　true=見える </summary>
    bool m_isLook = false; 
    // Start is called before the first frame update
    void Start()
    {
        m_passoword = GetComponent<InputField>();
    }
    
    /// <summary>
    /// m_isLookに応じて目のイラストを変更する
    /// </summary>
    public void ChangeImage()
    {
        if (!m_isLook) //パスワード可視状態にする
        {
            m_passoword.contentType = InputField.ContentType.Alphanumeric;
            m_passoword.ActivateInputField();
            m_lookImage.sprite = m_eyesSprites[1];
            m_isLook = true;
        }
        else  //パスワード不可視状態にする
        {
            m_passoword.contentType = InputField.ContentType.Password;
            m_passoword.ActivateInputField();
            m_lookImage.sprite = m_eyesSprites[0];
            m_isLook = false;
        }
    }
}
