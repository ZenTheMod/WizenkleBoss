using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Content.Rarities;
using WizenkleBoss.Content.Tiles;
using WizenkleBoss.Content.Walls;

namespace WizenkleBoss.Content.Items.Tiles
{
    public class StarryCloudWallItem : ModItem
    {
        private static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glowmask");
            }
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<StarryCloudWall>());
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<StarryCloudItem>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Main.spriteBatch.Draw(GlowTexture.Value, Item.position - Main.screenPosition + (GlowTexture.Size() / 2), null, Color.White, rotation, GlowTexture.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }
}
