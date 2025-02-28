using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using WizenkleBoss.Common.StarRewrite;

namespace WizenkleBoss.Common.Config
{
    public class ContrastShaderIntRangeElement : BaseShaderIntRangeElement { public override ref int modifying => ref ModContent.GetInstance<VFXConfig>().InkContrast; }
    public class EmbossShaderIntRangeElement : BaseShaderIntRangeElement { public override ref int modifying => ref ModContent.GetInstance<VFXConfig>().EmbossStrength; }
    public class SunAndMoonReworkLockedBooleanElement : BaseLockedBooleanElement { public override bool LockToggle => ModContent.GetInstance<VFXConfig>().SunAndMoonRework; public override bool LockMode => false; }
    public class RealisticSkyLockedBooleanElement : BaseLockedBooleanElement { public override bool LockToggle => RealisticSkyCompatSystem.RealisticSkyEnabled; public override bool LockMode => false; }

    public class VFXConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Shaders")]

        [DefaultValue(40)]
        [SliderColor(85, 25, 255, 255)]
        [CustomModConfigItem(typeof(ContrastShaderIntRangeElement))]
        public int InkContrast;

        [DefaultValue(90)]
        [Range(40, 100)]
        [SliderColor(85, 25, 255, 255)]
        [CustomModConfigItem(typeof(EmbossShaderIntRangeElement))]
        public int EmbossStrength;

        [Header("Sky")]

        [DefaultValue(true)]
        [ReloadRequired]
        public bool SunAndMoonRework;

        [DefaultValue(false)]
        [CustomModConfigItem(typeof(SunAndMoonReworkLockedBooleanElement))]
        public bool PixelatedSunAndMoon;

        [DefaultValue(false)]
        [CustomModConfigItem(typeof(SunAndMoonReworkLockedBooleanElement))]
        public bool TransparentMoonShadow;

        [DefaultValue(false)]
        public bool PixelatedStars;

        [DefaultValue(false)]
        [CustomModConfigItem(typeof(RealisticSkyLockedBooleanElement))]
        public bool DrawRealisticStars;

        [Header("Misc")]

        [DefaultValue(75)]
        [Slider]
        [Increment(5)]
        [DrawTicks]
        [Range(0, 100)]
        [SliderColor(254, 179, 255, 255)]
        public int ScreenShake;

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

        [DefaultValue(false)]
        public bool TitleScreenHeavyRain;
    }
}
