using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーデータを所持
/// 戦績UIで勝利数、最速タイム、名前を表示する
/// </summary>
public class PlayerDataTest : MonoBehaviour
{
    [Tooltip("勝利数を表示")]
    [SerializeField] Text m_winCountText = default;
    [Tooltip("プレイヤーネームを表示するテキスト")]
    [SerializeField] Text m_nameText = default;
    [Tooltip("最速タイムを表示するテキスト")]
    [SerializeField] Text m_bestTimeText = default;

    /// <summary>勝利数</summary>
    static int m_winCount = 0;
    /// <summary>プレイヤーが使っているハムスターの顔マテリアルの名前</summary>
    static string m_faceMaterialName = "";
    /// <summary>プレイヤーが使っているハムスターの体マテリアルの名前</summary>
    static string m_bodyMaterialName = "";
    /// <summary>プレイヤーネーム</summary>
    static string m_userName = "";
    /// <summary>前回のタイム</summary>
    static float m_playerTime;

    /// <summary>プレイヤーネーム</summary>
    public static string UserName { get => m_userName; set => m_userName = value; }
    /// <summary>プレイヤーが使っているハムスターの顔マテリアルの名前</summary>
    public static string FaceMaterialName { get => m_faceMaterialName; set => m_faceMaterialName = value; }
    /// <summary>プレイヤーが使っているハムスターの顔マテリアルの名前</summary>
    public static string BodyMaterialName { get => m_bodyMaterialName; set => m_bodyMaterialName = value; }
    /// <summary>勝利数</summary>
    public static int WinCount { get => m_winCount; set => m_winCount = value; }
    /// <summary>前回のタイム</summary>
    public static float PlayerTime { get => m_playerTime; set => m_playerTime = value; }

    private void Start()
    {
        m_winCountText.text = m_winCount.ToString();
        m_nameText.text = m_userName;
        m_bestTimeText.text = m_playerTime.ToString();
    }
}