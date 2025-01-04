using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Tiles;
using WizenkleBoss.Content.Rarities;
using Terraria.ID;

namespace WizenkleBoss.Content.Items.Placeable
{
    public class TerminalItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<TerminalTile>());
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
    }
}
