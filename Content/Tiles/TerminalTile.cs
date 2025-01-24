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
using WizenkleBoss.Common.Helpers;
using Terraria.Localization;
using WizenkleBoss.Content.UI;
using Terraria.UI;
using WizenkleBoss.Content.Projectiles.Misc;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using WizenkleBoss.Content.UI.Notes;

namespace WizenkleBoss.Content.Tiles
{
    public class TerminalTile : ModTile
    {
        private static Asset<Texture2D> GlowTexture;
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glowmask");
            }
            RegisterItemDrop(ModContent.ItemType<TerminalItem>());

            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = [16, 16];
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(142, 82, 82));
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
                zero = Vector2.Zero;

            Vector2 offsets = -Main.screenPosition + zero;

            int iX16 = i * 16;
            int jX16 = j * 16;
            Vector2 location = new(iX16, jX16);

            Vector2 drawCoordinates = location + offsets;

            Main.spriteBatch.Draw(GlowTexture.Value, drawCoordinates, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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

            Point16 point = (Point16)satelliteDish;
            StarMapUIHelper.CurrentTileWorldPosition = Helper.GetTopLeftTileInMultitile(point.X, point.Y).ToWorldCoordinates();
            StarMapUIHelper.CurrentTerminalWorldPosition = pos16.ToWorldCoordinates();

            StarMapUIHelper.TerminalAnim = 0f;
            StarMapUIHelper.TerminalState = ContactingState.None;

            BaseFancyUI.GenericOpenFancyUI(StarMapUIHelper.satelliteUI, player);

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
