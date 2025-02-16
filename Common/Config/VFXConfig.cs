using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace WizenkleBoss.Common.Config
{
    public class ContrastShaderIntRangeElement : BaseShaderIntRangeElement { public override ref int modifying => ref ModContent.GetInstance<VFXConfig>().InkContrast; }
    public class EmbossShaderIntRangeElement : BaseShaderIntRangeElement { public override ref int modifying => ref ModContent.GetInstance<VFXConfig>().EmbossStrength; }

    public class VFXConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Effects")]

        [DefaultValue(40)]
        [SliderColor(85, 25, 255, 255)]
        [CustomModConfigItem(typeof(ContrastShaderIntRangeElement))]
        public int InkContrast;

        [DefaultValue(90)]
        [Range(40, 100)]
        [SliderColor(85, 25, 255, 255)]
        [CustomModConfigItem(typeof(EmbossShaderIntRangeElement))]
        public int EmbossStrength;

        [DefaultValue(true)]
        [CustomModConfigItem(typeof(TooltipEffectsImageBooleanElement))]
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
        public int ScreenShake; // the value in question.

        [DefaultValue(false)]
        public bool TitleScreenHeavyRain;
    }
}
