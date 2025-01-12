using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using Terraria.Utilities;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.NPCs.InkCreature;

namespace WizenkleBoss.Common.Ink
{
    public class InkSystem : ModSystem
    {
        public static Color OutlineColor { get; private set; } = new(255, 230, 105); // just so i can change it later

            // Lame rt setup
        #region RenderTargetSetup

        public class InkTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);

                DrawInk();
                Main.LocalPlayer.GetModPlayer<InkPlayer>().DrawGoo();

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }

        public static InkTargetContent inkTargetByRequest;

        public class InsideInkTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);

                DrawInInk();

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
                InsideInkTargetDrawnToThisFrame = true;
            }
        }
        public static InsideInkTargetContent insideInkTargetByRequest;

        public static bool InsideInkTargetDrawnToThisFrame = false;

        public override void Load()
        {
            inkTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(inkTargetByRequest);

            insideInkTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(insideInkTargetByRequest);

            On_Main.CheckMonoliths += DrawInsideInkTarget;
        }

        public override void Unload()
        {
            Main.ContentThatNeedsRenderTargets.Remove(inkTargetByRequest);

            Main.ContentThatNeedsRenderTargets.Remove(insideInkTargetByRequest);

            On_Main.CheckMonoliths -= DrawInsideInkTarget;
        }
            // This makes sure that the target gets drawn to BEFORE the player normally draws.
        private void DrawInsideInkTarget(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            if (Main.gameMenu)
                return;
            if (AnyActiveInk)
                insideInkTargetByRequest.Request();
        }

        #endregion
            // This is so that the effect actually ends when youre not drugged asf
        public override void PostUpdateEverything()
        {
            AnyActiveInk = _AnyActiveInk();

            InkShaderData.ToggleActivityIfNecessary();
        }
        public static bool AnyActiveInk;

        private static bool _AnyActiveInk() =>
            Main.projectile.Where(p => p.active && (p.ModProjectile is IDrawInk || p.ModProjectile is IDrawInInk)).Any() ||
            Main.npc.Where(npc => npc.active && (npc.ModNPC is IDrawInk || npc.ModNPC is IDrawInInk)).Any() ||
            Main.LocalPlayer.GetModPlayer<InkPlayer>().InkBuffActive ||
            Main.LocalPlayer.GetModPlayer<InkPlayer>().Intoxication > 0f;

        public static void DrawInk()
        {
            foreach (var p in Main.ActiveProjectiles)
            {
                if (!p.active || p.ModProjectile is not IDrawInk drawer)
                    continue;
                drawer.Shape();
            }
            foreach (var npc in Main.ActiveNPCs)
            {
                if (!npc.active || npc.ModNPC is not IDrawInk drawer)
                    continue;
                drawer.Shape();
            }

        }

        public static void DrawInInk()
        {
            foreach (var p in Main.ActiveProjectiles)
            {
                if (!p.active || p.ModProjectile is not IDrawInInk drawer)
                    continue;
                drawer.Shape();
            }
            foreach (var npc in Main.ActiveNPCs)
            {
                if (!npc.active || npc.ModNPC is not IDrawInInk drawer)
                    continue;
                drawer.Shape();
            }

            DrawVanishedPlayers();

            InkCreatureHelper bh = ModContent.GetInstance<InkCreatureHelper>();
            if (bh is IDrawInInk bhdrawer)
                bhdrawer.Shape();
        }

        public static void DrawVanishedPlayers()
        {
            foreach (var player in Main.player.Where(p => p.active && p.GetModPlayer<InkPlayer>().InkBuffActive && p.dye.Length > 0))
            {
                player.heldProj = -1;
                if (player.GetModPlayer<InkPlayer>().InGhostInk && player.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
                {
                    float rot = player.GetModPlayer<InkPlayer>().DashVelocity.ToRotation() + MathHelper.PiOver2;

                    rot = MathF.Round(rot / MathHelper.PiOver4) * MathHelper.PiOver4;

                    if (player.GetModPlayer<InkPlayer>().InTile)
                    {
                        int count = (int)(Main.GlobalTimeWrappedHourly * 60 % 60 / 4);
                        Rectangle frame = TextureRegistry.InkDash.Value.Frame(1, 15, 0, count, 0, 0);
                        Main.spriteBatch.Draw(TextureRegistry.InkDash.Value, player.Center - Main.screenLastPosition, frame, Color.White, rot, new Vector2(26), 1f, SpriteEffects.None, 0f);

                        count = (int)((Main.GlobalTimeWrappedHourly + 20) * 60 % 60 / 4);
                        frame = TextureRegistry.InkDash.Value.Frame(1, 15, 0, count, 0, 0);
                        Main.spriteBatch.Draw(TextureRegistry.InkDash.Value, player.Center - Main.screenLastPosition, frame, Color.White * 0.5f, rot, new Vector2(26), 2f, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        Main.spriteBatch.Draw(TextureRegistry.Star.Value, player.Center - Main.screenLastPosition, null, Color.White with { A = 0 }, rot, TextureRegistry.Star.Size() / 2f, 0.8f * MathF.Sin(Main.GlobalTimeWrappedHourly * 10), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(TextureRegistry.Star.Value, player.Center - Main.screenLastPosition, null, Color.White with { A = 0 }, rot + MathHelper.PiOver4, TextureRegistry.Star.Size() / 2f, 1.3f * MathF.Sin(Main.GlobalTimeWrappedHourly * 14), SpriteEffects.None, 0f);
                    }
                    for (int k = player.GetModPlayer<InkPlayer>().dashOldPos.Length - 1; k > 0; k--)
                    {
                        float interpolator = (player.GetModPlayer<InkPlayer>().dashOldPos.Length - k) / (float)player.GetModPlayer<InkPlayer>().dashOldPos.Length;
                        Main.spriteBatch.Draw(TextureRegistry.Star.Value, player.GetModPlayer<InkPlayer>().dashOldPos[k] - Main.screenLastPosition, null, (Color.White * interpolator) with { A = 0 }, rot + MathHelper.PiOver4 * k, TextureRegistry.Star.Size() / 2f, 0.7f * MathF.Sin(Main.GlobalTimeWrappedHourly * interpolator * 5) * interpolator, SpriteEffects.None, 0f);
                    }
                }
                else
                {
                    Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position - (Main.screenPosition - Main.screenLastPosition), player.fullRotation, player.fullRotationOrigin, 0);
                }
            }
        }
    }
}
