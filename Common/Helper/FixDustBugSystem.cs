using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Content.Dusts;
using MonoMod.Cil;

namespace WizenkleBoss.Common.Helper
{
    public class FixDustBugSystem : ModSystem
    {
        public override void Load()
        {
            IL_Dust.NewDust += IL_Dust_NewDust;
        }

        public override void Unload()
        {
            IL_Dust.NewDust -= IL_Dust_NewDust;
        }

        private void IL_Dust_NewDust(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After,
                i => i.MatchLdloca(1),
                i => i.MatchLdloc2(),
                i => i.MatchCall<Rectangle>("Intersects"));

            c.EmitLdarg3();
            c.EmitDelegate((bool Intersects, int Type) =>
            {
                if (Type == ModContent.DustType<ShrinkingGlowDust>())
                    return true;
                return Intersects;
            });
        }
    }
}
