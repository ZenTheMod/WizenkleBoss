using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Tiles;
using WizenkleBoss.Content.Rarities;
using Terraria.ID;

namespace WizenkleBoss.Content.Items.Placeable
{
    public class ObservatorySatelliteDishItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ObservatorySatelliteDishTile>());
            Item.width = 44;
            Item.width = 46;
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.HallowedBar, 30)
                .AddIngredient(ItemID.AdamantiteBar, 10)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.AdamantiteForge)
                .Register();

            CreateRecipe(1)
                .AddIngredient(ItemID.HallowedBar, 30)
                .AddIngredient(ItemID.TitaniumBar, 10)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddTile(TileID.AdamantiteForge)
                .Register();
        }
    }
}
