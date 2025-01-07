using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Tiles;
using WizenkleBoss.Content.Rarities;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
            Item.width = 30;
            Item.width = 24;
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => true;
    }
}
