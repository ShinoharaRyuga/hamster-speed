using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 天候を変えて天候のマテリアルをセット
/// するスクリプト
/// </summary>
public class WeatherGimmick : MonoBehaviour
{
    /// <summary>天気が切り替わるまでの時間</summary>
    [SerializeField, Header("天気が切り替わるまでの時間 (秒指定)")] float m_changeTime = 0f;
    /// <summary>晴れの割合</summary>
    [SerializeField, Header("晴れの割合"),Range(0, 10)] int m_sunnyRatio = 0;
    /// <summary>曇りの割合</summary>
    [SerializeField, Header("曇りの割合"), Range(0, 10)] int m_cloudyRatio = 0;
    /// <summary>雷の割合</summary>
    [SerializeField, Header("雷の割合"), Range(0, 10)] int m_thunderRatio = 0;
    [SerializeField, Header("天気が雷になった時のSE")] AudioClip m_thunderSE = default;
    /// <summary>現在の天気</summary>
    Weather m_currentWeather = Weather.Sunny;

    AudioSource m_audio;

    float m_time = 0f;
    /// <summary>割合が超えていないかチェックする変数summary>
    int m_totalRatio = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_totalRatio = m_sunnyRatio + m_cloudyRatio + m_thunderRatio;
        m_audio = this.gameObject.AddComponent<AudioSource>();

        if (m_totalRatio > 10)
            Debug.LogError("割合が超えています");
        
        if(m_totalRatio < 10)
            Debug.LogError("割合が足りません 10にしてください");

    }

    // Update is called once per frame
    void Update()
    {
        if (m_time >= m_changeTime)
        {
            ChangeWeather();
            m_time = 0;
        }
    }

    /// <summary>
    /// 天気を切り替える関数
    /// </summary>
    private void ChangeWeather()
    {
        int number = Random.Range(1, 11);
        Debug.Log(number);

        if (number <= m_sunnyRatio)  //晴れ
        {
            ThunderGenerator.FallFlag = false;
        }
        else if (m_sunnyRatio < number && number <= m_sunnyRatio + m_cloudyRatio) //曇り
        {
            ThunderGenerator.FallFlag = false;
        }
        else if (m_sunnyRatio + m_cloudyRatio < number) //雷
        {
            m_audio.PlayOneShot(m_thunderSE);
            ThunderGenerator.FallFlag = true;
        }
    }
}

public enum Weather
{
    Sunny,
    Cloudy,
    Thunder,
}
