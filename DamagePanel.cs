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

        PerformanceDamageManager manager;
        List<Wheel> wheelsData;
        CanvasGroup group;
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
            bodyMap = new DamageMap(manager, SystemToRepair.CLEANCAR, 0, 1);
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

                group = gameObject.AddComponent<CanvasGroup>();
                group.alpha = 0;
                group.blocksRaycasts = false;

                bool hasTurbo = CarManager.GetCarStatsForCar(GameModeManager.GetSeasonDataCurrentGameMode().SelectedCar)
                    .Aspiration != CarSpecs.EngineAspiration.NATURAL;

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
                wheels = new Image[4];

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
            UpdateSuspensions();
            radiator.color = LerpHSV(badColor, goodColor, radiatorMap.GetStatus());
            engine.color = LerpHSV(badColor, goodColor, engineMap.GetStatus());
            turbo.color = LerpHSV(badColor, goodColor, turboMap.GetStatus());
            gearbox.color = LerpHSV(badColor, goodColor, gearboxMap.GetStatus());

            for (int i = 0; i < wheels.Length; i++)
                wheels[i].color = LerpHSV(badColor, goodColor, wheelsData[i].tirePuncture ? 0 : 1);
        }

        public void OnPartDamage(SystemToRepair part)
        {
            DamageMap map = null;
            Image ui = null;

            switch (part)
            {
                case SystemToRepair.CLEANCAR:
                    map = bodyMap;
                    ui = body;
                    break;

                case SystemToRepair.SUSPENSION:
                    UpdateSuspensions();
                    Main.Log(part + " damaged to " + Mathf.RoundToInt(suspensionsMap.GetStatus() / 2 * 100) + "%");
                    break;

                case SystemToRepair.RADIATOR:
                    map = radiatorMap;
                    ui = radiator;
                    break;

                case SystemToRepair.ENGINE:
                    map = engineMap;
                    ui = engine;
                    break;

                case SystemToRepair.TURBO:
                    map = turboMap;
                    ui = turbo;
                    break;

                case SystemToRepair.GEARBOX:
                    map = gearboxMap;
                    ui = gearbox;
                    break;
            }

            if (ui != null)
            {
                ui.color = LerpHSV(Settings.GetColor(Main.settings.badColor), Settings.GetColor(Main.settings.goodColor), map.GetStatus());
                Main.Log(part + " damaged to " + Mathf.RoundToInt(map.GetStatus() * 100) + "%");
            }
        }

        void UpdateSuspensions()
        {
            // alignment is 0.05 <= 0 => -0.05
            Color goodColor = Settings.GetColor(Main.settings.goodColor);
            Color badColor = Settings.GetColor(Main.settings.badColor);

            float suspensionState = suspensionsMap.GetStatus() / 2;
            float alignment = GameModeManager.GetSeasonDataCurrentGameMode().SelectedCar.performancePartsCondition.SteeringAlignment;

            leftSuspension.color = LerpHSV(badColor, goodColor, suspensionState + Mathf.Lerp(0.5f, 0, Mathf.InverseLerp(0, -0.05f, alignment)));
            rightSuspension.color = LerpHSV(badColor, goodColor, suspensionState + Mathf.Lerp(0.5f, 0, Mathf.InverseLerp(0, 0.05f, alignment)));
        }

        public void OnTirePuncture(Wheel wheel) => SetWheelState(wheelsData.IndexOf(wheel), 0);

        public void OnTireRepair(Wheel wheel) => SetWheelState(wheelsData.IndexOf(wheel), 1);

        void SetWheelState(int index, int state)
        {
            wheels[index].color = LerpHSV(Settings.GetColor(Main.settings.badColor), Settings.GetColor(Main.settings.goodColor), state);
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

        public void PlayAnimation(bool fadeIn) => LeanTween.alphaCanvas(group, fadeIn ? 1 : 0, 0.3f).setEaseOutSine().setIgnoreTimeScale(true);

        public static void Refresh()
        {
            if (instance == null)
                return;

            instance.transform.localPosition = Settings.GetUIPosition();
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
