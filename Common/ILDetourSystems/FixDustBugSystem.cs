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

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class FixDustBugSystem : ModSystem
    {
        public override void Load()
        {
            IL_Dust.NewDust += FixStupidBugBecauseTerrariaDevsAreIncompetent;
        }

        public override void Unload()
        {
            IL_Dust.NewDust -= FixStupidBugBecauseTerrariaDevsAreIncompetent;
        }

        private void FixStupidBugBecauseTerrariaDevsAreIncompetent(ILContext il)
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
