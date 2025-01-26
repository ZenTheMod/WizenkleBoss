using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Ink;

namespace WizenkleBoss.Content.Rarities
{
    public class StarboundRarity : ModRarity
    {
        public override Color RarityColor => InkSystem.OutlineColor;
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type;
        }
    }
    public class StarboundRarityGlobalItem : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (item.rare != ModContent.RarityType<StarboundRarity>() || !ModContent.GetInstance<VFXConfig>().TooltipEffects || !(line.Mod == "Terraria" && line.Name == "ItemName"))
                return base.PreDrawTooltipLine(item, line, ref yOffset);

            float X = line.X;
            float Y = line.Y;

            float rotation = line.Rotation;

            SpriteFont font = FontRegistry.Starlight.Value;

            Vector2 origin = line.Origin;
            Vector2 baseScale = line.BaseScale;
            Vector2 fontSize = Helper.GetStringSize(font, line.Text, Vector2.One);
            Vector2 center = fontSize / 2f;
            Vector2 pos = new(X, Y);
            Vector2 starBoxPos = pos - new Vector2(7, 5);
            Vector2 starOrigin = TextureRegistry.Star.Value.Size() / 2;

            float colmultiplier = MathHelper.Lerp(1.3f, 0.9f, Main.GameUpdateCount % 60 / 60f);

            Main.spriteBatch.Draw(TextureRegistry.TextBoxStars.Value, new Rectangle((int)starBoxPos.X, (int)starBoxPos.Y, (int)(TextureRegistry.TextBoxStars.Value.Width * 1.4f), (int)fontSize.Y + 10), null, (InkSystem.OutlineColor * colmultiplier) with { A = 0 }, line.Rotation, Vector2.Zero, SpriteEffects.None, 0f);

            Helper.DrawColorCodedStringShadow(Main.spriteBatch, font, line.Text, pos + (Vector2.UnitY * 8), InkSystem.OutlineColor with { A = 0 }, rotation, origin, baseScale * 1.3f);
            Helper.DrawColorCodedString(Main.spriteBatch, font, line.Text, pos + (Vector2.UnitY * 8), Color.Black, rotation, origin, baseScale * 1.3f);

            UnifiedRandom rand = new(Main.LocalPlayer.name.GetHashCode() + (int)(center.X + center.Y));
            int sparkleCount = rand.Next((int)fontSize.X / 6, (int)fontSize.X / 4) + 1;

            for (int i = 0; i < sparkleCount; i++)
            {
                Color color = InkSystem.OutlineColor;
                Color color2 = color * 0.75f;

                Vector2 v = new(rand.NextFloat(fontSize.X), rand.NextFloat(fontSize.Y * 0.8f));
                float lifeTime = Main.GlobalTimeWrappedHourly * 5.6f + rand.NextFloat((float)Math.PI * 14f);
                lifeTime %= (float)Math.PI * 14f;

                float starRotation = rand.NextFloat(0, MathHelper.TwoPi);
                if (!(lifeTime > (float)Math.PI * 2f))
                {
                    float sinValue = (float)Math.Sin((double)lifeTime);
                    Color white = (new Color(200 + color.R / 20, 200 + color.G / 20, 200 + color.B / 20, 255) * sinValue) with { A = 0 };
                    Main.spriteBatch.Draw(TextureRegistry.Star.Value, new Vector2(X, Y - lifeTime * 1f + 2f) + v, null, white, starRotation, starOrigin, (lifeTime / ((float)Math.PI * 2f) * 0.66f) / 2, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(TextureRegistry.Star.Value, new Vector2(X, Y - lifeTime * 1f + 2f) + v, null, white * 0.5f, starRotation, starOrigin, (lifeTime / ((float)Math.PI * 2f)) / 2, SpriteEffects.None, 0f);
                    //float scale2 = (float)Math.Sin((double)(lifeTime / ((float)Math.PI / 2f))) + 1f;
                    float scale3 = lifeTime / ((float)Math.PI * 2f) * 1.5f;
                    Color col2 = (color2 * sinValue) with { A = 0 };
                    Main.spriteBatch.Draw(TextureRegistry.Star.Value, new Vector2(X, Y) + v, null, col2, starRotation, starOrigin, scale3 * 0.9f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(TextureRegistry.Star.Value, new Vector2(X, Y) + v, null, col2, starRotation, starOrigin, scale3 * 0.6f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(TextureRegistry.Star.Value, new Vector2(X, Y) + v, null, col2, starRotation, starOrigin, scale3 * 0.55f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
    }
}
