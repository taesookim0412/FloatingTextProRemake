using Lovatto.FloatingTextAsset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.FloatingTextPro.Content.Scripts.Runtime.Main
{
    public class FloatingTextObserverProps
    {
        public bl_FloatingTextManager FloatingTextManager;

        public FloatingTextObserverProps(bl_FloatingTextManager floatingTextManager)
        {
            FloatingTextManager = floatingTextManager;
        }
    }
    public class FloatingTextObserver
    {

        public ObserverStatus ObserverStatus = ObserverStatus.Active;
        private FloatingTextType FloatingTextType;
        private RaycastHit Ray;
        private FloatingTextObserverProps Props;
        private FloatingText FloatingText;
        private bl_FloatingText FloatingTextInstance;
        private bool FloatingTextSet = false;

        public FloatingTextObserver(FloatingTextType floatingTextType, RaycastHit ray, FloatingTextObserverProps floatingTextObserverProps)
        {
            FloatingTextType = floatingTextType;
            Ray = ray;
            Props = floatingTextObserverProps;
        }

        public void OnUpdate()
        {
            if (!FloatingTextSet)
            {
                if (Props.FloatingTextManager.FloatingTextPrefabsDict.TryGetValue(FloatingTextType, out bl_FloatingText floatingTextPrefab) && 
                    Props.FloatingTextManager.FloatingTextSettingsDict.TryGetValue(FloatingTextType, out FloatingTextSettings floatingTextSettings))
                {
                    switch (FloatingTextType)
                    {
                        case FloatingTextType.LeagueOfLegends:
                            bl_FloatingTextManager.Instance.ChangeTextPrefab(floatingTextPrefab);

                            FloatingText = new FloatingText(
                                target: Ray.transform,
                                position: Ray.point,
                                text: $"{UnityEngine.Random.Range(500, 5000)}",
                                textColor: new Color(0.1462264f, 0.8359416f, 1f, 1),
                                positionOffset: Vector3.zero,
                                extraTextSize: UnityEngine.Random.Range(-10, 10),
                                reuseTimes: 0,
                                outlineSize: -1,
                                outlineColor: Color.clear,
                                floatingTextSettings,
                                finishCallback: null,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                internalOnly: new FloatingText.InternalProps(),
                                isRemade: true);
                            break;
                        default:
                            ObserverStatus = ObserverStatus.Remove;
                            return;
                    }
                }
                else
                {
                    ObserverStatus = ObserverStatus.Remove;
                    return;
                }
                FloatingTextSet = true;
                FloatingTextInstance = FloatingText.Show();
            }
            if (!FloatingTextInstance.gameObject.activeSelf)
            {
                ObserverStatus = ObserverStatus.Remove;
            }
        }
    }

    public enum ObserverStatus
    {
        Active,
        Remove
    }
    public enum FloatingTextType
    {
        LeagueOfLegends
    }
}
