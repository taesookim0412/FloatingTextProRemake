﻿using Lovatto.FloatingTextAsset;
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
        private RaycastHit Ray;
        private FloatingTextObserverProps Props;
        private FloatingText FloatingText;
        private bl_FloatingText FloatingTextInstance;
        private Transform FloatingTextInstanceTransform;

        private FloatingTextSettings FloatingTextSettings;

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
                    Props.FloatingTextManager.FloatingTextSettingsDict.TryGetValue(FloatingTextType, out FloatingTextSettings))
                {
                    switch (FloatingTextType)
                    {
                        case FloatingTextType.LeagueOfLegends:
                            Props.FloatingTextManager.ChangeTextPrefab(floatingTextPrefab);

                            FloatingText = new FloatingText(
                                target: Ray.transform,
                                position: Ray.point,
                                text: $"{UnityEngine.Random.Range(500, 5000)}",
                                textColor: new Color(0.1462264f, 0.8359416f, 1f, 1),
                                positionOffset: Vector3.zero,
                                textSize: UnityEngine.Random.Range(30, 50),
                                reuseTimes: 0,
                                outlineSize: -1,
                                outlineColor: Color.clear,
                                FloatingTextSettings,
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
                FloatingTextInstance = Props.FloatingTextManager.GetTextInstance(FloatingText);
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
        LeagueOfLegends
    }
}
