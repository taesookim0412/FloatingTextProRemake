using Assets.Crafter.Components.Models;
using Lovatto.FloatingTextAsset;
using System;
using System.Collections;
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
        private Vector3 HitPositionWorld;
        private FloatingTextObserverProps Props;
        private FloatingText FloatingText;
        private bl_FloatingText FloatingTextInstance;
        private Transform FloatingTextInstanceTransform;

        private FloatingTextSettings FloatingTextSettings;
        private PoolBagDco<bl_FloatingText> PoolBag;

        private Vector3 TargetScreenPosition;
        private float SequenceTime;
        private float PercentageAccumulate;
        private float FloatDurationPercentage;
        private float FinishDurationPercentage;
        private float FinishSequenceDivMultiplier;
        private float BaseAlpha;
        private float ElapsedDuration = 0f;
        private Vector3 RectPosition = Vector3.zero;

        private bool FloatingTextSet = false;
        private bool StartSequenceCompleted = false;
        private bool StaticSequenceCompleted = false;
        private bool FloatSequenceCompleted = false;
        private bool FinishSequenceCompleted = false;

        public FloatingTextObserver(FloatingTextType floatingTextType, Vector3 hitPositionWorld, FloatingTextObserverProps floatingTextObserverProps)
        {
            FloatingTextType = floatingTextType;
            HitPositionWorld = hitPositionWorld;
            Props = floatingTextObserverProps;
        }

        public void OnUpdate()
        {
            if (!FloatingTextSet)
            {
                if (Props.FloatingTextManager.FloatingTextPrefabsDict.TryGetValue(FloatingTextType, out bl_FloatingText floatingTextPrefab) && 
                    Props.FloatingTextManager.FloatingTextSettingsDict.TryGetValue(FloatingTextType, out FloatingTextSettings) &&
                    Props.FloatingTextManager.FloatingTextInstancePools.TryGetValue(FloatingTextType, out PoolBag))
                {
                    // base font sizes:
                    // Apex 38
                    // Fortnite 50
                    // The Division 38
                    // LoL 38
                    // Candy Crush 65
                    // W Background 35
                    switch (FloatingTextType)
                    {
                        case FloatingTextType.Basic:
                            //basefont: 38
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: $"{UnityEngine.Random.Range(10, 150)}",
                                textColor: Color.white,
                                positionOffset: Vector3.right,
                                textSize: 40,
                                outlineSize: 2.5f,
                                outlineColor: new Color(1f, 0f, 0f, 0.7f),
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true
                                );
                            FloatingText.DontRewindOnReuse();
                            break;
                        case FloatingTextType.Fortnite:
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: $"{UnityEngine.Random.Range(10, 150)}",
                                textColor: Color.white,
                                positionOffset: Vector3.zero,
                                textSize: 50,
                                outlineSize: 3,
                                outlineColor: new Color(0, 0, 0, 0.7f),
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true
                                );
                            break;
                        case FloatingTextType.TomClansysTheDivision:
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: $"{UnityEngine.Random.Range(10, 90)}",
                                textColor: Color.white,
                                positionOffset: Vector3.zero,
                                textSize: 38,
                                outlineSize: 1,
                                outlineColor: new Color(0, 0, 0, 1f),
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true);
                            break;
                        case FloatingTextType.LeagueOfLegends:
                            // baseFont: 38
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: $"{UnityEngine.Random.Range(500, 5000)}",
                                textColor: new Color(0.1462264f, 0.8359416f, 1f, 1),
                                positionOffset: Vector3.zero,
                                textSize: 38 + UnityEngine.Random.Range(-10, 10),
                                outlineSize: -1,
                                outlineColor: Color.clear,
                                FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true);
                            break;
                        case FloatingTextType.CandyCrush:
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: $"{UnityEngine.Random.Range(10, 90)}",
                                textColor: new Color(0.3050465f, 1, 0.3050465f, 1),
                                positionOffset: Vector3.zero,
                                textSize: 65,
                                outlineSize: -1,
                                outlineColor: new Color(0.0993236f, 0.2264151f, 0.1272217f, 1),
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.None,
                                isRemade: true);
                            break;
                        case FloatingTextType.CustomText1:
                            int newCommentIndex = (Props.FloatingTextManager.CurrentComment + 1) % bl_FloatingTextManager.SampleComments.Length;
                            Props.FloatingTextManager.CurrentComment = newCommentIndex;
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: Props.FloatingTextManager.IncrementSampleComment(),
                                textColor: Color.white,
                                positionOffset: Vector3.up * 1.5f,
                                textSize: 38,
                                outlineSize: 1,
                                outlineColor: Color.black,
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true);
                            break;
                        case FloatingTextType.RandomText:
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: $"{UnityEngine.Random.Range(10, 99)}",
                                textColor: new Color(UnityEngine.Random.value,
                                    UnityEngine.Random.value, UnityEngine.Random.value, 1f),
                                positionOffset: Vector3.zero,
                                textSize: 50 + UnityEngine.Random.Range(-15, 15),
                                outlineSize: 1,
                                outlineColor: Color.black,
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true);
                            break;
                        case FloatingTextType.SlideText:
                            int newCommentIndex2 = (Props.FloatingTextManager.CurrentComment + 1) % bl_FloatingTextManager.SampleComments.Length;
                            Props.FloatingTextManager.CurrentComment = newCommentIndex2;
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: Props.FloatingTextManager.IncrementSampleComment(),
                                textColor: Color.white,
                                positionOffset: Vector3.up * 1.5f,
                                textSize: 35,
                                outlineSize: -1,
                                outlineColor: Color.clear,
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true);
                            break;
                        case FloatingTextType.ShakeText:
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: $"{UnityEngine.Random.Range(10, 99)}",
                                textColor: Color.black,
                                positionOffset: Vector3.zero,
                                textSize: 38,
                                outlineSize: 0,
                                outlineColor: Color.clear,
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true);
                            break;
                        case FloatingTextType.DropText:
                            FloatingText = new FloatingText(
                                position: HitPositionWorld,
                                text: $"{UnityEngine.Random.Range(10, 99)}",
                                textColor: Color.white,
                                positionOffset: Vector3.zero,
                                textSize: 38,
                                outlineSize: 1,
                                outlineColor: Color.clear,
                                settings: FloatingTextSettings,
                                flags: FloatingTextFlags.StickAtOriginPosition,
                                isRemade: true);
                            FloatingText.InvertHorizontalDirectionRandomly();
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
                FloatingTextInstance = PoolBag.InstantiatePooled(Props.FloatingTextManager.parentReference);

                FloatingTextInstanceTransform = FloatingTextInstance.transform;
                FloatingTextInstance.Set_Remade(FloatingText);

                //set the start position
                Vector3 targetScreenPosition = bl_FloatingTextManager.GetScreenPositionFromWorldPosition(FloatingText.GetPosition());

                FloatingTextInstanceTransform.position = targetScreenPosition;
                FloatingText.InternalOnly.InitRectPosition = targetScreenPosition;
                TargetScreenPosition = targetScreenPosition;

                SequenceTime = FloatingTextSettings.StartSequenceDuration + FloatingTextSettings.FloatingDuration + FloatingTextSettings.FinishSequenceDuration;
                PercentageAccumulate = FloatingTextSettings.StartSequenceDuration / SequenceTime;
                FloatDurationPercentage = FloatingTextSettings.FloatingDuration / SequenceTime;
                FinishDurationPercentage = FloatingTextSettings.FinishSequenceDuration / SequenceTime;
                if (FloatingTextSettings.FinishSequenceDuration > 0f)
                {
                    FinishSequenceDivMultiplier = 1 / FloatingTextSettings.FinishSequenceDuration;
                }

                BaseAlpha = FloatingTextInstance.canvasGroup.alpha;
                
                // skips onNewText?.Invoke();
            }

            UpdateFloatingText();

            if (!FloatingTextInstance.gameObject.activeSelf)
            {
                ObserverStatus = ObserverStatus.Remove;
            }
        }
        private void UpdateFloatingText()
        {
            if (!StartSequenceCompleted)
            {
                bool completeStartSequence = false;
                if (FloatingTextSettings.StartSequenceDuration > 0f)
                {
                    ElapsedDuration += Time.deltaTime / FloatingTextSettings.StartSequenceDuration;
                    if (ElapsedDuration < 1f)
                    {
                        FloatingTextInstance.canvasGroup.alpha = FloatingTextSettings.FadeInCurve.Evaluate(ElapsedDuration);

                        float startScaleCurveValue = FloatingTextSettings.StartScaleCurve.Evaluate(ElapsedDuration);
                        FloatingTextInstanceTransform.localScale = new Vector3(startScaleCurveValue, startScaleCurveValue, startScaleCurveValue);

                        float movePercentage;
                        if (FloatingTextSettings.floatingDirectionType == FloatingTextSettings.FloatingType.CurveAxis)
                        {
                            movePercentage = PercentageAccumulate * ElapsedDuration;
                        }
                        else
                        {
                            movePercentage = 0f;
                        }

                        // The original is probably missing StartStaticDuration.
                        //if (FloatingTextSettings.StaticDuration <= 0f)
                        MoveFloatingText(movePercentage);
                    }
                    else
                    {
                        completeStartSequence = true;
                    }
                }
                else
                {
                    completeStartSequence = true;
                }
                if (completeStartSequence)
                {
                    ElapsedDuration = 0f;
                    StartSequenceCompleted = true;
                }
            }
            else if (!StaticSequenceCompleted)
            {
                bool completeStaticSequence = false;
                if (FloatingTextSettings.StaticDuration > 0f)
                {
                    ElapsedDuration += Time.deltaTime / FloatingTextSettings.StaticDuration;
                    if (FloatingText.Flags.HasFlag(FloatingTextFlags.StickAtOriginPosition))
                    {
                        if (ElapsedDuration < 1f)
                        {
                            TargetScreenPosition = bl_FloatingTextManager.GetScreenPositionFromWorldPosition(FloatingText.GetPosition());
                            FloatingTextInstanceTransform.position = TargetScreenPosition;
                        }
                        else
                        {
                            completeStaticSequence = true;
                        }
                    }
                    else
                    {
                        if (ElapsedDuration > FloatingTextSettings.StaticDuration)
                        {
                            completeStaticSequence = true;
                        }
                    }
                }
                else
                {
                    completeStaticSequence = true;
                }
                if (completeStaticSequence)
                {
                    ElapsedDuration = 0f;
                    StaticSequenceCompleted = true;
                }
            }
            else if (!FloatSequenceCompleted)
            {
                bool completeFloatSequence = false;

                if (FloatingTextSettings.FloatingDuration > 0)
                {
                    ElapsedDuration += Time.deltaTime / FloatingTextSettings.FloatingDuration;

                    if (ElapsedDuration < 1f)
                    {
                        FloatingTextInstance.canvasGroup.alpha = FloatingTextSettings.FadeOverLifeTime.Evaluate(ElapsedDuration);

                        float movePercentage;
                        if (FloatingTextSettings.floatingDirectionType == FloatingTextSettings.FloatingType.CurveAxis)
                        {
                            if (FloatingTextSettings.StaticDuration > 0f)
                            {
                                movePercentage = PercentageAccumulate;
                            }
                            else
                            {
                                movePercentage = FloatDurationPercentage * ElapsedDuration;
                            }
                        }
                        else
                        {
                            movePercentage = 0f;
                        }
                        MoveFloatingText(movePercentage);
                    }
                    else
                    {
                        completeFloatSequence = true;
                    }
                }
                else
                {
                    completeFloatSequence = true;
                }
                if (completeFloatSequence)
                {
                    ElapsedDuration = 0f;
                    FloatSequenceCompleted = true;
                }
            }
            else if (!FinishSequenceCompleted)
            {
                //?
                //TODO: Remove UseTimes?

                bool completeFinishSequence = false;
                if (FloatingTextSettings.FinishSequenceDuration > 0)
                {
                    ElapsedDuration += Time.deltaTime * FinishSequenceDivMultiplier;
                    if (ElapsedDuration < 1f)
                    {
                        FloatingTextInstance.canvasGroup.alpha = BaseAlpha * FloatingTextSettings.FadeOutCurve.Evaluate(ElapsedDuration);
                        float finishScaleCurveScale = FloatingTextSettings.FinishScaleCurve.Evaluate(ElapsedDuration);

                        FloatingTextInstanceTransform.localScale = new Vector3(finishScaleCurveScale, finishScaleCurveScale, finishScaleCurveScale);

                        float movePercentage;
                        if (FloatingTextSettings.floatingDirectionType == FloatingTextSettings.FloatingType.CurveAxis)
                        {
                            movePercentage = PercentageAccumulate + (FinishDurationPercentage * ElapsedDuration);
                        }
                        else
                        {
                            movePercentage = 0f;
                        }

                        MoveFloatingText(movePercentage);
                    }
                    else
                    {
                        completeFinishSequence = true;
                    }
                }
                else
                {
                    completeFinishSequence = true;
                }
                if (completeFinishSequence)
                {
                    ElapsedDuration = 0f;
                    FinishSequenceCompleted = true;

                    //???
                    //if (FloatingTextInstance.UseTimes == -2)
                    //{
                    //    Props.FloatingTextManager.RemoveFromReused(FloatingText);
                    //}
                    //FloatingTextInstance.UseTimes = -1;
                    PoolBag.ReturnPooled(FloatingTextInstance);
                    FloatingTextInstance.gameObject.SetActive(false);
                }
            }
        }

        private void MoveFloatingText(float percentage)
        {
            if (FloatingText.Flags.HasFlag(FloatingTextFlags.StickAtOriginPosition))
            {
                if ((Time.frameCount % 2) == 0)
                {
                    TargetScreenPosition = bl_FloatingTextManager.GetScreenPositionFromWorldPosition(FloatingText.GetPosition());
                }
            }

            switch (FloatingText.Settings.floatingDirectionType)
            {
                case FloatingTextSettings.FloatingType.FixedDirection:
                    RectPosition.x += FloatingTextSettings.FloatDirection.x;
                    RectPosition.y += FloatingTextSettings.FloatDirection.y;
                    break;
                case FloatingTextSettings.FloatingType.CurveAxis:
                    RectPosition.x = FloatingTextSettings.XFloatCurve.Evaluate(percentage);
                    RectPosition.y = FloatingTextSettings.YFloatCurve.Evaluate(percentage);
                    break;
                case FloatingTextSettings.FloatingType.RandomizeAxis:
                    RectPosition.x += FloatingText.InternalOnly.FloatDirection.x;
                    RectPosition.y += FloatingText.InternalOnly.FloatDirection.y;
                    break;
            }

            if (FloatingText.InternalOnly.InvertHorizontalDirection)
            {
                RectPosition.x *= -1f;
            }

            FloatingTextInstanceTransform.position = TargetScreenPosition + RectPosition;
        }
    }

    public enum ObserverStatus
    {
        Active,
        Remove
    }
    public enum FloatingTextType
    {
        Basic,
        Fortnite,
        TomClansysTheDivision,
        LeagueOfLegends,
        CandyCrush,
        CustomText1,
        RandomText,
        SlideText,
        ShakeText,
        DropText
    }
}
