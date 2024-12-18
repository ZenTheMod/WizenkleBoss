using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Buffs;

namespace WizenkleBoss.Assets.Helper
{
    public class DisableTileCollisionWhileInkActiveSystem : ModSystem
    {
        public override void OnModLoad()
        {
            On_Player.DryCollision += StopCollisionForGhostsWhileDashing;
            On_Player.WaterCollision += StopWaterCollisionForGhostsWhileDashing;
            On_Player.SlopingCollision += StopSlopingCollisionForGhostsWhileDashing;
            On_Player.HoneyCollision += StopHoneyCollisionForGhostsWhileDashing;
            On_Player.ShimmerCollision += StopShimmerCollisionForGhostsWhileDashing;
        }
        public override void OnModUnload()
        {
            On_Player.DryCollision -= StopCollisionForGhostsWhileDashing;
            On_Player.WaterCollision -= StopWaterCollisionForGhostsWhileDashing;
            On_Player.SlopingCollision -= StopSlopingCollisionForGhostsWhileDashing;
            On_Player.HoneyCollision -= StopHoneyCollisionForGhostsWhileDashing;
            On_Player.ShimmerCollision -= StopShimmerCollisionForGhostsWhileDashing;
        }

        private void StopShimmerCollisionForGhostsWhileDashing(On_Player.orig_ShimmerCollision orig, Player self, bool fallThrough, bool ignorePlats, bool noCollision)
        {
            if (self.HasBuff<InkDrugStatBuff>() && self.GetModPlayer<InkPlayer>().InkyArtifact && self.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
            {
                self.position += self.GetModPlayer<InkPlayer>().DashVelocity;
                return;
            }
            orig(self, fallThrough, ignorePlats, noCollision);
        }

        private void StopHoneyCollisionForGhostsWhileDashing(On_Player.orig_HoneyCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (self.HasBuff<InkDrugStatBuff>() && self.GetModPlayer<InkPlayer>().InkyArtifact && self.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
            {
                self.position += self.GetModPlayer<InkPlayer>().DashVelocity;
                return;
            }
            orig(self, fallThrough, ignorePlats);
        }
        private void StopSlopingCollisionForGhostsWhileDashing(On_Player.orig_SlopingCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (self.HasBuff<InkDrugStatBuff>() && self.GetModPlayer<InkPlayer>().InkyArtifact && self.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
            {
                self.position += self.GetModPlayer<InkPlayer>().DashVelocity;
                return;
            }
            orig(self, fallThrough, ignorePlats);
        }
        private void StopWaterCollisionForGhostsWhileDashing(On_Player.orig_WaterCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (self.HasBuff<InkDrugStatBuff>() && self.GetModPlayer<InkPlayer>().InkyArtifact && self.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
            {
                self.position += self.GetModPlayer<InkPlayer>().DashVelocity;
                return;
            }
            orig(self, fallThrough, ignorePlats);
        }
        private void StopCollisionForGhostsWhileDashing(On_Player.orig_DryCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            if (self.HasBuff<InkDrugStatBuff>() && self.GetModPlayer<InkPlayer>().InkyArtifact && self.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
            {
                self.position += self.GetModPlayer<InkPlayer>().DashVelocity;
                return;
            }
            orig(self, fallThrough, ignorePlats);
        }
    }
}
