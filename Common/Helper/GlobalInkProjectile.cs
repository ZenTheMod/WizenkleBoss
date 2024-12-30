using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WizenkleBoss.Common.Helper
{
    public class GlobalInkProjectile : GlobalProjectile
    {
        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            if (Main.player[projectile.owner].GetModPlayer<InkPlayer>().InGhostInk)
                return false;
            return base.CanHitNPC(projectile, target);
        }
        public override bool CanHitPvp(Projectile projectile, Player target)
        {
            if (Main.player[projectile.owner].GetModPlayer<InkPlayer>().InGhostInk)
                return false;
            return base.CanHitPvp(projectile, target);
        }
    }
}
