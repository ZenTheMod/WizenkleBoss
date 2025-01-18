using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace WizenkleBoss.Common.Config
{
    public class SFXConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Sounds")]

        [DefaultValue(true)]
        public bool LaserLoop;
    }
}
