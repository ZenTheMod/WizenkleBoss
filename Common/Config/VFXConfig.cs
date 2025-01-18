using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WizenkleBoss.Common.Config
{
    public class VFXConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Effects")]

        [DefaultValue(40)]
        [Slider]
        [Increment(5)]
        [DrawTicks]
        [Range(0, 100)]
        [SliderColor(254, 179, 255, 255)]
        public int InkContrast;

        [DefaultValue(90)]
        [Slider]
        [Increment(5)]
        [DrawTicks]
        [Range(40, 100)]
        [SliderColor(254, 179, 255, 255)]
        public int EmbossStrength;

        [DefaultValue(true)]
        public bool TooltipEffects;

        [DefaultValue(75)]
        [Slider]
        [Increment(5)]
        [DrawTicks]
        [Range(0, 100)]
        [SliderColor(254, 179, 255, 255)]
        public int VideoOpacity;

        [DefaultValue(75)]
        [Slider]
        [Increment(5)]
        [DrawTicks]
        [Range(0, 100)]
        [SliderColor(254, 179, 255, 255)]
        public int ScreenShake;

        [DefaultValue(false)]
        public bool TitleScreenHeavyRain;
    }
}
