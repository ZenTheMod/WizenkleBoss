using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Tiles.Clouds;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class TileFallDamageSystem : ModSystem
    {
            // Huge thanks to lion8cake/Kittenbun.
        public override void Load()
        {
            IL_Player.Update += TileFallDamage;
        }
        public override void Unload()
        {
            IL_Player.Update -= TileFallDamage;
        }
        private void TileFallDamage(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.After, i => i.MatchLdfld<Player>("fallStart"), i => i.MatchSub(), i => i.MatchStloc(56));
            c.EmitLdarg0();
            c.EmitLdloca(56);   //num26
            c.EmitDelegate((Player self, ref int num26) =>
            {
                if (num26 > 0 || self.gravDir == -1f && num26 < 0)
                {

                    int xPos = (int)(self.position.X / 16f);
                    int XWidth = (int)((self.position.X + self.width) / 16f);
                    int j = (int)((self.position.Y + self.height + 1f) / 16f);

                    if (self.gravDir == -1f)
                    {
                        j = (int)((self.position.Y - 1f) / 16f);
                    }
                    for (int i = xPos; i <= XWidth; i++)
                    {
                        if (Main.tile[i, j] != null && Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<StarryCloudTile>())
                        {
                            num26 = 0;
                            break;
                        }
                    }

                }
            });
        }
    }
}
