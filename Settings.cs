using UnityEngine;
using UnityModManagerNet;

using static UnityModManagerNet.UnityModManager;

namespace DamageUI
{
    public class Settings : ModSettings, IDrawable
    {
        readonly static Color Brown = new Color(0.75f, 0.5f, 0.25f);

        public enum ColorTag
        {
            White,
            Grey,
            Black,
            Red,
            Green,
            Blue,
            Yellow,
            Magenta,
            Cyan,
            Brown
        }

        [Draw(DrawType.PopupList)]
        public ColorTag goodColor = ColorTag.Green;
        [Draw(DrawType.PopupList)]
        public ColorTag badColor = ColorTag.Red;

        [Draw(DrawType.Slider, Min = 0, Max = 1, Precision = 3)]
        public float xPositionPercent = 0.17f;
        [Draw(DrawType.Slider, Min = 0, Max = 1, Precision = 3)]
        public float yPositionPercent = 0.42f;

        [Draw(DrawType.Slider, Min = 0.2f, Max = 1.5f, Precision = 2)]
        public float uiScale = 0.8f;

        public override void Save(ModEntry modEntry) => Save(this, modEntry);

        public void OnChange() => DamagePanel.Refresh();

        public static Color GetColor(ColorTag tag)
        {
            switch (tag)
            {
                case ColorTag.White:
                    return Color.white;
                case ColorTag.Grey:
                    return Color.grey;
                case ColorTag.Black:
                    return Color.black;
                case ColorTag.Red:
                    return Color.red;
                case ColorTag.Green:
                    return Color.green;
                case ColorTag.Blue:
                    return Color.blue;
                case ColorTag.Yellow:
                    return Color.yellow;
                case ColorTag.Magenta:
                    return Color.magenta;
                case ColorTag.Cyan:
                    return Color.cyan;
                case ColorTag.Brown:
                    return Brown;
                default:
                    return Color.clear;
            }
        }

        public static Vector2 GetUISize()
        {
            return new Vector2(Main.settings.xPositionPercent * Screen.width, Main.settings.xPositionPercent * Screen.height);
        }
    }
}