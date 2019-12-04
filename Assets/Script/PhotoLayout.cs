using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoLayout
{
    private float _texWidth, _texHeight, _ratio;
    private Rect[] _rects;
    public PhotoLayout(float width, float height)
    {
        _texWidth = width;
        _texHeight = height;
        _ratio = _texWidth / _texHeight;
    }
    public Rect[] GetPhotoesLayout(Rect outRect, int total, float titleX, float titleY, float marginX, float marginY)
    {
        Rect[] rects = new Rect[total] ;
        float sw = (outRect.width + marginX) / (titleX + marginX);
        float w = sw - marginX;
        float h = w / _ratio;
        float sh = h + marginY;
        int i = 0;
        for (int _y = 0; _y < titleY; _y++)
        {
            for (int _x = 0; _x < titleX; _x++)
            {
                if ((_x * _y) >= total) break;
                rects[i++] = new Rect(outRect.x + sw * _x, outRect.y + sh * _y, w, h);
            }
        }
        return rects;
    }
    private Texture2D _tex;
    public void init()
    {
        _tex = Resources.LoadAll<Texture2D>("PhotoBox")[0];
        _rects = GetPhotoesLayout(new Rect(1000, 200, 1900, 300), 20, 10, 2, 0, 0);
    }
    public void test()
    {
        foreach (Rect _rc in _rects)
        {
            GUI.DrawTexture(_rc, _tex);
        }
    }
}
