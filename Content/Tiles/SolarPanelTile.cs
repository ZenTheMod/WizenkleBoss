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
using WizenkleBoss.Content.TileEntities;

namespace WizenkleBoss.Content.Tiles
{
    public class SolarPanelTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<SolarPanelItem>());

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(2, 2);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;

            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<SolarPanelTileEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.UsesCustomCanPlace = true;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;

            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(142, 82, 82));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<SolarPanelTileEntity>().Kill(i, j);
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;

            if (player.Center.Y >= Main.worldSurface * 16)
                return false;

            Point16 pos16 = Helper.GetTopLeftTileInMultitile(i, j);

            SoundEngine.PlaySound(SoundID.Mech, player.Center);

            Main.mouseRightRelease = false;

            if (PowerDisplayUISystem.WorldPosition == pos16.ToWorldCoordinates())
                PowerDisplayUISystem.inUI = !PowerDisplayUISystem.inUI;
            else
            {
                PowerDisplayUISystem.WorldPosition = pos16.ToWorldCoordinates();
                PowerDisplayUISystem.WorldOffset = new Vector2(24, 0);

                PowerDisplayUISystem.Timer = 0f;
                PowerDisplayUISystem.inUI = true;
            }

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
