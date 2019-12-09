using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NHSAnimationObject
{
    private List<GameObject> _activePhotoObjects = new List<GameObject>();
    private TransfromX[] _beginTrans;
    private TransfromX[] _endTrans;
    private static NHSAnimationObject _inst;
    private float _animTime = 0;
    private int _frame = 0;
    private float _totalFrame = 0;
    public static NHSAnimationObject Inst
    {
        get
        {
            _inst = _inst == null ? new NHSAnimationObject() : _inst;
            return _inst;
        }
    }
    public void InitTransformAnim(List<GameObject> activePhotoObjects, TransfromX[] beginTrans, TransfromX[] endTrans)
    {
        _activePhotoObjects = activePhotoObjects;
        _beginTrans = beginTrans;
        _endTrans = endTrans;
        ResetTransformAnim();
    }
    public void ResetTransformAnim()
    {
        _animTime = 0;
    }
    public bool UpdateTransformAnim(float speed = 4f)
    {
        _animTime += Time.deltaTime * speed;
        for (int i = 0; i < _activePhotoObjects.Count; i++)
        {
            _activePhotoObjects[i].transform.position = Vector3.Lerp(_beginTrans[i].position, _endTrans[i].position, _animTime);
            _activePhotoObjects[i].transform.localScale = Vector3.Lerp(_beginTrans[i].scale, _endTrans[i].scale, _animTime);
        }
        if (_animTime > 1) { _animTime = 0; return false; } else return true;
    }
    private GameObject _SequenceAnimObj;
    private int _SequenceAnimFPS = 25;
    private int _SequenceAnimBeginFrame = 0;
    private Texture2D[] _Texture2DSequence;
    private Sprite[] _SpriteSequence;
    public void InitTextureSequenceAnim(GameObject SequenceShowObj, Texture2D[] textures, int fps, int begin = 0)
    {
        _SequenceAnimObj = SequenceShowObj;
        _Texture2DSequence = textures;
        _SequenceAnimFPS = fps;
        _SequenceAnimBeginFrame = begin;
    }
    public void UpdateTextureSequenceAnim()
    {
        _SequenceAnimObj.GetComponent<RawImage>().texture = UpdateSequenceAnim(_Texture2DSequence, _SequenceAnimFPS, _SequenceAnimBeginFrame);
    }

    private T UpdateSequenceAnim<T>(T[] sequence, int fps, int begin = 0)
    {
        return UpdateSequenceAnim(sequence, ref _frame, begin, sequence.Length - 1, fps, Time.deltaTime, ref _totalFrame);
    }
    private T UpdateSequenceAnim<T>(T[] sequence, ref int frame, int minFrame, int maxFrame, int fps, float deltaTime, ref float totalTime)
    {
        minFrame = Mathf.Min(minFrame, maxFrame);
        totalTime += deltaTime;
        if (totalTime >= (1f / fps - 0.01f))
        {
            frame = Mathf.Max(frame, minFrame);
            frame += 1;
            if (frame > maxFrame) frame = minFrame;
            totalTime = 0;
        }
        return sequence[frame];
    }
}
