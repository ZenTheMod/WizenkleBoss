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
    public class UIConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("UI")]

        [DefaultValue(true)]
        public bool TelescopeMovementKeyPrompt;

        [DefaultValue(0)]
        [Slider]
        [Increment(10)]
        [DrawTicks]
        [Range(0, 100)]
        [SliderColor(254, 179, 255, 255)]
        public int TelescopeMovementVelocity;

        [DefaultValue(0)]
        [Slider]
        [Increment(10)]
        [DrawTicks]
        [Range(0, 100)]
        [SliderColor(254, 179, 255, 255)]
        public int SatelliteMovementVelocity;

        [DefaultValue(true)]
        public bool SatelliteUseMousePosition;

        [DefaultValue(false)]
        public bool SatelliteMidnightMode;
    }
}
