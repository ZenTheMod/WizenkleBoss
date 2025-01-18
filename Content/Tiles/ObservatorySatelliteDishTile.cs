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
using WizenkleBoss.Common.Helpers;
using Terraria.Localization;
using WizenkleBoss.Content.UI;
using Terraria.UI;
using WizenkleBoss.Content.Projectiles.Misc;
using System.Runtime.InteropServices;

namespace WizenkleBoss.Content.Tiles
{
    public class ObservatorySatelliteDishTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<ObservatorySatelliteDishItem>());

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.Origin = new Point16(3, 4);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 18];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(142, 82, 82));
        }
    }
}
