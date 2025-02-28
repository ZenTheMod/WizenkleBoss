using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.Utilities;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.Helpers;

namespace WizenkleBoss.Common.StarRewrite
{
    public class InteractableStar
    {
        public InteractableStar(UnifiedRandom rand)
        {
            position = rand.NextUniformVector2Circular(1200);
            temperature = rand.Next(4000, 30000);
            baseSize = rand.NextFloat(0.5f, 1.4f);
            starType = rand.Next(0, 4);
            rotation = rand.NextFloatDirection();
            twinkle = rand.NextFloat(2f);

            name = Language.GetTextValue("Mods.WizenkleBoss.StarNames.Name" + rand.Next(235)) + " - " + rand.Next(100000);
        }

        public InteractableStar()
        {
            position = -Vector2.UnitY * 330;
            temperature = 0;
            baseSize = 2;
            inkStar = true;

            name = Language.GetTextValue("Mods.WizenkleBoss.StarNames.ImportantStarName");
        }

        private static Color lowestTemperature = new(255, 204, 152);
        private static Color lowTemperature = new(255, 242, 238);
        private static Color highTemperature = new(236, 238, 255);
        private static Color highestTemperature = new(153, 185, 255);
        private static Color compressed = new(255, 96, 136);

        public Vector2 position;

        public int temperature;

        public float baseSize;

        public float compression;

        public float rotation;

        public float twinkle;

        public int starType;

        public string name;

        internal bool inkStar;

        public Color GetColor()
        {
            float interpolate = temperature / 30000f;

            if (inkStar)
                return Color.Lerp(InkSystem.InkColor, compressed, compression);

            Color color;
            if (interpolate <= 0.4f) 
                color = Color.Lerp(lowestTemperature, lowTemperature, Utils.Remap(interpolate, 0f, 0.4f, 0f, 1f));
            else if (interpolate <= 0.6f)
                color = Color.Lerp(lowTemperature, highTemperature, Utils.Remap(interpolate, 0.4f, 0.6f, 0f, 1f));
            else
                color = Color.Lerp(highTemperature, highestTemperature, Utils.Remap(interpolate, 0.6f, 1f, 0f, 1f));

            return Color.Lerp(color, compressed, compression);
        }
    }
}
