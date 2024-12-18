using Microsoft.CodeAnalysis.CSharp.Syntax;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.Map;
using Terraria.ModLoader;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.Tiles.Clouds;

namespace WizenkleBoss.Assets.Helper
{
    public class InkKeybindSystem : ModSystem
    {
        public static ModKeybind InkDash { get; private set; }

        public override void Load()
        {
            InkDash = KeybindLoader.RegisterKeybind(Mod, "InkDash", "RightShift");
        }
        public override void Unload()
        {
            InkDash = null;
        }
    }
}
