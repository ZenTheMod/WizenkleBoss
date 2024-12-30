using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WizenkleBoss.Common.Config
{
    public class WizenkleBossConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Effects")]

        [DefaultValue(0.4f)]
        [Slider]
        [Increment(0.05f)]
        [DrawTicks]
        [Range(0f, 1f)]
        [SliderColor(254, 179, 255, 255)]
        public float InkContrast;

        [DefaultValue(0.9f)]
        [Slider]
        [Increment(0.05f)]
        [DrawTicks]
        [Range(0.4f, 1f)]
        [SliderColor(254, 179, 255, 255)]
        public float EmbossStrength;

        [DefaultValue(true)]
        public bool TooltipEffects;

        [DefaultValue(true)]
        public bool Particles;

        [DefaultValue(700)]
        [Slider]
        [Increment(1)]
        [Range(10, 700)]
        [SliderColor(254, 179, 255, 255)]
        public int ParticleCap;

        [DefaultValue(0.75f)]
        [Slider]
        [Increment(0.05f)]
        [DrawTicks]
        [Range(0f, 1f)]
        [SliderColor(254, 179, 255, 255)]
        public float VideoOpacity;

        [Header("Sounds")]

        [DefaultValue(1f)]
        [Slider]
        [Increment(0.05f)]
        [DrawTicks]
        [Range(0f, 1f)]
        [SliderColor(254, 179, 255, 255)]
        public float VideoVolume;

        [DefaultValue(true)]
        public bool LaserLoop;

        [Header("UI")]

        [DefaultValue(true)]
        public bool TelescopeMovementKeyPrompt;

        [DefaultValue(0f)]
        [Slider]
        [Increment(0.1f)]
        [DrawTicks]
        [Range(0f, 1f)]
        [SliderColor(254, 179, 255, 255)]
        public float TelescopeMovementVelocity;

        [DefaultValue(0f)]
        [Slider]
        [Increment(0.1f)]
        [DrawTicks]
        [Range(0f, 1f)]
        [SliderColor(254, 179, 255, 255)]
        public float SatelliteMovementVelocity;

        [DefaultValue(true)]
        public bool SatelliteUseMousePosition;
    }
}
