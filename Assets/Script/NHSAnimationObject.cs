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
    public void UpdateTransformAnim(float speed = 5f)
    {
        _animTime += Time.deltaTime * speed;
        for (int i = 0; i < _activePhotoObjects.Count; i++)
        {
            _activePhotoObjects[i].transform.position = Vector3.Lerp(_beginTrans[i].position, _endTrans[i].position, _animTime);
            _activePhotoObjects[i].transform.position = Vector3.Lerp(_beginTrans[i].position, _endTrans[i].position, _animTime);
        }
    }
    private GameObject _targetSpriteObj;
    private Sprite[] _spriteSequence;
    private int _spriteSequenceAnimFPS = 25;
    private int _spriteSequenceAnimBeginFrame = 0;
    public void InitSequenceAnim(GameObject targetSpriteObj, Sprite[] sprites, int fps, int begin = 0)
    {
        _targetSpriteObj = targetSpriteObj;
        _spriteSequence = sprites;
        _spriteSequenceAnimFPS = fps;
        _spriteSequenceAnimBeginFrame = begin;
    }
    public void UpdateSequenceAnim()
    {
        _targetSpriteObj.GetComponent<Image>().sprite = UpdateSequenceAnim(_spriteSequence, _spriteSequenceAnimFPS, _spriteSequenceAnimBeginFrame);
    }
    private Sprite UpdateSequenceAnim(Sprite[] sprites, int fps, int begin = 0)
    {
        return UpdateSequenceAnim(sprites, ref _frame, begin, sprites.Length - 1, fps, Time.deltaTime, ref _totalFrame);
    }
    private Sprite UpdateSequenceAnim(Sprite[] sprites, ref int frame, int minFrame, int maxFrame, int fps, float deltaTime, ref float totalTime)
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
}
