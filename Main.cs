using System;
using UnityEngine;
using UnityModManagerNet;

using static UnityModManagerNet.UnityModManager;

namespace DamageUI
{
    public class Main
    {
        // TODO : Add test method to force wheel puncture

        public static bool enabled { get; private set; }

        public static ModEntry.ModLogger Logger;
        public static Settings settings;
        public static DamagePanel damagePanel;

        // Called by the mod manager
        static bool Load(ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            settings = ModSettings.Load<Settings>(modEntry);

            // hook in mod manager event
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = (entry) => settings.Draw(entry);
            modEntry.OnSaveGUI = (entry) => settings.Save(entry);

            // TODO : Load UI from bundle

            return true;
        }

        static bool OnToggle(ModEntry modEntry, bool state)
        {
            enabled = state;
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
                Log(e.ToString());
            }
        }

        public static void SpawnUI(Car.CarStats stats)
        {
            // TODO : What is the parent transform ?
            //Transform UIParent = ;

            // TODO : spawn and keep reference to UI
            //new Vector2(settings., settings.)
            //damagePanel = GameObject.Instantiate(, settings.uiPosition, Quaternion.identity, UIParent);
            //damagePanel.Init(stats.Aspiration != CarSpecs.EngineAspiration.NATURAL);
        }
    }
}