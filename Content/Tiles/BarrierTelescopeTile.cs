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
    public class BarrierTelescopeTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<BarrierTelescopeItem>());

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSpelunker[Type] = true;

            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 18];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(142, 82, 82));
        }
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.Center.Distance(new Point(i, j).ToWorldCoordinates()) >= 140 || Main.LocalPlayer.Center.Y >= Main.worldSurface * 16)
                return false;
                // All of this is what lets the menu be the only thing on screen.

                // Demount to prevent even more movement (bad)
            if (Main.LocalPlayer.mount.Active)
            {
                Main.LocalPlayer.mount.Dismount(Main.LocalPlayer);
            }

            TelescopeUISystem.telescopeTilePosition = new Point(i, j).ToWorldCoordinates();
                // I HATE FANCY UI
            IngameFancyUI.CoverNextFrame();
            Main.ClosePlayerChat();

            Main.ingameOptionsWindow = false;
            Main.playerInventory = false;
            Main.editChest = false;
            Main.npcChatText = string.Empty;
            Main.chatText = string.Empty;
            Main.inFancyUI = true;

            Main.InGameUI.SetState(TelescopeUISystem.barrierTelescopeUI);
                // IngameFancyUI.OpenUIState(BarrierTelescopeUISystem.barrierTelescopeUI); wouldnt work because this game is coded perfectly :3
            return true;
        }
            // Assigning to Main.hoverItemName to make text appear next to the mouse doesn't work in fancy UI
            // You have to use Main.instance.MouseText(str)
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            if (player.Center.Distance(new Point(i, j).ToWorldCoordinates()) >= 140 || Main.LocalPlayer.Center.Y >= Main.worldSurface * 16)
                return;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = -1;
            player.cursorItemIconText = Language.GetTextValue("Mods.WizenkleBoss.Tiles.BarrierTelescopeTile.Hover"); // "Peer"
        }
    }
}
