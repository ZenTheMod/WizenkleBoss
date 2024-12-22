using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ObjectData;
using WizenkleBoss.Content.Items.Placeable;
using Terraria.Localization;

namespace WizenkleBoss.Content.Tiles.Paintings
{
    public class ElegyForTheRingsTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<ElegyForTheRingsItem>());

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileWaterDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16];
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = 7;

            AddMapEntry(new Color(99, 50, 30), Language.GetText("MapObject.Painting"));
        }
    }
}
