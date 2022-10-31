using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

/// <summary>
/// サーバー時刻を取得する
/// 開始時刻を設定する
/// </summary>
public static class GameTime
{
    private const string KeyStartTime = "StartTime";

    private static readonly Hashtable propsToSet = new Hashtable();

    // ゲームの開始時刻が設定されていれば取得する
    public static bool TryGetStartTime(this Room room, out int timestamp)
    {

        if (room.CustomProperties[KeyStartTime] is int value)
        {
            timestamp = value;
            return true;
        }
        else
        {
            timestamp = 0;
            return false;
        }
    }

    /// <summary>
    /// ゲームの開始時刻を設定する
    /// </summary>
    /// <param name="room"></param>
    /// <param name="timestamp"></param>
    public static void SetStartTime(this Room room, int timestamp)
    {
        if (PhotonNetwork.InRoom)
        {
            propsToSet[KeyStartTime] = timestamp;
            room.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }
}
