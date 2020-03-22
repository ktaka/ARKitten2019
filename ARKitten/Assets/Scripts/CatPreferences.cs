using System; // DateTimeとTimeSpanを使うために必要
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatPreferences
{

    // 最後にごはんをあげた時間
    public static DateTime LastFeedTime
    {
        get
        {
            string dateStr = PlayerPrefs.GetString("lastFeedTime");
            return DateTime.Parse(dateStr);
        }
        set
        {
            string dateStr = value.ToString();
            Debug.Log(dateStr);
            PlayerPrefs.SetString("lastFeedTime", dateStr);
        }
    }

    // 最後にボール遊び、もしくはなでた時間
    public static DateTime LastCaredTime
    {
        get
        {
            string dateStr = PlayerPrefs.GetString("LastCaredTime");
            return DateTime.Parse(dateStr);
        }
        set
        {
            string dateStr = value.ToString();
            PlayerPrefs.SetString("LastCaredTime", dateStr);
        }
    }

    // なでた回数
    public static int StrokingNum
    {
        get { return PlayerPrefs.GetInt("strokingNum", 0); }
        set { PlayerPrefs.SetInt("strokingNum", value); }
    }

    // ボール遊びをした回数
    public static int BallPlayingNum
    {
        get { return PlayerPrefs.GetInt("ballPlayingNum", 0); }
        set { PlayerPrefs.SetInt("ballPlayingNum", value); }
    }

    // 機嫌が良い状態か
    public static bool IsGoodTemper()
    {
        if (BallPlayingNum > 10 && StrokingNum > 10 && ElapsedSecondsFromLastCared < 30.0f && !IsStarving())
        {
            return true;
        }
        return false;
    }

    // お腹がへった状態か
    public static bool IsStarving()
    {
        if (ElapsedSecondsFromLastFeed > 7200)
        {
            return true;
        }
        return false;
    }

    // ファイルに保存
    public static void Save()
    {
        PlayerPrefs.Save();
    }

    // 最後にごはんをあげた時間を保存
    public static void SaveLastFeedTime()
    {
        LastFeedTime = DateTime.UtcNow;
        Save();
    }

    // 最後にボール遊び、もしくはなでた時間を保存
    public static void SaveLastCaredTime()
    {
        LastCaredTime = DateTime.UtcNow;
    }

    // 最後にごはんをあげた時間からの経過時間
    public static double ElapsedSecondsFromLastFeed
    {
        get
        {
            TimeSpan elapsed = DateTime.UtcNow.Subtract(LastFeedTime);
            return elapsed.TotalSeconds;
        }
    }

    // 最後にボール遊び、もしくはなでた時間からの経過時間
    public static double ElapsedSecondsFromLastCared
    {
        get
        {
            TimeSpan elapsed = DateTime.UtcNow.Subtract(LastCaredTime);
            return elapsed.TotalSeconds;
        }
    }

    public static void addStrokingNum()
    {
        StrokingNum++;
        SaveLastCaredTime();
    }

    public static void addBallPlayingNum()
    {
        BallPlayingNum++;
        SaveLastCaredTime();
    }
}
