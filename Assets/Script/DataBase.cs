using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

struct PersionInfo
{
    public int No;
    public string UserName;
    public string Area;

    public static PersionInfo[] ToPersionInfo(string input)
    {
        
        var matches = Regex.Matches(input, "(\"no\":\"(\\d+)\",\"username\":\"(\\w+)\",\"area\":\"(\\w+)\")");
        PersionInfo[] infos = new PersionInfo[matches.Count];
        for(int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            infos[i] = new PersionInfo()
            {
                No = int.Parse(match.Groups[2].Value),
                UserName = match.Groups[3].Value,
                Area = match.Groups[4].Value
            };
        }
        return infos;
    }
}


enum EType
{
    eNormal,
    eExtra,
    eOther,
}

struct RoundRemainInfo
{
    public int Round;
    public string Award;
    public int Num;
    public int Total;
    public int Remain;
    public static RoundRemainInfo FromString(string str)
    {
        Match match = Regex.Match(str, ("\"details\":\\{\"round\":\"(\\d+)\",\"award\":\"(\\w+)\",\"num\":(\\d+),\"total\":(\\d+),\"remain\":(\\d+)\\}"));
        return new RoundRemainInfo()
        {
            Round = int.Parse(match.Groups[1].Value),
            Award = match.Groups[2].Value,
            Num = int.Parse(match.Groups[3].Value),
            Total = int.Parse(match.Groups[4].Value),
            Remain = int.Parse(match.Groups[5].Value)
        };
    }
}

struct RoundInfo
{
    public int Id;
    public int round;
    public int num;
    public string award;

    public static RoundInfo[] FromString(string str)
    {
        string pattern = "(\\{\"id\":(\\d+),\"round\":\"(\\w+)\",\"num\":(\\d+),\"award\":\"(\\w+)\",\"drawType\":(\\d+),\"addr\":(\\w+)\\})";
        var matches = Regex.Matches(str, pattern);
        RoundInfo[] infos = new RoundInfo[matches.Count];
        for(int i = 0; i <infos.Length; i++)
        {
            infos[i] = new RoundInfo()
            {
                Id = int.Parse(matches[i].Groups[2].Value),
                round = int.Parse(matches[i].Groups[3].Value),
                num = int.Parse(matches[i].Groups[4].Value),
                award = matches[i].Groups[5].Value
            };
        }
        return infos;
    }
}

struct ExtraInfo
{
    public int Num;
    public int Amount;
    public int Remain;

    public static ExtraInfo FromString(string str)
    {
        Match match = Regex.Match(str, ("\"details\":\\{\"num\":(\\d+),\"amount\":(\\d+),\"remain\":(\\d+)\\}"));
        return new ExtraInfo()
        {
            Num = int.Parse(match.Groups[1].Value),
            Amount = int.Parse(match.Groups[2].Value),
            Remain = int.Parse(match.Groups[3].Value)
        };
    }
}


class DataBase
{
    private int mCurrentRound = 1;

    public delegate void Callback(PersionInfo[] result);

    public delegate void RoundInfoCallback(RoundInfo[] result);

    public delegate void ExtraCallback(PersionInfo[] result, ExtraInfo info);

    public delegate void NormalCallback(PersionInfo[] result, RoundRemainInfo info);

    private static string URLBase = "http://172.17.58.59:8181";

    private static string DrawURL = URLBase + "/draw";

    private static string DrawNormalURL = DrawURL + "/normal";

    private static string DrawExtraURL = DrawURL + "/extra";

    private static string DrawOtherURL = DrawURL + "/others";
    
    private static string DrawUnluckyURL = DrawURL + "/unlucky";

    private static string ResetURL = URLBase + "/sys/reset";

    private static string RequireNormalURL = URLBase +"/round/";



    private static DataBase mInst = null;

    private PersionInfo[] mResult = null;

    private bool isDrawing = false;
    public static DataBase Inst
    {
        get
        {
            if(mInst == null)
            {
                mInst = new DataBase();
            }
            return mInst;
        }
    }

    public bool GetResult(out PersionInfo[] result)
    {
        result = mResult;
        mResult = null;
        return result != null;
    }

