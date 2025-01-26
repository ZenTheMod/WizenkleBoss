using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Ink;

namespace WizenkleBoss.Content.Dusts
{
    public class InkDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new(Main.rand.Next(0, 3) * 10, 0, 10, 48);
            dust.scale = 1.6f;
        }
        public override bool Update(Dust dust)
        {
            if (dust.scale < 0.05f)
            {
                dust.active = false;
                return false;
            }
            dust.scale -= 0.01f;
            if (Collision.SolidCollision(dust.position, 1, 1))
            {
                dust.frame.Y = 48;
                dust.rotation = 0;
                dust.velocity.Y = 0;
                dust.velocity.X = 0;
                return false;
            }

            dust.velocity.Y += 0.35f;
            dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;

            dust.position += dust.velocity;
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
                // fuck you why no origin
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.White, dust.rotation, new Vector2(5, 24), dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
