using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static WizenkleBoss.Common.Helper.InkSystem;

namespace WizenkleBoss.Content.Projectiles
{
    public class InkSplat : ModProjectile, IDrawInk
    {
        public override string Texture => "WizenkleBoss/Assets/Textures/MagicPixel";
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 9000;

            Projectile.hide = true;
        }
        public void Shape()
        {
        }
        public override void AI()
        {
            base.AI();
        }
    }
}
