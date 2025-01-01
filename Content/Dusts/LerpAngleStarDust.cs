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
    public class LerpAngleStarDust : ModDust, IDrawDustAboveDarkness
    {
        public override string Texture => "WizenkleBoss/Assets/Textures/MagicPixel";

            // Theres only gonna be like 5 on screen MAX and theyre client sided anyway.
        private const int length = 12;

        public override void OnSpawn(Dust dust)
        {
            dust.scale = 2.2f;
            dust.noGravity = true;

            dust.rotation = dust.velocity.ToRotation();

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
            dust.rotation = Utils.AngleLerp(dust.rotation, (-Vector2.One).ToRotation(), 0.3f);
            dust.velocity = dust.rotation.ToRotationVector2() * dust.velocity.Length() * 1.1f;
            dust.position += dust.velocity;
            dust.scale -= 0.13f;

            if (dust.customData != null && dust.customData is TrailData[] Trail)
                SetArray(ref Trail, dust);

            dust.fadeIn = MathHelper.Clamp(dust.fadeIn + 0.1f, 0f, 1f);

            if (dust.scale <= 0.01f || dust.position.Distance(Center) > 3000f)
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust) => false;

        public void DrawAbove(SpriteBatch spriteBatch, GraphicsDevice device, Dust dust)
        {
            if (dust.customData == null || dust.customData is not TrailData[] Trail)
                return;

                // PRIMITIVE DUST ????????????? VAEMA APROVED GUYS
            List<VertexInfo2> verticies = [];
            for (int i = 0; i < length; i++)
            {
                float progress = (float)i / (float)length; // float cast ffs

                float width = 2.3f * dust.scale * (1 - progress);

                Color col = dust.color * (1 - progress) * dust.fadeIn;

                Vector2 position = (Trail[i].Position - Main.screenPosition) / 2f;

                verticies.Add(new VertexInfo2(position + new Vector2(width, 0).RotatedBy(Trail[i].Rotation - MathHelper.PiOver2),
                    new Vector3(progress, 0f, 0f),
                    col));

                verticies.Add(new VertexInfo2(position + new Vector2(width, 0).RotatedBy(Trail[i].Rotation + MathHelper.PiOver2),
                    new Vector3(progress, 1f, 0f),
                    col));
            }

            Main.instance.GraphicsDevice.Textures[0] = TextureRegistry.TextBoxStars;

            if (verticies.Count > 3)
                Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, verticies.ToArray(), 0, verticies.Count - 2);
        }
    }
}
