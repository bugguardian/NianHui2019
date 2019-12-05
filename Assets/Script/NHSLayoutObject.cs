using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct TransfromX
{
    public Vector3 position;
    public Vector3 scale;
}
public class NHSLayoutObject
{
    private float _texWidth, _texHeight, _ratio;
    private Rect _ScreenRect;
    private static NHSLayoutObject _inst;
    public static NHSLayoutObject Inst
    {
        get
        {
            _inst = _inst == null ? new NHSLayoutObject() : _inst;
            return _inst;
        }
    }
    public void Init(Vector2 screenSize, Vector2 photoSize)
    {
        _texWidth = photoSize.x;
        _texHeight = photoSize.y;
        _ratio = _texWidth / _texHeight;
        _ScreenRect = new Rect(0, 0, screenSize.x, screenSize.y);
    }
    public TransfromX[] GetTransformXLayout(int id)
    {
        return ConvertRectToTransformX(GetRectLayout(id));
    }
    public Rect[] GetRectLayout(int id)
    {
        Rect[] rects = null;

        switch (id)
        {
            case 1:
                rects = GetPhotoesRect(_ScreenRect, 300, 1, 1, 1, 20, 40);
                break;
            case 2:
                rects = GetPhotoesRect(_ScreenRect, 300, 2, 2, 1, 20, 40);
                break;
            case 3:
                rects = GetPhotoesRect(_ScreenRect, 300, 3, 3, 1, 20, 40);
                break;
            case 4:
                rects = GetPhotoesRect(_ScreenRect, 270, 4, 4, 1, 20, 40);
                break;
            case 5:
                rects = GetPhotoesRect(_ScreenRect, 200, 5, 3, 2, 20, 40);
                break;
            case 6:
                rects = GetPhotoesRect(_ScreenRect, 200, 6, 3, 2, 20, 40);
                break;
            case 7:
                rects = GetPhotoesRect(_ScreenRect, 200, 7, 4, 2, 20, 40);
                break;
            case 8:
                rects = GetPhotoesRect(_ScreenRect, 200, 8, 4, 2, 20, 40);
                break;
            case 9:
                rects = GetPhotoesRect(_ScreenRect, 200, 9, 5, 2, 20, 40);
                break;
            case 10:
                rects = GetPhotoesRect(_ScreenRect, 200, 10, 5, 2, 20, 40);
                break;
            case 11:
            case 15:
                rects = GetPhotoesRect(_ScreenRect, 150, 15, 8, 2, 0, 40);
                break;
            case 12:
            case 20:
                rects = GetPhotoesRect(_ScreenRect, 120, 20, 10, 2, 5, 80);
                break;
            case 13:
            case 30:
                rects = GetPhotoesRect(_ScreenRect, 100, 30, 10, 3, 10, 40);
                break;
            default:
                break;
        }
        return rects;
    }
    public Rect[] GetPhotoesRect(Rect outRect, float height, int total, int titleX, int titleY, float marginX, float marginY, float posX = 0, float posY = 0)
    {
        Rect[] rects = new Rect[total];
        float kit = 0.14f;
        float width = height * _ratio;
        float sw = width + marginX;
        float sh = height + marginY;
        float paddingX = (outRect.width - (sw * titleX - marginX)) * 0.5f + width * kit;
        float paddingY = (outRect.height - (sh * titleY - marginY)) * 0.5f;
        float offset = 0;

        int i = 0;

        for (int _y = titleY - 1; _y >= 0; _y--)
        {
            if (total - i < titleX) offset = (titleX * titleY - total) * sw * 0.5f;
            for (int _x = 0; _x < titleX; _x++)
            {
                if (i >= total) break;
                rects[i++] = new Rect(posX + paddingX + outRect.x + sw * _x + offset,
                                      posY + paddingY + outRect.y + sh * _y,
                                      width, height);
            }
        }
        return rects;
    }
    public TransfromX[] ConvertRectToTransformX(Rect[] rects)
    {
        TransfromX[] trans = new TransfromX[rects.Length];
        float scale;
        for (int i = 0; i < rects.Length; i++)
        {
            scale = rects[i].height / _texHeight;
            trans[i] = new TransfromX()
            {
                position = new Vector3(rects[i].x + rects[i].width * .5f, _ScreenRect.height - rects[i].y - rects[i].height * .5f, 100),
                scale = new Vector3(scale, scale, scale)
            };
        }
        return trans;
    }
    private Texture2D _tex;
    public void Init()
    {
        _tex = Resources.LoadAll<Texture2D>("PhotoBox/tex")[0];
    }
    public TransfromX[] GetRandomTransformXLayout(int id, Rect rect)
    {
        float rx, ry;
        int num = id;
        switch (id)
        {
            case 11:
            case 15:
                num = 15;
                break;
            case 12:
            case 20:
                num = 20;
                break;
            case 13:
            case 30:
                num = 30;
                break;
            default:
                break;
        }
        TransfromX[] trans = new TransfromX[num];
        for (int i = 0; i < num; i++)
        {
            rx = Random.Range(rect.x, rect.xMax);
            ry = Random.Range(rect.y, rect.yMax);
            trans[i].position = new Vector3(rx, ry, 100);
            trans[i].scale = new Vector3(.01f, .01f, .01f);
        }
        return trans;
    }
}
