using System;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using WizenkleBoss.Content.Items.Tiles;
using WizenkleBoss.Content.Tiles.Clouds;

namespace WizenkleBoss.Content.Walls
{
    internal class StarryCloudWall : ModWall
    {
        private static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glowmask");
            }
            RegisterItemDrop(ModContent.ItemType<StarryCloudWallItem>());
            DustType = DustID.RainCloud;
            Main.wallLight[Type] = true;
        }
        public override void AnimateWall(ref byte frame, ref byte frameCounter)
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
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            int xLength = 32;

            int xPos = tile.WallFrameX;
            int yPos = tile.WallFrameY;

            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            Color col = Color.White * (StarryCloudTile.StarAlpha + 0.1f);

            UnifiedRandom rand = new((i.ToString() + "fuckyoutoo<3" + j.ToString()).GetHashCode());
            col *= rand.NextFloat(-1.5f, 1f);

            spriteBatch.Draw(TextureAssets.Wall[Type].Value, pos + new Vector2(-8, -8), new(xPos, yPos, xLength, 32), Lighting.GetColor(i, j) * (StarryCloudTile.StarAlpha + 0.97f), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            spriteBatch.Draw(GlowTexture.Value, pos + new Vector2(-8, -8), new(xPos, yPos + (Main.wallFrame[Type] * 178), xLength, 32), j > Main.worldSurface ? Color.White : col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
