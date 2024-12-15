using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.GameContent;
using WizenkleBoss.Content.Items.Placeable;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;
using Terraria.Localization;
using WizenkleBoss.Content.UI;
using Terraria.UI;

namespace WizenkleBoss.Content.Tiles
{
    public class ObservatoryChartTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<ObservatoryChartItem>());

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileWaterDeath[Type] = false;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.Origin = new Point16(3, 3);
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
