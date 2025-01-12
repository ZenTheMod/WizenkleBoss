using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Buffs;
using MonoMod.Cil;
using WizenkleBoss.Common.Ink;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class GatekeepSystem : ModSystem
    {
        public override void OnModLoad()
        {
            IL_Player.Update += StopTheFloorBecauseDanteSkyblockCanceledYouToo;

            On_Player.DryCollision += StopCollisionForGhostsWhileDashing;
            On_Player.WaterCollision += StopWaterCollisionForGhostsWhileDashing;
            On_Player.SlopingCollision += StopSlopingCollisionForGhostsWhileDashing;
            On_Player.HoneyCollision += StopHoneyCollisionForGhostsWhileDashing;
            On_Player.ShimmerCollision += StopShimmerCollisionForGhostsWhileDashing;
        }
        public override void OnModUnload()
        {
            IL_Player.Update -= StopTheFloorBecauseDanteSkyblockCanceledYouToo;

            On_Player.DryCollision -= StopCollisionForGhostsWhileDashing;
            On_Player.WaterCollision -= StopWaterCollisionForGhostsWhileDashing;
            On_Player.SlopingCollision -= StopSlopingCollisionForGhostsWhileDashing;
            On_Player.HoneyCollision -= StopHoneyCollisionForGhostsWhileDashing;
            On_Player.ShimmerCollision -= StopShimmerCollisionForGhostsWhileDashing;
        }
        private void StopTheFloorBecauseDanteSkyblockCanceledYouToo(ILContext il)
        {
            ILCursor c = new(il);

            ILLabel target = c.DefineLabel();

            c.GotoNext(MoveType.After,
                i => i.MatchLdarg0(),
                i => i.MatchCall<Player>("CheckCrackedBrickBreak"),
                i => i.MatchLdarg0(),
                i => i.MatchLdfld<Player>("shimmering"),
                i => i.MatchBrtrue(out target));

            c.EmitLdarg0();

            c.EmitDelegate((Player player) => player.GetModPlayer<InkPlayer>().InGhostInk && player.GetModPlayer<InkPlayer>().InkDashCooldown > 0);
            c.EmitBrtrue(target);
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
