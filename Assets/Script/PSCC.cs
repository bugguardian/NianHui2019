using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class PSCC : MonoBehaviour, UGUIActor
{
    public GameObject PSObj;
    public GameObject BGImageObj;

    private Image BGImage;
    private Sprite[] SpriteArr;
    private Material PSMat;
    private ParticleSystem PS;
    private RenderTexture rt;
    private int numTilesX, numTilesY;
    private CombineRenderTexture crt;
    private Texture2D GetEmployeePhoto(int id)
    {
        return crt != null ? crt.textures[id] : null;
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
    private int _frame = 0;
    private float _totalTime = 0;
    private Sprite ShowSpriteSequence(Sprite[] sprites, int fps, int begin = 0)
    {
        //if (_frame == 0) Debug.Log(Time.time);
        return ShowSpriteSequence(sprites, ref _frame, begin, sprites.Length - 1, fps, Time.deltaTime, ref _totalTime);
    }
    private Sprite ShowSpriteSequence(Sprite[] sprites, ref int frame, int minFrame, int maxFrame, int fps, float deltaTime, ref float totalTime)
    {
        totalTime += deltaTime;
        if (totalTime >= (1f / fps - 0.01f))
        {
            frame = Mathf.Max(frame, minFrame);
            frame += 1;
            if (frame > maxFrame) frame = minFrame;
            totalTime = 0;
        }
        return sprites[frame];
    }
    private void Init()
    {
        if (PSObj != null)
        {
            PS = PSObj.GetComponent<ParticleSystem>();
            PSMat = PSObj.GetComponent<ParticleSystemRenderer>().material;

            rt = crt.CreateCombineRT(out numTilesX, out numTilesY);
            PSMat.SetTexture("_MainTex", rt);

            SetParticleCount(numTilesX * numTilesY);
            SetParticleTitleXY(numTilesX, numTilesY);
        }
        if (BGImageObj != null)
        {
            BGImage = BGImageObj.GetComponent<Image>();
            SpriteArr = Resources.LoadAll<Sprite>("BGSequence");
        }
    }
    PhotoLayout pl = new PhotoLayout(1406, 954);
    void Start()
    {
        string path = "PIC";

        crt = new CombineRenderTexture(path);

        Init();
        SetParticleCount(0);
        pl.init();
    }

    // Update is called once per frame
    void Update()
    {
        BGImage.sprite = ShowSpriteSequence(SpriteArr, 25, 500);
    }
    private void OnGUI()
    {
        pl.test();
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