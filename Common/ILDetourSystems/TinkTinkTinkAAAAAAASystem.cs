using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WizenkleBoss.Common.ILDetourSystems
{
        // Thanks to Azathoth for letting me use this system - https://github.com/Twilight-Egress/TwilightEgress/commit/6d8c50c47ae4a42a293cb43eb5b74196351bf318
    public class TinkTinkTinkAAAAAAASystem : ModSystem
    {
        public override void Load()
        {
            On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += MineNPC;
        }

        public override void Unload()
        {
            On_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool -= MineNPC;
        }

        private void MineNPC(On_Player.orig_ItemCheck_UseMiningTools_ActuallyUseMiningTool orig, Player self, Item sItem, out bool canHitWalls, int x, int y)
        {
            orig(self, sItem, out canHitWalls, x, y);

                // Simpler check for it
            if ((self.Center - Main.MouseWorld).LengthSquared() > Math.Pow((self.blockRange + sItem.tileBoost) * 16, 2) || self.whoAmI != Main.myPlayer || Main.SmartCursorShowing)
                return;

            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.ModNPC is not IMinableNPC mine)
                    continue;
                if (n.Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y))
                {
                    int damage = n.SimpleStrikeNPC(sItem.pick, 0, noPlayerInteraction: false);
                    mine.OnMined(damage, self);
                    self.ApplyItemTime(sItem, self.pickSpeed * 1.5f);
                }
            }
        }
    }
}
