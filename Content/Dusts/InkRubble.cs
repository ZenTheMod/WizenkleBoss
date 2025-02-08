using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace WizenkleBoss.Content.Dusts
{
    public class InkRubble : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new(0, Main.rand.Next(0, 3) * 28, 20, 28);
            dust.scale = 1f;
        }
        public override bool Update(Dust dust)
        {
            dust.fadeIn += 0.007f;
            if (dust.fadeIn > 1f)
            {
                dust.active = false;
                return false;
            }
            if (Collision.SolidCollision(dust.position, 1, 1))
            {
                dust.velocity.Y = -dust.velocity.Y * 0.49f;
                dust.velocity.X *= 0.85f;
            }
            else
                dust.velocity.Y += 0.35f;

            dust.rotation += dust.velocity.X * 0.02f;

            dust.position += dust.velocity;
            return false;
        }
        public override bool PreDraw(Dust dust)
        {
                // fuck you why no origin
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Color.White * (1f - dust.fadeIn), dust.rotation, new Vector2(10, 14), dust.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
