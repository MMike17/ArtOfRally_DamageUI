using UnityEngine;
using UnityEngine.UI;

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

        public void Init(bool hasTurbo)
        {
            instance = this;
            transform.localPosition = Vector2.one * Settings.GetUISize();

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

            // TODO : Do we really start with everything at 100% ?
            // How do I inject the start state of parts here ?

            body.color = Settings.GetColor(Main.settings.goodColor);
            leftSuspension.color = Settings.GetColor(Main.settings.goodColor);
            rightSuspension.color = Settings.GetColor(Main.settings.goodColor);
            radiator.color = Settings.GetColor(Main.settings.goodColor);
            engine.color = Settings.GetColor(Main.settings.goodColor);
            turbo.color = Settings.GetColor(Main.settings.goodColor);
            gearbox.color = Settings.GetColor(Main.settings.goodColor);
            frontLeftWheel.color = Settings.GetColor(Main.settings.goodColor);
            frontRightWheel.color = Settings.GetColor(Main.settings.goodColor);
            backLeftWheel.color = Settings.GetColor(Main.settings.goodColor);
            backRightWheel.color = Settings.GetColor(Main.settings.goodColor);

            gameObject.SetActive(Main.enabled);
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

        public static void Refresh()
        {
            if (instance == null)
                return;

            instance.transform.localScale = Vector2.one * Main.settings.uiScale;
            instance.transform.localPosition = Settings.GetUISize();

            // TODO : Get states of parts
            // TODO : Set colors of UI
        }
    }
}
