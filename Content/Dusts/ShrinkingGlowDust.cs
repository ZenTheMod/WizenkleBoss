using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Registries;
using static WizenkleBoss.Content.Projectiles.Misc.DeepSpaceTransmitterHelper;

namespace WizenkleBoss.Content.Dusts
{
    public class ShrinkingGlowDust : ModDust, IDrawDustAboveDarkness
    {
        public override string Texture => "WizenkleBoss/Assets/Textures/MagicPixel";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            dust.scale -= 0.05f;

            if (dust.scale <= 0.01f)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust) => false;

        public void DrawAbove(SpriteBatch spriteBatch, GraphicsDevice device, Dust dust)
        {
            spriteBatch.Draw(Textures.Bloom.Value, dust.position - Main.screenPosition, null, dust.color * dust.scale, 0f, Textures.Bloom.Size() / 2, dust.scale * 0.4f, SpriteEffects.None, 0f);
        }
    }
}
