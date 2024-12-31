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

namespace WizenkleBoss.Common.Helper
{
    public static partial class Helper
    {
        /// <summary>
        /// Measures the text <paramref name="text"/> with the font <paramref name="font"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static Vector2 MeasureString(this string text, DynamicSpriteFont font = null)
        {
            if (font == null) { font = FontAssets.MouseText.Value; }
            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
            return ChatManager.GetStringSize(font, snippets, Vector2.One);
        }
        /// <summary>
        /// A simple utility that gets an <see cref="Projectile"/>'s <see cref="Projectile.ModProjectile"/> instance as a specific type without having to do clunky casting.
        /// </summary>
        /// <typeparam name="T">The ModProjectile type to convert to.</typeparam>
        /// <param name="p">The Projectile to access the ModProjectile from.</param>
        public static T As<T>(this Projectile p) where T : ModProjectile
        {
            return p.ModProjectile as T;
        }
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
        /// Swaps <paramref name="itemToReplace"/> with <paramref name="itemtoReplaceWith"/>.
        /// </summary>
        /// <param name="player">The player swapping items. Don't run this server side.</param>
        /// <param name="itemToReplace">The item to replace</param>
        /// <param name="itemtoReplaceWith">The item to replace with</param>
        public static void SwapItems(this Player player, Item itemToReplace, int itemtoReplaceWith)
        {
            bool foundSlot = false;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i] == itemToReplace)
                {
                    Item newItem = new(itemtoReplaceWith, itemToReplace.stack, itemToReplace.prefix);
                    newItem.active = true;
                    newItem.favorited = itemToReplace.favorited;
                    player.inventory[i] = newItem;
                    itemToReplace.active = false;
                    foundSlot = true;
                    break;
                }
            }
            if (!foundSlot) //inventory array was full, drop it on the ground instead
            {
                Item.NewItem(player.GetSource_ItemUse(itemToReplace), player.Center, itemtoReplaceWith, prefixGiven: itemToReplace.prefix);
            }
        }
        /// <summary>
        /// Shorthand of necessary code to kill an NPC properly.
        /// <para>Does the following:</para>
        /// <code>npc.life = 0;
        /// npc.HitEffect(0, 10.0);
        /// npc.checkDead();
        /// npc.active = false;</code>
        /// </summary>
        /// <param name="npc">Target</param>
        public static void KillNPC(this NPC npc)
        {
            npc.life = 0;
            npc.HitEffect(0, 10.0);
            npc.checkDead();
            npc.active = false;
        }
        /// <summary>
        /// </summary>
        /// <param name="player"></param>
        /// <returns>The <paramref name="player"/>'s held item.</returns>
        public static Item GetActiveItem(this Player player) => player.HeldItem;
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

            PunchCameraModifier modifier = new(startPosition, direction, strength, vibrations, frames, fallOff, modType.FullName);
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
        /// UI panel setup shorthand.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="size"></param>
        /// <param name="offset"></param>
        public static void SimpleSetupButton(this UIPanel button, Vector2 size, Vector2 offset)
        {
            button.Width.Set(size.X, 0);
            button.Height.Set(size.Y, 0);
            //button3.HAlign = 0.5f;
            button.Top.Set(offset.Y, 0);
            button.Left.Set(offset.X, 0f);
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
        /// Detects nearby hostile NPCs from a given point
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="ignoreTiles">Whether to ignore tiles when finding a target or not</param>
        /// <param name="bossPriority">Whether bosses should be prioritized in targetting or not</param>
        public static NPC ClosestNPCAt(this Vector2 origin, float maxDistanceToCheck)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            bool bossFound = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                    // If we've found a valid boss target, ignore ALL targets which aren't bosses.
                if (bossFound && !(npc.boss || npc.type == NPCID.WallofFleshEye))
                    continue;

                if (npc.CanBeChasedBy(null, false))
                {
                    if (Vector2.Distance(origin, npc.Center) < distance)
                    {
                        if (npc.boss || npc.type == NPCID.WallofFleshEye)
                            bossFound = true;

                        distance = Vector2.Distance(origin, npc.Center);
                        closestTarget = npc;
                    }
                }
            }
            return closestTarget;
        }
        /// <summary>
        /// Detects nearby hostile NPCs from a given point with minion support
        /// </summary>
        /// <param name="origin">The position where we wish to check for nearby NPCs</param>
        /// <param name="maxDistanceToCheck">Maximum amount of pixels to check around the origin</param>
        /// <param name="owner">Owner of the minion</param>
        public static NPC MinionHoming(this Vector2 origin, float maxDistanceToCheck, Player owner, bool checksRange = false)
        {
            if (owner is null || owner.whoAmI !<= Main.maxPlayers || owner.MinionAttackTargetNPC !<= Main.maxNPCs)
                return ClosestNPCAt(origin, maxDistanceToCheck);
            NPC npc = Main.npc[owner.MinionAttackTargetNPC];
            float extraDistance = (npc.width / 2) + (npc.height / 2);
            bool distCheck = Vector2.Distance(origin, npc.Center) < (maxDistanceToCheck + extraDistance) || !checksRange;
            if (owner.HasMinionAttackTargetNPC && distCheck)
            {
                return npc;
            }
            return ClosestNPCAt(origin, maxDistanceToCheck);
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
        public static bool IsOnGroundPrecise(this Player player)
        {
            var tile1 = Main.tile[(int)(player.position.X / 16f), (int)((player.position.Y + (player.gravDir == 1 ? player.Size.Y + 1 : -1)) / 16f)];
            var tile2 = Main.tile[(int)((player.position.X + player.Size.X) / 16f), (int)((player.position.Y + (player.gravDir == 1 ? player.Size.Y + 1 : -1)) / 16f)];

            return (tile1.HasTile && (Main.tileSolid[tile1.TileType] || Main.tileSolidTop[tile1.TileType]) && player.velocity.Y == 0f) ||
                (tile2.HasTile && (Main.tileSolid[tile2.TileType] || Main.tileSolidTop[tile2.TileType]) && player.velocity.Y == 0f);
        }
    }
}
