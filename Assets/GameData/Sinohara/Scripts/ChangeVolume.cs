using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// bgmの音量を変化する
/// スクリプト
/// </summary>
public class ChangeVolume : MonoBehaviour
{
    AudioSource m_audioSource = default;
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.volume = ConfigurationManager.BgmVolume;
    }
}
