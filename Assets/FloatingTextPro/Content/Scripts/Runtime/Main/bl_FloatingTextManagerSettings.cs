using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "FloatingTextManagerSettings", menuName = "Lovatto/Floating Text/Manager Settings")]
public class bl_FloatingTextManagerSettings : ScriptableObject
{
    [Header("Presents")]
    public FTSettings[] floatingTextSettings;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="settingName"></param>
    /// <returns></returns>
    public FloatingTextSettings GetSettings(string settingName)
    {
        for (int i = 0; i < floatingTextSettings.Length; i++)
        {
            if (floatingTextSettings[i].Name == settingName) return floatingTextSettings[i].Settings;
        }
        return null;
    }

    [System.Serializable]
    public class FTSettings
    {
        public string Name;
        public FloatingTextSettings Settings;
    }

    private static bl_FloatingTextManagerSettings m_Data;
    public static bl_FloatingTextManagerSettings Instance
    {
        get
        {
            if (m_Data == null)
            {
                m_Data = Resources.Load("FloatingTextManagerSettings", typeof(bl_FloatingTextManagerSettings)) as bl_FloatingTextManagerSettings;
            }
            return m_Data;
        }
    }
}