    public IEnumerator Normal(int round, int numPerTime, bool isExtra, NormalCallback callBack = null)
    {
        if(numPerTime <= 0 || isDrawing)
        {
            yield return null;
        }
        isDrawing = true;
        mResult = null;
        string url = string.Format("{0}?round={1}&num={2}&isExtra={3}", DrawNormalURL, round, numPerTime, isExtra);
        using (UnityWebRequest webReqest = UnityWebRequest.Get(url))
        {
            yield return webReqest.SendWebRequest();
            if(webReqest.isNetworkError)
            {
                Debug.Log("网络连接错误");
            }
            isDrawing = false;
            string value = webReqest.downloadHandler.text;
            mResult = PersionInfo.ToPersionInfo(value);

            callBack?.Invoke(mResult, RoundRemainInfo.FromString(value));
        }
    }

    public IEnumerator Extra(int num, int amount, ExtraCallback callBack = null)
    {
        if (num <= 0 || isDrawing || amount <= 0)
        {
            yield return null;
        }
        isDrawing = true;
        mResult = null;
        string url = string.Format("{0}?num={1}&amount={2}", DrawExtraURL, num, amount);
        using (UnityWebRequest webReqest = UnityWebRequest.Get(url))
        {
            yield return webReqest.SendWebRequest();
            if (webReqest.isNetworkError)
            {
                Debug.Log("网络连接错误");
            }
            isDrawing = false;
            string value = webReqest.downloadHandler.text;
            mResult = PersionInfo.ToPersionInfo(value);
            callBack?.Invoke(mResult, ExtraInfo.FromString(value));
        }
    }

    public IEnumerator Other(int num, string award, Callback callBack = null)
    {
        if (num <= 0 || isDrawing || string.IsNullOrEmpty(award))
        {
            yield return null;
        }
        isDrawing = true;
        mResult = null;
        string url = string.Format("{0}?num={1}&award={2}", DrawOtherURL, num, award);
        using (UnityWebRequest webReqest = UnityWebRequest.Get(url))
        {
            yield return webReqest.SendWebRequest();
            if (webReqest.isNetworkError)
            {
                Debug.Log("网络连接错误");
            }
            isDrawing = false;
            string value = webReqest.downloadHandler.text;
            mResult = PersionInfo.ToPersionInfo(value);
            callBack?.Invoke(mResult);
        }
    }

    public IEnumerator GetUnlucky(EType type, int round = 0, Callback callBack = null)
    {
        if (isDrawing ||(type == EType.eNormal && round <= 0))
        {
            yield return null;
        }
        isDrawing = true;
        mResult = null;
        string url = string.Format("{0}?round={1}", DrawUnluckyURL, (type == EType.eNormal ? round.ToString() : (type == EType.eExtra ? "PLUS" : "OTHER")));
        using (UnityWebRequest webReqest = UnityWebRequest.Get(url))
        {
            yield return webReqest.SendWebRequest();
            if (webReqest.isNetworkError)
            {
                Debug.Log("网络连接错误");
            }
            isDrawing = false;
            string value = webReqest.downloadHandler.text;
            mResult = PersionInfo.ToPersionInfo(value);
            callBack?.Invoke(mResult);
        }
    }

    public IEnumerator RequireInfo(RoundInfoCallback callBack)
    {
        if (isDrawing)
        {
            yield return null;
        }
        isDrawing = true;
        mResult = null;
        using (UnityWebRequest webReqest = UnityWebRequest.Get(RequireNormalURL))
        {
            yield return webReqest.SendWebRequest();
            if (webReqest.isNetworkError)
            {
                Debug.Log("网络连接错误");
            }
            isDrawing = false;
            string value = webReqest.downloadHandler.text;
            callBack?.Invoke(RoundInfo.FromString(value));
        }
    }

    public IEnumerator Reset()
    {
        using (UnityWebRequest webReqest = UnityWebRequest.Get(ResetURL))
        {
            yield return webReqest.SendWebRequest();
            mCurrentRound = 1;
        }
    }
}
