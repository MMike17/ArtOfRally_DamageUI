﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

using static RepairsManagerUI;

namespace DamageUI
{
    public class DamagePanel : MonoBehaviour
    {
        static DamagePanel instance;

        Image body;
        Image leftSuspension;
        Image rightSuspension;
        Image radiator;
        GameObject engineAndTurboHolder;
        Image engine;
        Image turbo;
        Image gearbox;
        Image frontLeftWheel;
        Image frontRightWheel;
        Image backLeftWheel;
        Image backRightWheel;

        // TODO : Add CanvasGroup and fade animation to hide at start of game

        void Awake() => StartCoroutine(InitWhenReady());

        IEnumerator InitWhenReady()
        {
            GameEntryPoint entry = FindObjectOfType<GameEntryPoint>();

            if (entry == null)
            {
                Main.Log("Can't find entry point. Are you sure you're spawning the UI in the right scene ?");
                yield break;
            }

            FieldInfo testInfo = entry.GetType().GetField("eventManager", BindingFlags.Static | BindingFlags.NonPublic);
            yield return new WaitUntil(() => testInfo.GetValue(entry) != null);

            Init();
        }

        void Init()
        {
            Main.Try(() =>
            {
                instance = this;
                transform.localScale = Vector3.one * Main.settings.uiScale;

                // detect turbo
                PerformanceDamageManager manager = GameEntryPoint.EventManager.playerManager.performanceDamageManager;
                FieldInfo info = manager.GetType().GetField("DamageablePartsList", BindingFlags.NonPublic | BindingFlags.Instance);
                bool hasTurbo = false;

                // TODO : Turbo detection doesn't work properly
                foreach (PerformanceDamage part in (List<PerformanceDamage>)info.GetValue(manager))
                {
                    if (part is TurboPerformanceDamage)
                    {
                        hasTurbo = true;
                        break;
                    }
                }

                // engine swap
                engineAndTurboHolder = transform.GetChild(4).gameObject;
                engineAndTurboHolder.SetActive(hasTurbo);
                transform.GetChild(3).gameObject.SetActive(!hasTurbo);

                // get UI refs
                body = transform.GetChild(0).GetComponent<Image>();
                leftSuspension = transform.GetChild(1).GetChild(0).GetComponent<Image>();
                rightSuspension = transform.GetChild(1).GetChild(1).GetComponent<Image>();
                radiator = transform.GetChild(2).GetComponent<Image>();
                engine = (hasTurbo ? engineAndTurboHolder.transform.GetChild(0) : transform.GetChild(3)).GetComponent<Image>();
                turbo = engineAndTurboHolder.transform.GetChild(1).GetComponent<Image>();
                gearbox = transform.GetChild(5).GetComponent<Image>();

                Transform wheelsHodler = transform.GetChild(6);
                frontLeftWheel = wheelsHodler.GetChild(0).GetComponent<Image>();
                frontRightWheel = wheelsHodler.GetChild(1).GetComponent<Image>();
                backLeftWheel = wheelsHodler.GetChild(2).GetComponent<Image>();
                backRightWheel = wheelsHodler.GetChild(3).GetComponent<Image>();

                // set initial colors
                Color goodColor = Settings.GetColor(Main.settings.goodColor);
                Color badColor = Settings.GetColor(Main.settings.badColor);

                body.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.CLEANCAR));
                // TODO : How do I detect suspension state ? (check the aligment tilt)
                //leftSuspension.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.CLEANCAR));
                //rightSuspension.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.CLEANCAR));
                radiator.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.RADIATOR));
                engine.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.ENGINE));
                turbo.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.TURBO));
                gearbox.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.GEARBOX));
                // TODO : Get state of wheels
                //frontLeftWheel.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.CLEANCAR));
                //frontRightWheel.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.CLEANCAR));
                //backLeftWheel.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.CLEANCAR));
                //backRightWheel.color = LerpHSV(badColor, goodColor, manager.GetConditionOfPart(SystemToRepair.CLEANCAR));
            });
        }

        // TODO : Call this from the event when the car takes damage
        public void Damage(RepairsManagerUI.SystemToRepair part, float amount)
        {
            // check PerformanceDamageManager.GetConditionOfPart

            //
        }

        // TODO : Call this when the car gets punctured
        public void PunctureTire(WheelPos position)
        {
            // pass this through the call from patch
            // PlayerCollider.wheels

            Image selectedWheel = null;

            switch (position)
            {
                case WheelPos.FRONT_LEFT:
                    selectedWheel = frontLeftWheel;
                    break;

                case WheelPos.FRONT_RIGHT:
                    selectedWheel = frontRightWheel;
                    break;

                case WheelPos.REAR_LEFT:
                    selectedWheel = backLeftWheel;
                    break;

                case WheelPos.REAR_RIGHT:
                    selectedWheel = backRightWheel;
                    break;
            }

            selectedWheel.color = Settings.GetColor(Main.settings.badColor);
        }

        Color LerpHSV(Color a, Color b, float percent)
        {
            Color.RGBToHSV(a, out float aH, out float aS, out float aV);
            Color.RGBToHSV(b, out float bH, out float bS, out float bV);

            return Color.HSVToRGB(
                Mathf.Lerp(aH, bH, percent),
                Mathf.Lerp(aS, bS, percent),
                Mathf.Lerp(aV, bV, percent)
            );
        }

        public static void Refresh()
        {
            if (instance == null)
                return;

            instance.transform.localPosition = Settings.GetUIPosition();
            instance.Init();
        }
    }
}
