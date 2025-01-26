using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using WizenkleBoss.Common.Config;
using Terraria.GameContent.UI.Elements;
using Terraria;
using System.Linq;
using System;
using Terraria.Graphics.CameraModifiers;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;

namespace WizenkleBoss.Common.Helpers
{
    public static partial class Helper
    {
        /// <summary>
        /// Hides <paramref name="n"/> from the bestiary, useful for Projectile NPCs.
        /// </summary>
        /// <param name="n"></param>
        public static void HideFromBestiary(this ModNPC n)
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(n.Type, value);
        }
        /// <summary>
        /// Checks for any active boss.
        /// <para>Use <paramref name="type"/> to check for that NPC as a boss.</para>
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if a boss is active.</returns>
        public static bool AnyBoss(int type = -1)
        {
            bool flag1 = false;
            foreach (var n in Main.ActiveNPCs)
            {
                if (type > -1 && n.boss && n.type == type) { flag1 = true; }
                else if (type == -1 && (n.boss || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail)) { flag1 = true; }
            }
            return flag1;
        }
        /// <summary>
        /// Uses the vanilla screenshake but I made it dumber.
        /// </summary>
        /// <param name="modType"></param>
        /// <param name="startPosition"></param>
        /// <param name="direction"></param>
        /// <param name="strength"></param>
        /// <param name="vibrations"></param>
        /// <param name="frames"></param>
        /// <param name="fallOff"></param>
        public static void CameraShakeSimple(this ModType modType, Vector2 startPosition, Vector2 direction, float strength = 20f, float vibrations = 6f, int frames = 30, float fallOff = 1000f)
        {
            if (direction == Vector2.Zero)
                direction = (Main.rand.NextFloat()*((float)Math.PI*2f)).ToRotationVector2();

            float s = ModContent.GetInstance<VFXConfig>().ScreenShake / 100f;

            if (s <= 0)
                return;

            PunchCameraModifier modifier = new(startPosition, direction, strength * s, vibrations * s, frames, fallOff, modType.FullName);
            Main.instance.CameraModifiers.Add(modifier);
        }
        /// <summary>
        /// Generate a random string of garbage characters.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="badidea">Terrible idea.</param>
        /// <returns></returns>
        public static string RandomString(int length, bool badidea = false)
        {
            if (length >= 2000)
            {
                return "F"; // :(
            }
            string value = "";
                // i smashed my keyboard for this
            string text = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
                // this maybe a bad idea. but i dont care, i can do whatever i want :troll:
            if (badidea) { text += "!@#$%&*()-=_+~`[]{}|;'<>?,./"; }
            for (int i = 0; i < length; i++)
            {
                value += text[Main.rand.Next(0, text.Length)];
            }
            return value;
        }
        /// <summary>
        /// </summary>
        /// <param name="projectileID"></param>
        /// <returns>True if any projectile of type <paramref name="projectileID"/> are active.</returns>
        public static bool AnyProjectiles(int projectileID)
        {
            foreach (var projectile in Main.ActiveProjectiles)
            {
                if (projectile.type == projectileID)
                    return true;
            }
            return false;
        }
        /// <summary>
		/// Atttempts to find the top-left corner of a multitile at location (<paramref name="x"/>, <paramref name="y"/>)
		/// </summary>
		/// <param name="x">The tile X-coordinate</param>
		/// <param name="y">The tile Y-coordinate</param>
		/// <returns>The tile location of the multitile's top-left corner, or the input location if no tile is present or the tile is not part of a multitile</returns>
		public static Point16 GetTopLeftTileInMultitile(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            int frameX = 0;
            int frameY = 0;

            if (tile.HasTile)
            {
                int style = 0, alt = 0;
                TileObjectData.GetTileInfo(tile, ref style, ref alt);
                TileObjectData data = TileObjectData.GetTileData(tile.TileType, style, alt);

                if (data != null)
                {
                    int size = 16 + data.CoordinatePadding;

                    frameX = tile.TileFrameX % (size * data.Width) / size;
                    frameY = tile.TileFrameY % (size * data.Height) / size;
                }
            }

            return new Point16(x - frameX, y - frameY);
        }
        /// <summary>
		/// Uses <seealso cref="GetTopLeftTileInMultitile(int, int)"/> to try to get the entity bound to the multitile at (<paramref name="i"/>, <paramref name="j"/>).
		/// </summary>
		/// <typeparam name="T">The type to get the entity as</typeparam>
		/// <param name="i">The tile X-coordinate</param>
		/// <param name="j">The tile Y-coordinate</param>
		/// <param name="entity">The found <typeparamref name="T"/> instance, if there was one.</param>
		/// <returns><see langword="true"/> if there was a <typeparamref name="T"/> instance, or <see langword="false"/> if there was no entity present OR the entity was not a <typeparamref name="T"/> instance.</returns>
		public static bool TryGetTileEntityAs<T>(int i, int j, out T entity) where T : TileEntity
        {
            Point16 origin = GetTopLeftTileInMultitile(i, j);

                // TileEntity.ByPosition is a Dictionary<Point16, TileEntity> which contains all placed TileEntity instances in the world
                // TryGetValue is used to both check if the dictionary has the key, origin, and get the value from that key if it's there
            if (TileEntity.ByPosition.TryGetValue(origin, out TileEntity existing) && existing is T existingAsT)
            {
                entity = existingAsT;
                return true;
            }

            entity = null;
            return false;
        }
        public static bool IsOnGroundPrecise(this Player player)
        {
            var tile1 = Main.tile[(int)(player.position.X / 16f), (int)((player.position.Y + (player.gravDir == 1 ? player.Size.Y + 1 : -1)) / 16f)];
            var tile2 = Main.tile[(int)((player.position.X + player.Size.X) / 16f), (int)((player.position.Y + (player.gravDir == 1 ? player.Size.Y + 1 : -1)) / 16f)];

            return (tile1.HasTile && (Main.tileSolid[tile1.TileType] || Main.tileSolidTop[tile1.TileType]) && player.velocity.Y == 0f) ||
                (tile2.HasTile && (Main.tileSolid[tile2.TileType] || Main.tileSolidTop[tile2.TileType]) && player.velocity.Y == 0f);
        }
    }
}
