using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class NHSCenterControlObject : MonoBehaviour, UGUIActor
{
    public GameObject PSObj;
    public GameObject BGImageObj;
    public GameObject PhotoBoxPrefab;
    private Texture2D[] regionTextures;
    private Image BGImage;
    private Sprite[] SpriteArr;
    private Material PSMat;
    private ParticleSystem PS;
    private RenderTexture rt;
    private int numTilesX, numTilesY;
    private NHSRenderObject EmployeePhotosCombineRender;
    private int SurprisedCount = 0;
    private Texture2D GetEmployeePhoto(int id)
    {
        return EmployeePhotosCombineRender != null ? EmployeePhotosCombineRender.textures[id] : null;
    }
    private void SetParticleCount(int count)
    {
        MainModule mm = PS.main;
        mm.maxParticles = count;
    }
    private void SetParticleTitleXY(int titleX, int titleY)
    {
        TextureSheetAnimationModule tsam = PS.textureSheetAnimation;
        tsam.numTilesX = numTilesX;
        tsam.numTilesY = numTilesY;
    }

    private void Init()
    {
        if (PSObj != null)
        {
            PS = PSObj.GetComponent<ParticleSystem>();
            PSMat = PSObj.GetComponent<ParticleSystemRenderer>().material;

            rt = EmployeePhotosCombineRender.CreateCombineRT(out numTilesX, out numTilesY);
            PSMat.SetTexture("_MainTex", rt);

            SetParticleCount(numTilesX * numTilesY);
            SetParticleTitleXY(numTilesX, numTilesY);
        }
        if (BGImageObj != null)
        {
            SpriteArr = Resources.LoadAll<Sprite>("BGSequence");
            NHSAnimationObject.Inst.InitSequenceAnim(BGImageObj, SpriteArr, 25, 500);
        }
        regionTextures = Resources.LoadAll<Texture2D>("PhotoBox/tex");
        NHSObjectsPool.Inst.Init(PhotoBoxPrefab, EmployeePhotosCombineRender.textures, regionTextures);
        NHSLayoutObject.Inst.Init(new Vector2(3900, 780), new Vector2(1406, 954));
    }

    void Start()
    {
        string path = "PIC";

        EmployeePhotosCombineRender = new NHSRenderObject(path);

        Init();
        SetParticleCount(0);
    }

    void Update()
    {
        NHSAnimationObject.Inst.UpdateSequenceAnim();
        NHSAnimationObject.Inst.UpdateTransformAnim();
    }
    private void OnGUI()
    {
        bool _click = false;

        if (GUILayout.Button("FORWARD"))
        {
            if (PhotoBoxPrefab != null)
            {
                SurprisedCount--;
                SurprisedCount = Mathf.Clamp(SurprisedCount, 1, 13);
                _click = true;
            }
        }
        if (GUILayout.Button("BACKWARD"))
        {
            if (PhotoBoxPrefab != null)
            {
                SurprisedCount++;
                SurprisedCount = Mathf.Clamp(SurprisedCount, 1, 13);
                _click = true;
            }
        }
        if (GUILayout.Button("Destroy"))
        {
            NHSObjectsPool.Inst.Destroy();
        }
        GUILayout.Box(SurprisedCount.ToString());

        if (_click)
        {
            CallBackFun();
        }
    }
    public void CallBackFun()
    {
        NHSObjectsPool.Inst.Destroy();

        TransfromX[] BeginTransArr = NHSLayoutObject.Inst.GetRandomTransformXLayout(SurprisedCount, new Rect(1900, 300, 100, 50));
        TransfromX[] EndTransArr = NHSLayoutObject.Inst.GetTransformXLayout(SurprisedCount);
        NHSAnimationObject.Inst.InitTransformAnim(NHSObjectsPool.Inst.ActivePhotoObjects, BeginTransArr, EndTransArr);

        foreach (TransfromX trans in EndTransArr)
        {
            NHSObjectsPool.Inst.Create(0, 1643, "王颖2号", trans);
        }
    }
    public void UGUI_Start_Click()
    {
        SetParticleCount(numTilesX * numTilesY);
        SetParticleTitleXY(numTilesX, numTilesY);
    }

    public void UGUI_Stop_Click()
    {
        SetParticleCount(0);
    }
}
public interface UGUIActor
{
    void UGUI_Start_Click();
    void UGUI_Stop_Click();
}