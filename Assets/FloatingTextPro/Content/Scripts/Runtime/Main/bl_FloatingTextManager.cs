using System.Collections.Generic;
using UnityEngine;
using Lovatto.FloatingTextAsset;
using UnityEngine.Events;
using Assets.FloatingTextPro.Content.Scripts.Runtime.Main;
using System.Linq;
using System;
using Assets.Crafter.Components.Models;

public class bl_FloatingTextManager : MonoBehaviour
{
    public static readonly string[] SampleComments = new string[] { "Awesome Comment!", "Inside cornflakes", "You never hear about wind farmers...", "No luck, all skills", "Blow me!" , "Game Over."
        , "Everything went silky smooth", "You can start creating faster", "Side hustle came to an end.", "No contribution is ever too small.", "Share with the world", "Pupusas"};
    public int CurrentComment = 0;

    #region Public members
    [SerializeField]
    public FloatingTextSettings[] FloatingTextSettings;
    [SerializeField]
    public bl_FloatingText[] FloatingTextPrefabs;
    public int maxTextsInstances = 30;
    public UEvent onNewText;

    [Header("References")]
    public RectTransform parentReference;
    [SerializeField] private Camera m_playerCamera;
    public Camera PlayerCamera
    {
        get => m_playerCamera;
        set => m_playerCamera = value;
    }
    [SerializeField] private bl_FloatingText textTemplate = null;
    #endregion

    private List<bl_FloatingText> textsPool = new List<bl_FloatingText>();
    private Dictionary<Transform, bl_FloatingText> reusedTexts = new Dictionary<Transform, bl_FloatingText>();
    private int currentPoolID = 0;
    private FloatingTextObserverProps FloatingTextObserverProps;
    private List<FloatingTextObserver> FloatingTextObservers = new List<FloatingTextObserver>();
    [HideInInspector]
    public Dictionary<FloatingTextType, bl_FloatingText> FloatingTextPrefabsDict;
    [HideInInspector]
    public Dictionary<FloatingTextType, FloatingTextSettings> FloatingTextSettingsDict;
    [HideInInspector]
    public FloatingTextType SelectedFloatingTextType;
    [HideInInspector]
    public Dictionary<FloatingTextType, PoolBagDco<bl_FloatingText>> FloatingTextInstancePools;
    [System.Serializable]
    public class UEvent : UnityEvent { }

    public void OnEnable()
    {
        FloatingTextObserverProps = new FloatingTextObserverProps(this);

        Dictionary<FloatingTextType, PoolBagDco<bl_FloatingText>> floatingTextInstancePools = new Dictionary<FloatingTextType, PoolBagDco<bl_FloatingText>>(FloatingTextObserver.FloatingTextTypeLength);
        Dictionary<FloatingTextType, bl_FloatingText>  floatingTextPrefabsDict = new Dictionary<FloatingTextType, bl_FloatingText>(FloatingTextObserver.FloatingTextTypeLength);
        for (int i = 0; i < FloatingTextPrefabs.Length; i++)
        {
            string prefabName = FloatingTextPrefabs[i].name;
            int firstCharIdx = prefabName.IndexOf('[') + 1;
            string textType = prefabName.Substring(firstCharIdx, prefabName.IndexOf(']') - firstCharIdx);

            List<FloatingTextType> prefabTypes = new List<FloatingTextType>();
            switch (textType)
            {
                // [0]
                case "Apex":
                    prefabTypes.Add(FloatingTextType.Basic);
                    break;
                // [1]
                case "Fortnite":
                    prefabTypes.Add(FloatingTextType.Fortnite);
                    prefabTypes.Add(FloatingTextType.RandomText);
                    break;
                // [2]
                case "The Division":
                    prefabTypes.Add(FloatingTextType.TomClansysTheDivision);
                    prefabTypes.Add(FloatingTextType.CustomText1);
                    break;
                // [3]
                case "League Of Legends":
                    prefabTypes.Add(FloatingTextType.LeagueOfLegends);
                    prefabTypes.Add(FloatingTextType.ShakeText);
                    prefabTypes.Add(FloatingTextType.DropText);
                    break;
                // [4]
                case "Candy Crush":
                    prefabTypes.Add(FloatingTextType.CandyCrush);
                    break;
                // [5]
                case "W Background":
                    prefabTypes.Add(FloatingTextType.SlideText);
                    break;
                default:
                    //???
                    continue;
            }
            foreach (FloatingTextType floatingTextType in prefabTypes)
            {
                floatingTextPrefabsDict[floatingTextType] = FloatingTextPrefabs[i];
                floatingTextInstancePools[floatingTextType] = new PoolBagDco<bl_FloatingText>(FloatingTextPrefabs[i],
                    capacity: 30);
            }
        }
        FloatingTextPrefabsDict = floatingTextPrefabsDict;
        FloatingTextInstancePools = floatingTextInstancePools;

        Dictionary<FloatingTextType, FloatingTextSettings> floatingTextSettingsDict = new Dictionary<FloatingTextType, FloatingTextSettings>(FloatingTextObserver.FloatingTextTypeLength);
        for (int i = 0; i < FloatingTextSettings.Length; i++)
        {
            string settingsName = FloatingTextSettings[i].name;
            int firstCharIdx = settingsName.IndexOf('[') + 1;
            string textType = settingsName.Substring(firstCharIdx, settingsName.IndexOf(']') - firstCharIdx);

            List<FloatingTextType> prefabTypes = new List<FloatingTextType>();
            switch (textType)
            {
                //case "Default":
                //    //prefabTypes.Add()
                //    break;
                case "Apex Legends":
                    prefabTypes.Add(FloatingTextType.Basic);
                    break;
                case "Fortnite":
                    prefabTypes.Add(FloatingTextType.Fortnite);
                    break;
                case "The Division":
                    prefabTypes.Add(FloatingTextType.TomClansysTheDivision);
                    break;
                case "League Of Legends":
                    prefabTypes.Add(FloatingTextType.LeagueOfLegends);
                    break;
                case "Candy Crush":
                    prefabTypes.Add(FloatingTextType.CandyCrush);
                    break;
                case "Comments":
                    prefabTypes.Add(FloatingTextType.CustomText1);
                    break;
                case "Shake":
                    prefabTypes.Add(FloatingTextType.ShakeText);
                    break;
                case "Random":
                    prefabTypes.Add(FloatingTextType.RandomText);
                    break;
                case "Left Slide":
                    prefabTypes.Add(FloatingTextType.SlideText);
                    break;
                case "Drop":
                    prefabTypes.Add(FloatingTextType.DropText);
                    break;
                default:
                    // Default is not added.
                    // ???
                    continue;
            }

            foreach (FloatingTextType floatingTextType in prefabTypes)
            {
                floatingTextSettingsDict[floatingTextType] = FloatingTextSettings[i];
            }
        }
        FloatingTextSettingsDict = floatingTextSettingsDict;
    }

