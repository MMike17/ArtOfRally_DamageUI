using UnityEngine;
using UnityEngine.UI;

namespace DamageUI
{
    public class DamagePanel : MonoBehaviour
    {
        static DamagePanel instance;

        Image body;
        GameObject engineHolder;
        GameObject engineAndTurboHolder;
        Image engine;
        Image turbo;
        Image radiator;
        Image gearbox;
        Image suspensions;
        Image frontLeftWheel;
        Image frontRightWheel;
        Image backLeftWheel;
        Image backRightWheel;

        public void Init(bool hasTurbo)
        {
            instance = this;

            engineHolder.SetActive(!hasTurbo);
            engineAndTurboHolder.SetActive(hasTurbo);

            // TODO : Finish getting all UI refs
            body = transform.GetChild(0).GetComponent<Image>();
            engine = (hasTurbo ? engineAndTurboHolder.transform.GetChild(0) : engineHolder.transform).GetComponent<Image>();
            // turbo
            // radiator
            // gearbox
            // suspensions
            // frontLeftWheel
            // frontRightWheel
            // backLeftWheel
            // backRightWheel

            // TODO : Do we really start with everything at 100% ?
            // How do I inject the start state of parts here ?

            body.color = Settings.GetColor(Main.settings.goodColor);
            engine.color = Settings.GetColor(Main.settings.goodColor);
            turbo.color = Settings.GetColor(Main.settings.goodColor);
            radiator.color = Settings.GetColor(Main.settings.goodColor);
            gearbox.color = Settings.GetColor(Main.settings.goodColor);
            suspensions.color = Settings.GetColor(Main.settings.goodColor);
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

            // TODO : Get states of parts
            // TODO : Set colors of UI
            // TODO : Position UI
        }
    }
}
