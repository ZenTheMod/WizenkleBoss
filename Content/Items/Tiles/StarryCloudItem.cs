using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using WizenkleBoss.Content.Rarities;
using WizenkleBoss.Content.Tiles;
using WizenkleBoss.Content.Tiles.Clouds;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace WizenkleBoss.Content.Items.Tiles
{
    public class StarryCloudItem : ModItem
    {
        private static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glowmask");
            }
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<StarryCloudTile>());
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient(ItemID.Cloud, 10)
                .AddIngredient(ItemID.FallenStar)
                .AddTile(TileID.SkyMill)
                .Register();
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Main.spriteBatch.Draw(GlowTexture.Value, Item.position - Main.screenPosition + (GlowTexture.Size() / 2), null, Color.White, rotation, GlowTexture.Size() / 2, scale, SpriteEffects.None, 0f);
        }
    }
}
