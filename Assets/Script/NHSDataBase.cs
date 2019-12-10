using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public enum EArea
{
    eGuangZhou,
    eWuHan,
    eShangHai,
    eChengDu,
    eMax,
}


public struct PersonInfo
{
    public int No;
    public string UserName;
    public int Area;

    public static EArea AreaFromString(string input)
    {
        if(input == "广州")
        {
            return EArea.eGuangZhou;
        }
        else if(input == "武汉")
        {
            return EArea.eWuHan;
        }
        else if(input == "上海")
        {
            return EArea.eShangHai;
        }
        else if(input == "成都")
        {
            return EArea.eChengDu;
        }
        return EArea.eMax;
    }

    public static PersonInfo[] ToPersionInfo(string input)
    {
        
        var matches = Regex.Matches(input, "(\"no\":\"(\\d+)\",\"username\":\"(\\w+)\",\"area\":\"(\\w+)\")");
        PersonInfo[] infos = new PersonInfo[matches.Count];
        for(int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];
            infos[i] = new PersonInfo()
            {
                No = int.Parse(match.Groups[2].Value),
                UserName = match.Groups[3].Value,
                Area = (int)AreaFromString(match.Groups[4].Value)
            };
        }
        return infos;
    }
}


enum EType
{
    eNormal,  // 普通未中奖
    eExtra,  // 老板核心加奖未中奖
    eOther, // 其它未中奖
    eNormalExtra,
}

public struct RoundRemainInfo
{
    public int Round; // 第几轮
    public string Award; // 本轮奖品名称
    public int Total; // 本轮抽奖人数
    public int Remain; // 本轮抽奖剩下人数 
    public int Num; // 当前抽奖人数
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
    public int remain;

    public static RoundInfo FromStringOne(string str)
    {
        string pattern = "(\\{\"id\":(\\d+),\"round\":\"(\\w+)\",\"num\":(\\d+),\"award\":\"(\\w+)\",\"addr\":(\\w+),\"remain\":(\\d+)\\})";
        var match = Regex.Match(str, pattern);
        RoundInfo info = new RoundInfo()
        {
            Id = int.Parse(match.Groups[2].Value),
            round = int.Parse(match.Groups[3].Value),
            num = int.Parse(match.Groups[4].Value),
            award = match.Groups[5].Value,
            remain = int.Parse(match.Groups[7].Value)
        };
        return info;
    }

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


class NHSDataBase
{
    public delegate void Callback(PersonInfo[] result);

    public delegate void RoundInfoCallback(RoundInfo[] result);

    public delegate void OneRoundInfoCallback(RoundInfo result);

    public delegate void ExtraCallback(PersonInfo[] result, ExtraInfo info);

    public delegate void NormalCallback(PersonInfo[] result, RoundRemainInfo info);

    private static string URLBase = "http://172.17.58.59:8181";

    private static string DrawURL = URLBase + "/draw";

    private static string DrawNormalURL = DrawURL + "/normal";

    private static string DrawExtraURL = DrawURL + "/extra";

    private static string DrawOtherURL = DrawURL + "/others";
    
    private static string DrawUnluckyURL = DrawURL + "/unlucky";

    private static string ResetURL = URLBase + "/sys/reset";

    private static string RequireNormalURL = URLBase +"/round/";


    private Dictionary<EType, List<List<PersonInfo>>> mCurrentInfos = new Dictionary<EType, List<List<PersonInfo>>>();


    private static NHSDataBase mInst = null;

    private PersonInfo[] mResult = null;

    private bool isDrawing = false;
    public static NHSDataBase Inst
    {
        get
        {
            if(mInst == null)
            {
                mInst = new NHSDataBase();
            }
            return mInst;
        }
    }

    public bool GetResult(out PersonInfo[] result)
    {
        result = mResult;
        mResult = null;
        return result != null;
    }

    public IEnumerator Normal(int round, int numPerTime, bool isExtra, NormalCallback callBack = null)
    {
        if (numPerTime <= 0 || isDrawing)
        {
            yield return null;
        }
        isDrawing = true;
        mResult = null;
        string url = string.Format("{0}?round={1}&num={2}&isExtra={3}", DrawNormalURL, round, numPerTime, isExtra);
        using (UnityWebRequest webReqest = UnityWebRequest.Get(url))
        {
            yield return webReqest.SendWebRequest();
            if (webReqest.isNetworkError)
            {
                Debug.Log("网络连接错误");
            }
            isDrawing = false;
            string value = webReqest.downloadHandler.text;
            mResult = PersonInfo.ToPersionInfo(value);

            List<List<PersonInfo>> typeList = null;
            EType t = isExtra ? EType.eNormalExtra : EType.eNormal;
            if (!mCurrentInfos.TryGetValue(t, out typeList))
            {
                typeList = new List<List<PersonInfo>>();
                mCurrentInfos.Add(t, typeList);
            }
            while (typeList.Count <= round - 1)
            {
                typeList.Add(new List<PersonInfo>());
            }
            typeList[round - 1].AddRange(mResult);
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
            mResult = PersonInfo.ToPersionInfo(value);
            List<List<PersonInfo>> typeList = null;
            if(!mCurrentInfos.TryGetValue(EType.eExtra, out typeList))
            {
                typeList = new List<List<PersonInfo>>();
                typeList.Add(new List<PersonInfo>());
                mCurrentInfos.Add(EType.eExtra, typeList);
                
            }
            typeList[0].AddRange(mResult);
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
            mResult = PersonInfo.ToPersionInfo(value);
            List<List<PersonInfo>> typeList = null;
            if (!mCurrentInfos.TryGetValue(EType.eOther, out typeList))
            {
                typeList = new List<List<PersonInfo>>();
                typeList.Add(new List<PersonInfo>());
                mCurrentInfos.Add(EType.eExtra, typeList);

            }
            typeList[0].AddRange(mResult);
            callBack?.Invoke(mResult);
        }
    }

    public List<PersonInfo> GetLucklyPersions(EType type, int round = 0)
    {
        List<List<PersonInfo>> typeList = null;
        if(mCurrentInfos.TryGetValue(type, out typeList))
        {
            if(typeList.Count > round)
            {
                return typeList[round];
            }
        }
        return new List<PersonInfo>();
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
            mResult = PersonInfo.ToPersionInfo(value);
            callBack?.Invoke(mResult);
        }
    }

    public IEnumerator RequireRoundInfo(int round, OneRoundInfoCallback callback)
    {
        if (isDrawing)
        {
            yield return null;
        }
        isDrawing = true;
        mResult = null;
        using (UnityWebRequest webReqest = UnityWebRequest.Get(RequireNormalURL + round))
        {
            yield return webReqest.SendWebRequest();
            if (webReqest.isNetworkError)
            {
                Debug.Log("网络连接错误");
            }
            isDrawing = false;
            string value = webReqest.downloadHandler.text;
            callback?.Invoke(RoundInfo.FromStringOne(value));
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
        }
    }
}
