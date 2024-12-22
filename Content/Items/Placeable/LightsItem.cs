using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Rarities;
using WizenkleBoss.Content.Tiles.Paintings;

namespace WizenkleBoss.Content.Items.Placeable
{
    public class LightsItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LightsTile>());
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
    }
}
