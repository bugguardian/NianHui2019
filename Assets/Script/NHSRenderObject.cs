using System.Collections.Generic;
using UnityEngine;

public class NHSRenderObject
{
    public int[] mIds;

    public Dictionary<int, Texture2D> textures;

    private Texture2D clearTex;
    private Material rtMat;
    private RenderTexture rt;

    private int RTW = 4096;
    private int RTH = 4096;

    private Vector4 mOffsetScale = new Vector4();
    public NHSRenderObject(string path)
    {
        init(path);
    }
    private int getNumber(string s)
    {
        string numStr = "";
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] >= '0' && s[i] <= '9')
            {
                numStr += s[i];
            }
        }
        return int.Parse(numStr);
    }
    private void init(string path)
    {
        Object[] objs = Resources.LoadAll(path);
        clearTex = new Texture2D(1, 1);
        clearTex.SetPixel(0, 0, Color.clear);
        Texture2D t = objs[0] as Texture2D;

        textures = new Dictionary<int, Texture2D>(objs.Length);
        List<int> keys = new List<int>();
        for (int i = 0; i < objs.Length; i++)
        {
            Texture2D tex = objs[i] as Texture2D;

            int key = getNumber(tex.name);
            if (keys.Contains(key)) continue;

            keys.Add(key);
            textures.Add(key, tex);
        }
        mIds = keys.ToArray();
        rtMat = new Material(Shader.Find("NIANHUI/merge"));
    }
    public RenderTexture GetCombineRT { get { return rt; } }
    public RenderTexture CreateCombineRT(out int numX, out int numY)
    {
        return CreateCombineRT(mIds, out numX, out numY);
    }
    public RenderTexture CreateCombineRT(int[] ids, out int numX, out int numY)
    {
        if (rt == null)
        {
            rt = new RenderTexture(RTW, RTH, 0, RenderTextureFormat.ARGB32);
        }
        numX = Mathf.CeilToInt(Mathf.Sqrt(ids.Length));
        numY = Mathf.CeilToInt(ids.Length / (float)numX);
        float _width = RTW / (float)numX;
        float _height = RTH / (float)numY;

        mOffsetScale.Set(0, 0, 1, 1);
        rtMat.SetVector("_OffsetScale", mOffsetScale);
        Graphics.Blit(clearTex, rt, rtMat);

        for (int i = 0; i < numX * numY; i++)
        {
            int index = i;
            if (i >= ids.Length) { index = Random.Range(0, ids.Length); }

            int x = i % numX;
            int y = i / numX;
            mOffsetScale.x = x * _width / RTW * 2.0f - 1 + _width / 4096.0f;
            mOffsetScale.y = y * _height / RTH * 2.0f - 1 + _height / 4096.0f;
            mOffsetScale.z = _width / RTW;
            mOffsetScale.w = _height / RTH;
            rtMat.SetVector("_OffsetScale", mOffsetScale);
            if (!textures.ContainsKey(ids[index])) { continue; }
            Graphics.Blit(textures[ids[index]], rt, rtMat);
        }

        return rt;
    }
}
