using HarmonyLib;
using System;
using System.IO;
using UnityEngine;
using UnityModManagerNet;

using static UnityModManagerNet.UnityModManager;

namespace DamageUI
{
    public class Main
    {
        public static bool enabled { get; private set; }

        public static ModEntry.ModLogger Logger;
        public static Settings settings;
        public static DamagePanel damagePanel;

        static GameObject damageUIPrefab;

        // Called by the mod manager
        static bool Load(ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            settings = ModSettings.Load<Settings>(modEntry);

            // Harmony patching
            Harmony harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();

            // hook in mod manager event
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = (entry) => settings.Draw(entry);
            modEntry.OnSaveGUI = (entry) => settings.Save(entry);

            Try(() =>
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(modEntry.Path, "damage_ui"));

                if (bundle != null)
                    damageUIPrefab = bundle.LoadAsset<GameObject>("DamageUI");
                else
                    Error("Couldn't load asset bundle \"damage_ui\"");

                if (bundle != null)
                    Log("Loaded bundle \"damage_ui\"");
            });

            return true;
        }

        static bool OnToggle(ModEntry modEntry, bool state)
        {
            enabled = state;

            if (damagePanel != null)
                damagePanel.gameObject.SetActive(state);

            return true;
        }

        public static void Log(string message) => Logger.Log(message);

        public static void Error(string message) => Logger.Error(message);

        public static void Try(Action callback)
        {
            try
            {
                callback?.Invoke();
            }
            catch (Exception e)
            {
                Error(e.ToString());
            }
        }

        public static void SpawnUI(Car.CarStats stats)
        {
            HudManager hud = GameObject.FindObjectOfType<HudManager>();

            if (hud == null)
            {
                Error("Couldn't find the HUD Manager. Aborting.");
                return;
            }

            Transform UIParent = hud.transform.GetChild(0);
            Vector2 screenPos = new Vector2(settings.xPositionPercent * Screen.width, settings.yPositionPercent * Screen.height);

            damagePanel = GameObject.Instantiate(damageUIPrefab, screenPos, Quaternion.identity, UIParent).GetComponent<DamagePanel>();
            damagePanel.Init(stats.Aspiration != CarSpecs.EngineAspiration.NATURAL);

            Log("Spawned Damage UI");
        }
    }
}