using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NHSObjectsPool
{
    public static NHSObjectsPool Inst
    {
        get
        {
            _inst = _inst == null ? new NHSObjectsPool() : _inst;
            return _inst;
        }
    }
    private static NHSObjectsPool _inst;
    private Queue<GameObject> _objPool = new Queue<GameObject>();
    private GameObject _photoBoxPrefab;
    private Texture2D[] _regionTextures;
    private Dictionary<int, Texture2D> _photos;
    private List<GameObject> _activePhotoObjects = new List<GameObject>();
    public List<GameObject> ActivePhotoObjects
    {
        get
        {
            return _activePhotoObjects;
        }
    }
    public void Init(GameObject photoBoxPrefab, Dictionary<int, Texture2D> photos, Texture2D[] regionTextures, int count = 30)
    {
        _photoBoxPrefab = photoBoxPrefab;
        _photos = photos;
        _regionTextures = regionTextures;
        GameObject PhotosPoolNode = GameObject.Find("PhotosPool") == null ? new GameObject("PhotosPool") : GameObject.Find("PhotosPool");
        for (int i = 0; i < count; i++)
        {
            GameObject _obj = GameObject.Instantiate(photoBoxPrefab);
            _obj.transform.parent = PhotosPoolNode.transform;
            _obj.name = "PhotoBox" + i.ToString("D2");
            //_obj.transform.position = new Vector3(0, 0, 100);
            _obj.SetActive(false);
            _objPool.Enqueue(_obj);
        }
    }
    public GameObject Create(int regionID, int employeeID, string employeeName, TransfromX trans)
    {
        GameObject obj = Create(regionID, employeeID, employeeName);
        if (obj != null)
        {
            obj.transform.position = trans.position;
            obj.transform.localScale = trans.scale;
        }
        return obj;
    }
    public GameObject Create(int regionID, int employeeID, string employeeName)
    {
        GameObject obj = null;

        if (!_photos.ContainsKey(employeeID)) employeeID = 0;

        if (_objPool.Count > 0)
        {
            obj = _objPool.Dequeue();
            obj.transform.Find("canvas").GetComponent<Canvas>().worldCamera = Camera.main;
            obj.transform.Find("canvas/regionBG").GetComponent<RawImage>().texture = _regionTextures[regionID];
            obj.transform.Find("canvas/regionBG/mask/photo").GetComponent<RawImage>().texture = _photos[employeeID];
            obj.transform.Find("canvas/regionBG/id").GetComponent<Text>().text = employeeID.ToString();
            obj.transform.Find("canvas/regionBG/name").GetComponent<Text>().text = employeeName;
            obj.SetActive(true);
            _activePhotoObjects.Add(obj);
        }

        return obj;
    }
    private void Delete(GameObject obj)
    {
        obj.SetActive(false);
        _objPool.Enqueue(obj);
    }
    public void Destroy()
    {
        foreach (GameObject obj in _activePhotoObjects)
        {
            Delete(obj);
        }
        _activePhotoObjects.Clear();
    }
}
