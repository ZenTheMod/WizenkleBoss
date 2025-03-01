﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WizenkleBoss.Common.Config
{
    public class DebugConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        public bool TelescopeDebugText;

        [DefaultValue(false)]
        public bool DebugColoredRipples;
    }
}
