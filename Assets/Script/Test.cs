using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnGUI()
    {

        if(GUI.Button(new Rect(100, 100, 100, 40), "抽奖"))
        {
            StartCoroutine(DataBase.Inst.Normal(20, false, (PersionInfo[] result, RoundRemainInfo roundInfo)=>{
            
                for(int i = 0; i < result.Length; i++)
                {
                    Debug.Log("工号：" + result[i].No + "姓名：" + result[i].UserName + "地区：" + result[i].Area);
                }
            }));
        }

        if (GUI.Button(new Rect(100, 150, 100, 40), "核心加奖"))
        {
            StartCoroutine(DataBase.Inst.Extra(20, 2000, (PersionInfo[] result, ExtraInfo extraInfo) =>
            {

                for (int i = 0; i < result.Length; i++)
                {
                    Debug.Log("工号：" + result[i].No + "姓名：" + result[i].UserName + "地区：" + result[i].Area);
                }
            }));
        }

        if (GUI.Button(new Rect(100, 200, 100, 40), "其他奖"))
        {
            StartCoroutine(DataBase.Inst.Other(20, "手机", (PersionInfo[] result) =>
            {

                for (int i = 0; i < result.Length; i++)
                {
                    Debug.Log("工号：" + result[i].No + "姓名：" + result[i].UserName + "地区：" + result[i].Area);
                }
            }));
        }

        if (GUI.Button(new Rect(100, 250, 100, 40), "查询"))
        {
            StartCoroutine(DataBase.Inst.RequireInfo((RoundInfo[] result) =>
            {

                for (int i = 0; i < result.Length; i++)
                {
                    Debug.Log("Id：" + result[i].Id + "round：" + result[i].round+ "num：" + result[i].num + "award:"+result[i].award);
                }
            }));
        }

        //if(GUI.Button(new Rect(100, 200, 100, 40), "匹配"))
        //{
        //    PersionInfo.ToPersionInfo(DataBase.Inst.value);
        //}

        if (GUI.Button(new Rect(100, 300, 100, 40), "重置系统"))
        {
            StartCoroutine(DataBase.Inst.Reset());
        }

    }
}
