using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;
using Terraria.Audio;
using WizenkleBoss.Content.Items.Placeable;
using WizenkleBoss.Common.Helper;
using Terraria.Localization;
using WizenkleBoss.Content.UI;
using Terraria.UI;
using WizenkleBoss.Content.Projectiles.Misc;

namespace WizenkleBoss.Content.Tiles
{
    public class TerminalTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<TerminalItem>());

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = [16, 16];
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

            Player player = Main.LocalPlayer;

            if (player.Center.Y >= Main.worldSurface * 16)
                return false;

            Point16 pos16 = Helper.GetTopLeftTileInMultitile(i, j);

            StarMapUIHelper.MapAccess = HasStarMap(pos16, out Point16? satelliteDish);

            if (satelliteDish == null)
                satelliteDish = new();

            SoundEngine.PlaySound(SoundID.Mech, player.Center);

            if (player.mount.Active)
            {
                player.mount.Dismount(player);
            }

            IngameFancyUI.CoverNextFrame();
            Main.ClosePlayerChat();

            Main.mouseRightRelease = false;

            Point16 point = (Point16)satelliteDish;
            StarMapUIHelper.CurrentTileWorldPosition = Helper.GetTopLeftTileInMultitile(point.X, point.Y).ToWorldCoordinates();

            StarMapUIHelper.TerminalAnim = 0f;
            StarMapUIHelper.TerminalState = ContactingState.None;

                // I HATE FANCY UI
            Main.ingameOptionsWindow = false;
            Main.playerInventory = false;
            Main.editChest = false;
            Main.npcChatText = string.Empty;
            Main.chatText = string.Empty;
            Main.inFancyUI = true;

            Main.InGameUI.SetState(StarMapUIHelper.satelliteUI);

            return true;
        }
        private bool HasStarMap(Point16 pos16, out Point16? satelliteDish)
        {
            Tile tile = Main.tile[pos16];

            satelliteDish = null;

            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
            if (data == null || tile == null || tile.TileType != Type)
                return false;

            Vector2 tileSize = new(data.Width, data.Height);
            if (WiringHelper.WireScanForTileType(pos16.X, pos16.Y, (int)tileSize.X, (int)tileSize.Y, ModContent.TileType<ObservatorySatelliteDishTile>(), out Point16? tl))
            {
                if (tl == null)
                    return false;

                satelliteDish = tl;

                return true;
            }
            return false;
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
