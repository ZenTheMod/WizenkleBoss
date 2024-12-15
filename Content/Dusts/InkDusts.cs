using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace WizenkleBoss.Content.Dusts
{
    public class InkStar : ModDust
    {
        public override string Texture => "WizenkleBoss/Assets/Textures/Stars/Star";
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new Rectangle(0, 0, 74, 74);
            dust.color = new Color(85, 25, 255, 255) with { A = 0 };
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn += 0.15f;
            if (dust.fadeIn >= 1)
                dust.scale -= 0.05f;
            else
                dust.scale = dust.fadeIn;

            if (dust.scale < 0.05f && dust.fadeIn >= 1)
            {
                    // Bad idea
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDust(dust.position, 1, 1, ModContent.DustType<InkDust>(), 0, -5, 0, Color.White, 5);
                }
                dust.active = false;
            }

            return false;
        }
    }
    public class InkDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
        }
        public override bool Update(Dust dust)
        {
            dust.scale -= 0.01f;
            dust.velocity.Y += 0.3f;
                // Bad idea
            if (Collision.SolidCollision(dust.position, 1, 1))
            {
                dust.velocity.Y = 0;
                dust.velocity.X = 0;
            }
            dust.position += dust.velocity;

            if (dust.scale < 0.05f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
