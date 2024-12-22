using Terraria.ModLoader;
using Terraria;
using WizenkleBoss.Content.Rarities;
using WizenkleBoss.Content.Tiles.Paintings;

namespace WizenkleBoss.Content.Items.Placeable
{
    internal class ElegyForTheStarsItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ElegyForTheStarsTile>());
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
    }
}