    public string IncrementSampleComment()
    {
        CurrentComment = (CurrentComment + 1) % bl_FloatingTextManager.SampleComments.Length;

        return SampleComments[CurrentComment];
    }

    private void Update()
    {
        if (FloatingTextObservers.Count > 0)
        {
            bool foundRemove = false;
            for (int i = 0; i < FloatingTextObservers.Count; i++)
            {
                FloatingTextObservers[i].OnUpdate();
                if (!foundRemove && FloatingTextObservers[i].ObserverStatus == ObserverStatus.Remove)
                {
                    foundRemove = true;
                }
            }
            // lazy way of removing for remake prototype
            if (foundRemove)
            {
                FloatingTextObservers = FloatingTextObservers.Where(observer => observer.ObserverStatus != ObserverStatus.Remove).ToList();
            }
        }
    }
    public void AddFloatingTextObserver(FloatingTextType floatingTextType, Vector3 hitPositionWorld)
    {
        FloatingTextObservers.Add(new FloatingTextObserver(floatingTextType, hitPositionWorld, FloatingTextObserverProps));
    }

    /// <summary>
    /// Instance a new floating text with the given data
    /// </summary>
    /// <param name="data"></param>
    /// 
    public bl_FloatingText InstanceFloatingText(FloatingText data)
    {
        bl_FloatingText floatingText = GetTextInstance(data);
        floatingText.Set(data);
        onNewText?.Invoke();
        return floatingText;
    }

    /// <summary>
    /// Pool text instance to instantiate only the required texts
    /// </summary>
    /// <returns></returns>
    public bl_FloatingText GetTextInstance(FloatingText data)
    {
        if((data.ReuseTimes > 0 || data.ReuseTimes == -2) && data.Target != null)
        {
            if (reusedTexts.ContainsKey(data.Target))
            {
                var t = reusedTexts[data.Target];
                if (t.UseTimes > 0)
                {
                    t.UseTimes--;
                    return t;
                }
                else if (t.UseTimes == -2) return t;
            }
        }

        if (textsPool.Count >= maxTextsInstances)
        {
            var t = textsPool[currentPoolID];
            currentPoolID = (currentPoolID + 1) % maxTextsInstances;
            return t;
        }
        else
        {
            for (int i = 0; i < textsPool.Count; i++)
            {
                if (!textsPool[i].IsActive()) return textsPool[i];
            }

            var go = Instantiate(textTemplate.gameObject) as GameObject;
            go.transform.SetParent(parentReference, false);

            var script = go.GetComponent<bl_FloatingText>();
            textsPool.Add(script);
            return script;
        }
    }

    /// <summary>
    /// Change the floating text prefab
    /// this will clear all the pool instances.
    /// </summary>
    public void ChangeTextPrefab(bl_FloatingText newPrefab)
    {
        if (newPrefab == textTemplate) return;

        textsPool.ForEach(x =>
        {
            if (x != null) Destroy(x.gameObject);
        });
        textsPool = new List<bl_FloatingText>();
        reusedTexts = new Dictionary<Transform, bl_FloatingText>();
        currentPoolID = 0;
        textTemplate = newPrefab;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="floatingText"></param>
    public void AddToReused(bl_FloatingText floatingText, FloatingText data)
    {
        if (reusedTexts.ContainsKey(data.Target)) return;
        reusedTexts.Add(data.Target, floatingText);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="floatingText"></param>
    public void RemoveFromReused(FloatingText data)
    {
        if (!reusedTexts.ContainsKey(data.Target)) return;
        reusedTexts.Remove(data.Target);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wPosition"></param>
    /// <returns></returns>
    public static Vector3 GetScreenPositionFromWorldPosition(Vector3 wPosition)
    {
        var cam = Instance.PlayerCamera;
        if (cam == null) cam = Camera.current;
        if (cam == null) return Vector3.zero;

        return cam.WorldToScreenPoint(wPosition);
    }

    private Canvas m_canvas;
    public Canvas UsedCanvas
    {
        get
        {
            if (m_canvas == null) m_canvas = transform.GetComponentInParent<Canvas>();
            return m_canvas;
        }
    }

    private static bl_FloatingTextManager _instance;
    public static bl_FloatingTextManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<bl_FloatingTextManager>();
            return _instance;
        }
    }
}