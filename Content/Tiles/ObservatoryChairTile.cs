    // https://media.discordapp.net/attachments/1155233672648208414/1310197807818473533/Z.png?ex=6744584b&is=674306cb&hm=9c4c2add9f77686ec64a6cabb6da27597fc0ec73a45f767223ea616544799014&=&format=webp&quality=lossless
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
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Common.Textures;
using Terraria.Localization;
using WizenkleBoss.Content.UI;
using Terraria.UI;

namespace WizenkleBoss.Content.Tiles
{
    public class ObservatoryChairTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<ObservatoryChairItem>());

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

            DustType = DustID.Iron;
            AdjTiles = [TileID.Chairs];

            TileID.Sets.CanBeSatOnForNPCs[Type] = true;
            TileID.Sets.CanBeSatOnForPlayers[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Chair"));

            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 0);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
        }

        public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            info.DirectionOffset = 0;
            info.VisualOffset = new Vector2(-8f, 0f);

            info.TargetDirection = -1;
            if (tile.TileFrameX >= 35)
                info.TargetDirection = 1;

            int xPos = tile.TileFrameX / 18;

            if (xPos == 1)
                i--;
            if (xPos == 2)
                i++;

            info.AnchorTilePosition.X = i;
            info.AnchorTilePosition.Y = j;
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                player.GamepadEnableGrappleCooldown();
                player.sitting.SitDown(player, i, j);
            }

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
            {
                return;
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<ObservatoryChairItem>();

            if (Main.tile[i, j].TileFrameX <= 35)
            {
                player.cursorItemIconReversed = true;
            }
        }
    }
}
