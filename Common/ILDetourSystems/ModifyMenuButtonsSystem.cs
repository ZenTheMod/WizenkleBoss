using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Content.Dusts;
using MonoMod.Cil;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.MenuStyles;
using Terraria.Localization;
using Terraria.ID;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class ModifyMenuButtonsSystem : ModSystem
    {
        public override void Load()
        {
            IL_Main.DrawMenu += ModifyButtons;
        }

        public override void Unload()
        {
            IL_Main.DrawMenu -= ModifyButtons;
        }

        private void ModifyButtons(ILContext il)
        {
            ILCursor c = new(il);
            ILLabel target = c.DefineLabel();

                // uhhhh lets be EXTRA safe okay ?
            c.GotoNext(MoveType.After,
                i => i.MatchLdloca(2),
                i => i.MatchLdloc1(),
                i => i.MatchLdloc1(),
                i => i.MatchLdloc1(),
                i => i.MatchLdcI4(255),
                i => i.MatchCall<Color>(".ctor"));

            c.EmitLdloca(2);
            c.EmitDelegate((ref Color color) => {
                if (!InkPondMenu.InMenu)
                    return;
                color = InkSystem.InkColor * 1.2f;
            });
        }
    }
}
