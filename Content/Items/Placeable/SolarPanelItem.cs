using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Tiles;
using WizenkleBoss.Content.Rarities;
using Terraria.ID;

namespace WizenkleBoss.Content.Items.Placeable
{
    public class SolarPanelItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SolarPanelTile>());
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.HallowedBar, 4)
                .AddIngredient(ItemID.TungstenBar, 10)
                .AddIngredient(ItemID.Glass, 15)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe(1)
                .AddIngredient(ItemID.HallowedBar, 4)
                .AddIngredient(ItemID.SilverBar, 10)
                .AddIngredient(ItemID.Glass, 15)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
