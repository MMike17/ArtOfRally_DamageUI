using System.Collections;
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

        // UI
        Image body;
        Image leftSuspension;
        Image rightSuspension;
        Image radiator;
        GameObject engineAndTurboHolder;
        Image engine;
        Image turbo;
        Image gearbox;
        Image[] wheels;

        // TODO : Add CanvasGroup and fade animation to hide at start of game

        PerformanceDamageManager manager;
        List<Wheel> wheelsData;
        DamageMap bodyMap;
        DamageMap suspensionsMap;
        DamageMap radiatorMap;
        DamageMap engineMap;
        DamageMap turboMap;
        DamageMap gearboxMap;

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

            manager = GameEntryPoint.EventManager.playerManager.performanceDamageManager;

            PlayerCollider player = FindObjectOfType<PlayerCollider>();
            FieldInfo info = player.GetType().GetField("wheels", BindingFlags.NonPublic | BindingFlags.Instance);
            wheelsData = (List<Wheel>)info.GetValue(player);

            GeneratePartsMaps();
            Init();
            RefreshValues();
        }

        void GeneratePartsMaps()
        {
            bodyMap = new DamageMap(manager, SystemToRepair.CLEANCAR, 1, 0);
            suspensionsMap = new DamageMap(manager, SystemToRepair.SUSPENSION, 0, 1);
            radiatorMap = new DamageMap(manager, SystemToRepair.RADIATOR, 0, 1);
            engineMap = new DamageMap(manager, SystemToRepair.ENGINE, 0, 1);
            turboMap = new DamageMap(manager, SystemToRepair.TURBO, 0, 1);
            gearboxMap = new DamageMap(manager, SystemToRepair.GEARBOX, 0, 1);
        }

        void Init()
        {
            Main.Try(() =>
            {
                instance = this;

                // detect turbo
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

                for (int i = 0; i < 4; i++)
                    wheels[i] = wheelsHodler.GetChild(i).GetComponent<Image>();
            });
        }

        void RefreshValues()
        {
            transform.localScale = Vector3.one * Main.settings.uiScale;

            // set initial colors
            Color goodColor = Settings.GetColor(Main.settings.goodColor);
            Color badColor = Settings.GetColor(Main.settings.badColor);

            body.color = LerpHSV(badColor, goodColor, bodyMap.GetStatus());

            // alignment is -0.05 <= 0 => 0.05
            float suspensionState = suspensionsMap.GetStatus() / 2;
            float alignment = GameModeManager.GetSeasonDataCurrentGameMode().SelectedCar.performancePartsCondition.SteeringAlignment;
            leftSuspension.color = LerpHSV(badColor, goodColor, (alignment < 0 ? alignment * -10 : 0) + suspensionState);
            rightSuspension.color = LerpHSV(badColor, goodColor, (alignment > 0 ? alignment * 10 : 0) + suspensionState);

            radiator.color = LerpHSV(badColor, goodColor, radiatorMap.GetStatus());
            engine.color = LerpHSV(badColor, goodColor, engineMap.GetStatus());
            turbo.color = LerpHSV(badColor, goodColor, turboMap.GetStatus());
            gearbox.color = LerpHSV(badColor, goodColor, gearboxMap.GetStatus());

            //for (int i = 0; i < wheels.Length; i++)
            //wheels[i].color = LerpHSV(badColor, goodColor, wheelsData[i].);
        }

        // TODO : Call this from the event when the car takes damage
        public void Damage(SystemToRepair part, float amount)
        {
            // check PerformanceDamageManager.GetConditionOfPart

            //
        }

        // TODO : Call this when the car gets punctured
        public void PunctureTire(WheelPos position) // int index
        {
            // pass this through the call from patch
            // PlayerCollider.wheels

            Image selectedWheel = null;

            //switch (position)
            //{
            //    case WheelPos.FRONT_LEFT:
            //        selectedWheel = frontLeftWheel;
            //        break;

            //    case WheelPos.FRONT_RIGHT:
            //        selectedWheel = frontRightWheel;
            //        break;

            //    case WheelPos.REAR_LEFT:
            //        selectedWheel = backLeftWheel;
            //        break;

            //    case WheelPos.REAR_RIGHT:
            //        selectedWheel = backRightWheel;
            //        break;
            //}

            selectedWheel.color = LerpHSV(Settings.GetColor(Main.settings.badColor), Settings.GetColor(Main.settings.goodColor), 0);
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

            instance.RefreshValues();
        }

        class DamageMap
        {
            float min;
            float max;
            SystemToRepair part;
            PerformanceDamageManager manager;

            public DamageMap(PerformanceDamageManager manager, SystemToRepair part, float min, float max)
            {
                this.manager = manager;
                this.part = part;
                this.min = min;
                this.max = max;
            }

            public float GetStatus() => Mathf.InverseLerp(min, max, manager.GetConditionOfPart(part));
        }
    }
}
