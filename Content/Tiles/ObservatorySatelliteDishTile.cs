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
using WizenkleBoss.Content.Projectiles.Misc;
using System.Runtime.InteropServices;

namespace WizenkleBoss.Content.Tiles
{
    public class ObservatorySatelliteDishTile : ModTile
    {
        public static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow");
            }
            RegisterItemDrop(ModContent.ItemType<ObservatorySatelliteDishItem>());

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSpelunker[Type] = true;

            TileID.Sets.HasOutlines[Type] = true;
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
        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;
        public override bool RightClick(int i, int j)
        {
            if (Helper.AnyProjectiles(ModContent.ProjectileType<DeepSpaceTransmitter>()))
                return false;

            if (Main.player.Where(p => p.active && p.Center.Distance(new Point(i, j).ToWorldCoordinates()) >= 400).Any())
                return false;

            Player player = Main.LocalPlayer;

            if (player.Center.Y >= Main.worldSurface * 16)
                return false;

            SoundEngine.PlaySound(SoundID.Mech, player.Center);

            if (player.mount.Active)
            {
                player.mount.Dismount(player);
            }
            Point16 pos16 = Helper.GetTopLeftTileInMultitile(i, j);

            IngameFancyUI.CoverNextFrame();
            Main.ClosePlayerChat();
                // I HATE FANCY UI
            Main.ingameOptionsWindow = false;
            Main.playerInventory = false;
            Main.editChest = false;
            Main.npcChatText = string.Empty;
            Main.chatText = string.Empty;
            Main.inFancyUI = true;

            ObservatorySatelliteDishUISystem.satelliteTilePosition = pos16.ToWorldCoordinates();

            ObservatorySatelliteDishUISystem.logtimer = 0f;
            ObservatorySatelliteDishUISystem.ErrorState = ContactingState.None;

            Main.InGameUI.SetState(ObservatorySatelliteDishUISystem.observatorySatelliteDishUI);

            return true;
        }
        public override void MouseOver(int i, int j)
        {
            if (Helper.AnyProjectiles(ModContent.ProjectileType<DeepSpaceTransmitter>()))
                return;
            Player player = Main.LocalPlayer;
            if (player.Center.Distance(new Point(i, j).ToWorldCoordinates()) >= 140 || Main.LocalPlayer.Center.Y >= Main.worldSurface * 16)
                return;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = -1;
            player.cursorItemIconText = Language.GetTextValue("Mods.WizenkleBoss.Tiles.ObservatorySatelliteDish.Hover");
        }
    }
}
