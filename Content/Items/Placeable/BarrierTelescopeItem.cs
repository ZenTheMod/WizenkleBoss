using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Tiles;
using WizenkleBoss.Content.Rarities;
using Terraria.ID;

namespace WizenkleBoss.Content.Items.Placeable
{
    public class BarrierTelescopeItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
                //ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BarrierTelescopeTile>());
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.GoldBar, 5)
                .AddIngredient(ItemID.Lens, 2)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe(1)
                .AddIngredient(ItemID.PlatinumBar, 5)
                .AddIngredient(ItemID.Lens, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
