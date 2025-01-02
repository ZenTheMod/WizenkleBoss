using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helper;
using static WizenkleBoss.Content.Projectiles.Misc.DeepSpaceTransmitterHelper;

namespace WizenkleBoss.Content.Dusts
{
    public class StarSpiralDust : ModDust, IDrawDustAboveDarkness
    {
        public override string Texture => "WizenkleBoss/Assets/Textures/MagicPixel";

            // Theres only gonna be like 5 on screen MAX and theyre client sided anyway.
        private const int length = 7;

        public override void OnSpawn(Dust dust)
        {
            dust.scale = 0.15f;
            dust.noGravity = true;

            dust.customData = new TrailData[length];

                // dumb
            if (dust.customData != null && dust.customData is TrailData[] Trail)
            {
                for (int i = 0; i < length; i++)
                {
                    Trail[i] = new(dust.position, dust.rotation);
                }
            }
        }

        private void SetArray(ref TrailData[] data, Dust dust)
        {
            for (int i = length - 2; i >= 0; i--)
            {
                data[i + 1] = data[i];
            }
            data[0] = new(dust.position, dust.rotation);
        }

        public override bool Update(Dust dust)
        {
            dust.position = Vector2.Lerp(dust.position, Center, 0.1f);
            dust.position = dust.position.RotatedBy(0.04f, Center);
            dust.rotation = dust.position.AngleTo(Center);
            dust.scale += 0.01f;

            if (dust.customData != null && dust.customData is TrailData[] Trail)
                SetArray(ref Trail, dust);

            dust.fadeIn = MathHelper.Clamp(dust.fadeIn + 0.1f, 0f, 1f);

            if (dust.scale >= 0.9f || dust.position.Distance(Center) < 2f)
            {
                    // FUCK YOU I HATE YOU SO MUCH
                Dust.NewDustPerfect(dust.position, ModContent.DustType<ShrinkingGlowDust>(), Vector2.Zero, 0, dust.color, 0.4f);
                Dust.NewDustPerfect(dust.position + Main.rand.NextVector2Circular(5, 5), ModContent.DustType<ShrinkingGlowDust>(), Vector2.Zero, 0, dust.color * 0.7f, 0.7f);

                dust.active = false;
            }
            return false;
        }

        public override bool PreDraw(Dust dust) => false;

        public void DrawAbove(SpriteBatch spriteBatch, GraphicsDevice device, Dust dust)
        {
            if (dust.customData == null || dust.customData is not TrailData[] Trail)
                return;

                // PRIMITIVE DUST ????????????? VAEMA APROVED GUYS
            List<VertexPositionColorTexture> vertices = [];
            for (int i = 0; i < length; i++)
            {
                float progress = (float)i / (float)length; // float cast ffs

                float width = 2.3f * dust.scale * (1 - progress);

                Color col = dust.color * (1 - progress) * dust.fadeIn;

                Vector2 position = (Trail[i].Position - Main.screenPosition) / 2f;

                vertices.Add(new VertexPositionColorTexture(new Vector3(position + new Vector2(width, 0).RotatedBy(Trail[i].Rotation - MathHelper.PiOver2), 0),
                    col,
                    new Vector2(progress, 0f)
                    ));

                vertices.Add(new VertexPositionColorTexture(new Vector3(position + new Vector2(width, 0).RotatedBy(Trail[i].Rotation + MathHelper.PiOver2), 0),
                    col,
                    new Vector2(progress, 0f)
                    ));
            }

            Main.instance.GraphicsDevice.Textures[0] = TextureRegistry.TextBoxStars;

            if (vertices.Count > 3)
                Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
        }
    }
}
