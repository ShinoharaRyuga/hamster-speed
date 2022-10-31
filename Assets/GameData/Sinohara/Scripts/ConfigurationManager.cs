using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 設定画面の管理するスクリプト
/// （bgm・SEの音量調整、感度設定）
/// </summary>
public class ConfigurationManager : MonoBehaviour
{
    /// <summary>bgmの音量を調整するスライダー</summary>
    [SerializeField] Slider m_bgmSlider = default;
    /// <summary>seの音量を調整するスライダー</summary>
    [SerializeField] Slider m_seSlider = default;
    /// <summary>視点感度を調整するスライダー</summary>
    [SerializeField] Slider m_sensitivitySlider = default;
    /// <summary>bgmを出力する為のソース</summary>
    [SerializeField] AudioSource m_audioSource = default;
    /// <summary>ボタンを押した時のSE</summary>
    [SerializeField] AudioClip m_btnSE = default;
    [SerializeField] EventSystem m_eventSystem = default;

    [SerializeField] GameObject m_endCredit = default;
    /// <summary>seを出力する為のソース</summary>
    AudioSource m_seAudioSource = default;
    /// <summary>値を調整するSlider</summary>
    Slider m_currentSlider = default;
    /// <summary>ハンドルのエリアr</summary>
    [SerializeField] GameObject m_handleSlideArea = default;
    /// <summary>ハンドルの画像</summary>
    Image m_handleImage = default;

    bool m_setEndFlag = false;

    /// <summary>seの音量</summary>
    static float m_seVolume = 0.5f;
    /// <summary>bgmの音量</summary>
    static float m_bgmVolume = 0.5f;
    /// <summary>実際の視点感度</summary>
    static float m_sensitivityValue = 300f;
    /// <summary>設定用の視点感度 </summary>
    float m_sensitivity = 0.5f;
    /// <summary>seの音量</summary>
    public static float SeVolume { get => m_seVolume; set => m_seVolume = value; }
    /// <summary>bgmの音量</summary>
    public static float BgmVolume { get => m_bgmVolume; set => m_bgmVolume = value; }
    /// <summary>視点感度</summary>
    public static float SensitivityValue { get => m_sensitivityValue; set => m_sensitivityValue = value; }
    public GameObject EndCredit { get => m_endCredit; set => m_endCredit = value; }

    void Start()
    {
        m_seAudioSource = this.gameObject.AddComponent<AudioSource>();
        m_currentSlider = m_seSlider;
        m_handleSlideArea = m_currentSlider.transform.GetChild(2).gameObject;
        m_handleImage = m_handleSlideArea.transform.GetChild(0).GetComponent<Image>();
        m_handleImage.color = Color.red;
        m_bgmSlider.value = m_bgmVolume;
        m_seSlider.value = m_bgmVolume;
        m_sensitivitySlider.value = m_sensitivity;
        m_bgmSlider.onValueChanged.AddListener(value => this.m_audioSource.volume = value);
        m_seSlider.onValueChanged.AddListener(value => this.m_seAudioSource.volume = value);
    }

    // Update is called once per frame
    void Update()
    {
        var mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        m_bgmVolume = m_bgmSlider.value;
        m_seVolume = m_seSlider.value;
        m_sensitivity = m_sensitivitySlider.value;
        SetSensitivity();
        
        m_currentSlider.value += mouseScrollWheel;
        if (Input.GetButton("Fire1") && m_eventSystem.currentSelectedGameObject != null)
        {
            if (m_eventSystem.currentSelectedGameObject.tag == "Slider")
            {
                if (m_handleImage != null)
                {
                    m_handleImage.color = Color.white;
                }
                m_currentSlider = m_eventSystem.currentSelectedGameObject.GetComponent<Slider>();
                m_handleSlideArea = m_currentSlider.transform.GetChild(2).gameObject;
                m_handleImage = m_handleSlideArea.transform.GetChild(0).GetComponent<Image>();
                m_handleImage.color = Color.red;
            }
        }
    }

    /// <summary>ボタンを押した時にSEを出す為の関数</summary> 
    public void PushButtonSE()
    {
        m_seAudioSource.PlayOneShot(m_btnSE);
    }

    /// <summary>設定用の視点感度を通じて実際の感度を設定する</summary>
    public void SetSensitivity()
    {
        switch (m_sensitivity)
        {
            case 0.0f:
                m_sensitivityValue = 200f;
                break;
            case 0.1f:
                m_sensitivityValue = 220f;
                break;
            case 0.2f:
                m_sensitivityValue = 240f;
                break;
            case 0.3f:
                m_sensitivityValue = 260f;
                break;
            case 0.4f:
                m_sensitivityValue = 280f;
                break;
            case 0.5f:
                m_sensitivityValue = 300f;
                break;
            case 0.6f:
                m_sensitivityValue = 320f;
                break;
            case 0.7f:
                m_sensitivityValue = 340f;
                break;
            case 0.8f:
                m_sensitivityValue = 360f;
                break;
            case 0.9f:
                m_sensitivityValue = 380f;
                break;
            case 1.0f:
                m_sensitivityValue = 400f;
                break;
        }
    }

    public void SetEndCredit()
    {
        if (!m_setEndFlag)
        {
            m_setEndFlag = true;
            m_endCredit.SetActive(true);
        }
        else
        {
            m_setEndFlag = false;
            m_endCredit.SetActive(false);
        }
    }
}
