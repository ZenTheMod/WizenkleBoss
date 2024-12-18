using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using WizenkleBoss.Content.Buffs;

namespace WizenkleBoss.Assets.Helper
{
    public class HidePlayerHeadForGhostsSystem : ModSystem
    {
        public override void OnModLoad()
        {
            On_PlayerHeadDrawRenderTargetContent.UsePlayer += HideGhosts;
        }
        public override void OnModUnload()
        {
            On_PlayerHeadDrawRenderTargetContent.UsePlayer -= HideGhosts;
        }
        private void HideGhosts(On_PlayerHeadDrawRenderTargetContent.orig_UsePlayer orig, PlayerHeadDrawRenderTargetContent self, Player player)
        {
            if (player.HasBuff<InkDrugBuff>() || player.HasBuff<InkDrugStatBuff>())
                orig(self, null);
            else
                orig(self, player);
        }
    }
}
