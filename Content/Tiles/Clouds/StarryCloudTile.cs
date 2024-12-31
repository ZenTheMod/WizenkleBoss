using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using WizenkleBoss.Content.Items.Placeable;
using WizenkleBoss.Content.Items.Tiles;

namespace WizenkleBoss.Content.Tiles.Clouds
{
    public class StarryCloudTile : ModTile
    {
        private static Asset<Texture2D> GlowTexture;
        public static float StarAlpha
        {
            get
            {
                float starAlpha;
                if (Main.dayTime)
                {
                    starAlpha = 0;
                }
                else if (Main.time < 16200f)
                {
                    float interpolator = (float)Main.time / 16200f;
                    starAlpha = interpolator;
                }
                else if (Main.time < 24000f)
                {
                    starAlpha = 1;
                }
                else
                {
                    float interpolator = ((float)Main.time - 24000f) / (32400f - 24000f);
                    starAlpha = 1 - interpolator;
                }
                return starAlpha;
            }
        }
        public override bool HasWalkDust() => true;
        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustID.RainCloud;
            makeDust = true;
        }
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glowmask");
            }
            RegisterItemDrop(ModContent.ItemType<StarryCloudItem>());
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = false;

            DustType = DustID.RainCloud;

            MineResist = 1f;
            AnimationFrameHeight = 90;
            AddMapEntry(new Color(56, 52, 71));
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter >= 18)
            {
                frameCounter = 0;
                if (++frame >= 4)
                {
                    frame = 0;
                }
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.57f * StarAlpha;
            g = 0.55f * StarAlpha;
            b = 0.83f * StarAlpha;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            Vector2 offsets = -Main.screenPosition + zero;

            if (!tile.IsTileInvisible)
                SlopedDrawing(i, j, TextureAssets.Tile[Type].Value, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(i, j) * (StarAlpha + 0.98f), offsets);
            return false;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Color col = Color.White * (StarAlpha + 0.1f);

            UnifiedRandom rand = new((i.ToString() + "fuckyou" + j.ToString()).GetHashCode());
            col *= rand.NextFloat(-1.5f, 1f);

            int uniqueAnimationFrame = Main.tileFrame[Type] + i + rand.Next(0, 5);

            uniqueAnimationFrame %= 4;

            int frameYOffset = uniqueAnimationFrame * AnimationFrameHeight;

            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            Vector2 offsets = -Main.screenPosition + zero;

            SlopedDrawing(i, j, GlowTexture.Value, new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, 16), j > Main.worldSurface ? Color.White : col, offsets);
        }
        private static void SlopedDrawing(int i, int j, Texture2D texture, Rectangle sourceRectangle, Color drawColor, Vector2 offsets)
        {
            Tile tile = Main.tile[i, j];

            int TileFrameX = sourceRectangle.X;
            int TileFrameY = sourceRectangle.Y;

            int iX16 = i * 16;
            int jX16 = j * 16;
            Vector2 location = new(iX16, jX16);

            Vector2 drawCoordinates = location + offsets;
            if (tile.Slope == 0 && !tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(TileFrameX, TileFrameY, 16, 16), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else if (tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(texture, new Vector2(drawCoordinates.X, drawCoordinates.Y + 8), new Rectangle(TileFrameX, TileFrameY, 16, 8), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                byte b = (byte)tile.Slope;
                Rectangle TileFrame;
                Vector2 drawPos;
                if (b == 1 || b == 2)
                {
                    int length;
                    int height2;
                    for (int a = 0; a < 8; ++a)
                    {
                        int aX2 = a * 2;
                        if (b == 2)
                        {
                            length = 16 - aX2 - 2;
                            height2 = 14 - aX2;
                        }
                        else
                        {
                            length = aX2;
                            height2 = 14 - length;
                        }

                        TileFrame = new Rectangle(TileFrameX + length, TileFrameY, 2, height2);
                        drawPos = new Vector2(iX16 + length, jX16 + aX2) + offsets;
                        Main.spriteBatch.Draw(texture, drawPos, TileFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }

                    TileFrame = new Rectangle(TileFrameX, TileFrameY + 14, 16, 2);
                    drawPos = new Vector2(iX16, jX16 + 14) + offsets;
                    Main.spriteBatch.Draw(texture, drawPos, TileFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
                else
                {
                    int length;
                    int height2;
                    for (int a = 0; a < 8; ++a)
                    {
                        int aX2 = a * 2;
                        if (b == 3)
                        {
                            length = aX2;
                            height2 = 16 - length;
                        }
                        else
                        {
                            length = 16 - aX2 - 2;
                            height2 = 16 - aX2;
                        }

                        TileFrame = new Rectangle(TileFrameX + length, TileFrameY + 16 - height2, 2, height2);
                        drawPos = new Vector2(iX16 + length, jX16) + offsets;
                        Main.spriteBatch.Draw(texture, drawPos, TileFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }

                    drawPos = new Vector2(iX16, jX16) + offsets;
                    TileFrame = new Rectangle(TileFrameX, TileFrameY, 16, 2);
                    Main.spriteBatch.Draw(texture, drawPos, TileFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
            }
        }
    }
}
