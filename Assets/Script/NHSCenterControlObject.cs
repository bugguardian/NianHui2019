using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class NHSCenterControlObject : MonoBehaviour, UGUIActor
{
    public GameObject PSObj;
    public GameObject BGSequenceObj;
    public GameObject PhotoBoxPrefab;
    private Texture2D[] regionTextures;
    private Image BGImage;
    private Texture2D[] SequenceTextures;
    private Material m_ParticleMaterial;
    private ParticleSystem m_Particle;
    private RenderTexture rt;
    private int numTilesX, numTilesY;
    private NHSRenderObject EmployeePhotosCombineRender;
    private int SurprisedCount = 0;
    private int SurprisedChanceCount = 0;
    private int SurprisedRound = 1;
    private Dictionary<int, int> numpad = new Dictionary<int, int> {
        {0,0},{1,1},{2,2},{3,3},{4,4},{5,5},{6,6},{7,7},{8,8},{9,9},{10,10},{11,15},{12,20}};
    private Texture2D GetEmployeePhoto(int id)
    {
        return EmployeePhotosCombineRender != null ? EmployeePhotosCombineRender.textures[id] : null;
    }
    private void CleanSurprisedPhoto()
    {
        NHSObjectsPool.Inst.Destroy();
    }
    private void InitParticle(int titleX, int titleY, PersonInfo[] people = null)
    {
        if (people == null)
            rt = EmployeePhotosCombineRender.CreateCombineRT(out numTilesX, out numTilesY);
        else
            rt = EmployeePhotosCombineRender.CreateCombineRT(people, out numTilesX, out numTilesY);

        SetParticleTexture(rt);
        SetParticleCount(titleX * titleY);
        SetParticleTitleXY(titleX, titleY);
    }
    private void SetParticleTexture(Texture rt)
    {
        m_ParticleMaterial.SetTexture("_MainTex", rt);
    }
    private void SetParticleCount(int count)
    {
        MainModule mm = m_Particle.main;
        mm.maxParticles = count;
    }
    private void SetParticleTitleXY(int titleX, int titleY)
    {
        TextureSheetAnimationModule tsam = m_Particle.textureSheetAnimation;
        tsam.numTilesX = numTilesX;
        tsam.numTilesY = numTilesY;
    }
    private void CleanNumpadChecked()
    {
        for (int i = 1; i <= 12; i++) SetNumpadChecked(numpad[i], false);
        SurprisedCount = 0;
        SetSurprisedButtonActive(0, false);
        SetSurprisedButtonActive(1, false);
    }
    private void SetNumpadChecked(int num, bool active) //点击数字键盘触发
    {
        if (num > 0)
        {
            string bt = "bt" + num.ToString("D2") + "/" + num.ToString("D2");
            string tx = "bt" + num.ToString("D2") + "/" + num.ToString("D2") + "/Text";
            Button buttonObj = GameObject.Find("UI/Background/BGWidget/Numpad/" + bt).GetComponent<Button>();
            Text textObj = GameObject.Find("UI/Background/BGWidget/Numpad/" + tx).GetComponent<Text>();

            buttonObj.GetComponentInParent<Image>().color = active ? new Color(231 / 255f, 42 / 255f, 129 / 255f) : new Color(38 / 255f, 122 / 255f, 203 / 255f);
            textObj.color = active ? Color.yellow : Color.white;
        }
    }
    private void CheckNumpadActive()
    {
        for (int i = 1; i <= 12; i++)
        {
            if (numpad[i] > SurprisedChanceCount) SetNumpadActive(numpad[i], false);
        }
    }
    private void SetNumpadActive(int num, bool active)
    {
        string bt = "bt" + num.ToString("D2") + "/" + num.ToString("D2");
        string tx = "bt" + num.ToString("D2") + "/" + num.ToString("D2") + "/Text";
        Button buttonObj = GameObject.Find("UI/Background/BGWidget/Numpad/" + bt).GetComponent<Button>();
        Text textObj = GameObject.Find("UI/Background/BGWidget/Numpad/" + tx).GetComponent<Text>();

        buttonObj.interactable = active;
        textObj.text = active ? num.ToString() : "";
    }
    private void FreezeNumpadPanel(bool freeze)
    {
        for (int i = 0; i <= 12; i++) SetNumpadFreeze(numpad[i], freeze);
    }
    private void SetNumpadFreeze(int num, bool freeze)
    {
        string bt = "bt" + num.ToString("D2") + "/" + num.ToString("D2");
        string tx = "bt" + num.ToString("D2") + "/" + num.ToString("D2") + "/Text";

        Button buttonObj = GameObject.Find("UI/Background/BGWidget/Numpad/" + bt).GetComponent<Button>();
        Text textObj = GameObject.Find("UI/Background/BGWidget/Numpad/" + tx).GetComponent<Text>();
        Color color = textObj.color;

        buttonObj.interactable = !freeze;
        textObj.color = freeze ? new Color(color.r, color.g, color.b, 0.5f) : new Color(color.r, color.g, color.b, 1f);
    }
    private void SetSurprisedButtonActive(int id, bool active)
    {
        Button beginBT = GameObject.Find("UI/Background/BGWidget/Start/Button").GetComponent<Button>();
        Text beginText = GameObject.Find("UI/Background/BGWidget/Start/Button/Text").GetComponent<Text>();
        Button stopBT = GameObject.Find("UI/Background/BGWidget/Stop/Button").GetComponent<Button>();
        Text stopText = GameObject.Find("UI/Background/BGWidget/Stop/Button/Text").GetComponent<Text>();

        if (id == 0)
        {
            beginBT.interactable = active;
            //beginBT.GetComponentInParent<Image>().color *= new Color(1, 1, 1, active ? 2f : 0.5f);
            beginText.color = new Color(beginText.color.r, beginText.color.g, beginText.color.b, active ? 1f : 0.35f);
        }
        if (id == 1)
        {
            stopBT.interactable = active;
            stopText.color = new Color(stopText.color.r, stopText.color.g, stopText.color.b, active ? 1f : 0.35f);
        }
    }
    private void InitRoundpadData(int roundNum, string awardName, int luckyNum, int unluckyNum)
    {

    }
    private void SetRoundpadData(int roundNum, string awardName, int luckyNum, int unluckyNum)
    {
        Text roundNumText = GameObject.Find("UI/Background/BGWidget/Roundpad/roundNum").GetComponent<Text>();
        Text awardNumText = GameObject.Find("UI/Background/BGWidget/Roundpad/awardName").GetComponent<Text>();
        Text luckyNumText = GameObject.Find("UI/Background/BGWidget/Roundpad/luckyNum").GetComponent<Text>();
        Text unluckyNumText = GameObject.Find("UI/Background/BGWidget/Roundpad/unluckyNum").GetComponent<Text>();

        roundNumText.text = roundNum.ToString();
        awardNumText.text = awardName;
        luckyNumText.text = luckyNum.ToString();
        unluckyNumText.text = unluckyNum.ToString();
    }
    private void Init()
    {
        EmployeePhotosCombineRender = new NHSRenderObject("PIC");

        if (PSObj != null)
        {
            m_Particle = PSObj.GetComponent<ParticleSystem>();
            m_ParticleMaterial = PSObj.GetComponent<ParticleSystemRenderer>().material;

            InitParticle(numTilesX, numTilesY);
        }
        if (BGSequenceObj != null)
        {
            SequenceTextures = Resources.LoadAll<Texture2D>("BGSequence");
            NHSAnimationObject.Inst.InitTextureSequenceAnim(BGSequenceObj, SequenceTextures, 25, 0);
        }
        regionTextures = Resources.LoadAll<Texture2D>("PhotoBox/tex");
        NHSObjectsPool.Inst.Init(PhotoBoxPrefab, EmployeePhotosCombineRender.textures, regionTextures);
        NHSLayoutObject.Inst.Init(new Vector2(3900, 780), new Vector2(1406, 954));

        SetParticleCount(0); //设置粒子不发射
        SetSurprisedButtonActive(0, false); //设置Begin按钮不可用
        SetSurprisedButtonActive(1, false); //设置Stop按钮不可用
    }
    void Start()
    {
        Init(); //初始化抽奖前端数据

        StartCoroutine(NHSDataBase.Inst.Reset()); //初始化抽奖后端数据
    }
    void Update()
    {
        NHSAnimationObject.Inst.UpdateTextureSequenceAnim();
    }
    private void OnGUI()
    {
        switch (Event.current.keyCode)
        {
            case KeyCode.UpArrow:
                SurprisedRound = Mathf.Clamp(SurprisedRound + 1, 1, 20);
                break;
            case KeyCode.DownArrow:
                SurprisedRound = Mathf.Clamp(SurprisedRound - 1, 1, 20);
                break;
            default:
                break;
        }
    }
    public void SurprisedStartCallBackFun(PersonInfo[] people) //开始抽奖按钮触发回调
    {
        CleanSurprisedPhoto(); //清除当前中奖者照片
        InitParticle(numTilesX, numTilesY, people); //初始化照片泡粒子
        FreezeNumpadPanel(true);
        SetSurprisedButtonActive(0, false);
        SetSurprisedButtonActive(1, true);
    }
    private IEnumerator SurprisedStopCall(PersonInfo[] people, RoundRemainInfo info)
    {
        SetSurprisedButtonActive(1, false);
        int PSNum = 0;
        for (int i = 1; i < 1000; i++)
        {
            yield return new WaitForSeconds(0.1f);
            PSNum = PSObj.GetComponent<ParticleSystem>().particleCount;
            if (PSNum < 9) break;
        }

        TransfromX[] BeginTransArr = NHSLayoutObject.Inst.GetRandomTransformXLayout(SurprisedCount, new Rect(1800, 200, 150, 150));
        TransfromX[] EndTransArr = NHSLayoutObject.Inst.GetTransformXLayout(SurprisedCount);
        NHSAnimationObject.Inst.InitTransformAnim(NHSObjectsPool.Inst.ActivePhotoObjects, BeginTransArr, EndTransArr);

        // 创建中奖照片对象
        for (int i = 0; i < people.Length; i++)
        {
            NHSObjectsPool.Inst.Create(people[i].Area, people[i].No, people[i].UserName, BeginTransArr[i]);
        }
        // 中奖照片弹出动画刷新，控制动画速度
        while (NHSAnimationObject.Inst.UpdateTransformAnim(3)) yield return new WaitForSeconds(0.01f);

        SetRoundpadData(info.Round, info.Award, info.Num, info.Remain);
        CleanNumpadChecked();
        FreezeNumpadPanel(false);
        CheckNumpadActive();
    }
    public void SurprisedStopCallBackFun(PersonInfo[] people, RoundRemainInfo info) //停止抽奖按钮触发回调
    {
        SetParticleCount(0);

        SurprisedChanceCount = info.Remain;
        SurprisedRound = info.Round;

        StartCoroutine(SurprisedStopCall(people, info));
    }
    public void UGUI_Start_Click()
    {
        StartCoroutine(NHSDataBase.Inst.GetUnlucky(EType.eNormal, SurprisedRound, SurprisedStartCallBackFun));
    }

    public void UGUI_Stop_Click()
    {
        StartCoroutine(NHSDataBase.Inst.Normal(SurprisedRound, SurprisedCount, false, SurprisedStopCallBackFun));
    }
    public void UGUI_Numpad_Click(int num)
    {
        CleanNumpadChecked();
        SetNumpadChecked(num, true);
        SurprisedCount = num;
        if (SurprisedCount > 0) SetSurprisedButtonActive(0, true);
    }
}
public interface UGUIActor
{
    void UGUI_Start_Click();
    void UGUI_Stop_Click();
    void UGUI_Numpad_Click(int num);
}
