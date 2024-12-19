using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Renderers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Content.Buffs;

namespace WizenkleBoss.Assets.Helper
{
    public class InkSystem : ModSystem
    {
            // easter eg
        public static Color OutlineColor { get; private set; }
        public override void OnWorldLoad()
        {
            string name = Main.LocalPlayer.name;
            if (name == "Zen")
                OutlineColor = Color.Plum;
            else if (name == "Wither" || name == "WizKid9000" || name == "White")
                OutlineColor = Color.Crimson;
            else if (name == "Buckle" || name == "BUKALE" || name == "Bicheal")
                OutlineColor = Color.Aqua;
            else if (name == "Violet")
                OutlineColor = Color.Violet;
            else if (name == "LESBIAN") // i luvb women
                OutlineColor = Main.DiscoColor;
            else
                OutlineColor = new(255, 230, 105);
        }
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

                DrawStars(spriteBatch, 50);
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
            if (AnyActiveInk())
                insideInkTargetByRequest.Request();
        }
        #endregion
        // This is so that the effect actually ends when youre not drugged asf
        public override void PostUpdateEverything()
        {
            InkShaderData.ToggleActivityIfNecessary();
        }
        public static bool AnyActiveInk() => 
            Main.projectile.Where(p => p.active && (p.ModProjectile is IDrawInk || p.ModProjectile is IDrawInInk)).Any() || 
            Main.npc.Where(npc => npc.active && (npc.ModNPC is IDrawInk || npc.ModNPC is IDrawInInk)).Any() || 
            Main.player.Where(p => p.active && (p.HasBuff<InkDrugStatBuff>() || p.HasBuff<InkDrugBuff>()) && p.dye.Length > 0).Any() || 
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
        public static void DrawStars(SpriteBatch spriteBatch, int num)
        {
            UnifiedRandom rand = new(Main.worldName.GetHashCode());
            for (int i = 0; i < num; i++)
            {
                float starRotation = rand.NextFloat(0, MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * rand.NextFloat(-0.2f, 0.2f);
                float starSize = rand.NextFloat(0.01f, 0.4f);
                
                Color starColor = (Color.White * starSize) with { A = 0 };

                Vector2 starPosition = new(((rand.NextFloat(Main.screenWidth + 50) + (Main.screenPosition.X * starSize)) % (Main.screenWidth + 50)) - 25, ((rand.NextFloat(Main.screenHeight + 50) + (Main.screenPosition.Y * starSize)) % (Main.screenHeight + 50)) - 25);
                starPosition = new Vector2(Main.screenWidth, Main.screenHeight) - starPosition;

                Texture2D starTexture = TextureRegistry.Star;
                Vector2 starOrigin = starTexture.Size() / 2;

                spriteBatch.Draw(starTexture, starPosition, null, starColor, starRotation, starOrigin, starSize, SpriteEffects.None, 0f);
            }
        }
        public interface IDrawInk
        {
            public void Shape();
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
        }
        public static void DrawVanishedPlayers()
        {
            foreach (var player in Main.player.Where(p => p.active && (p.HasBuff<InkDrugBuff>() || p.HasBuff<InkDrugStatBuff>()) && p.dye.Length > 0))
            {
                player.heldProj = -1;
                if (player.GetModPlayer<InkPlayer>().InGhostInk && player.GetModPlayer<InkPlayer>().InkDashCooldown > 0)
                {
                    float rot = player.GetModPlayer<InkPlayer>().DashVelocity.ToRotation() + MathHelper.PiOver2;

                    rot = MathF.Round(rot / MathHelper.PiOver4) * MathHelper.PiOver4;

                    if (player.GetModPlayer<InkPlayer>().InTile)
                    {
                        int count = (int)(((Main.GlobalTimeWrappedHourly * 60) % 60) / 4);
                        Rectangle frame = TextureRegistry.InkDash.Frame(1, 15, 0, count, 0, 0);
                        Main.spriteBatch.Draw(TextureRegistry.InkDash, player.Center - Main.screenLastPosition, frame, Color.White, rot, new Vector2(26), 1f, SpriteEffects.None, 0f);
                        count = (int)((((Main.GlobalTimeWrappedHourly + 20) * 60) % 60) / 4);
                        frame = TextureRegistry.InkDash.Frame(1, 15, 0, count, 0, 0);
                        Main.spriteBatch.Draw(TextureRegistry.InkDash, player.Center - Main.screenLastPosition, frame, Color.White * 0.5f, rot, new Vector2(26), 2f, SpriteEffects.None, 0f);
                    }
                    else
                    {
                        Main.spriteBatch.Draw(TextureRegistry.Star, player.Center - Main.screenLastPosition, null, Color.White with { A = 0 }, rot, TextureRegistry.Star.Size() / 2f, 0.8f * MathF.Sin(Main.GlobalTimeWrappedHourly * 10), SpriteEffects.None, 0f);
                        Main.spriteBatch.Draw(TextureRegistry.Star, player.Center - Main.screenLastPosition, null, Color.White with { A = 0 }, rot + MathHelper.PiOver4, TextureRegistry.Star.Size() / 2f, 1.3f * MathF.Sin(Main.GlobalTimeWrappedHourly * 14), SpriteEffects.None, 0f);
                    }
                    for (int k = player.GetModPlayer<InkPlayer>().dashOldPos.Length - 1; k > 0; k--)
                    {
                        float interpolator = (player.GetModPlayer<InkPlayer>().dashOldPos.Length - k) / (float)player.GetModPlayer<InkPlayer>().dashOldPos.Length;
                        Main.spriteBatch.Draw(TextureRegistry.Star, player.GetModPlayer<InkPlayer>().dashOldPos[k] - Main.screenLastPosition, null, (Color.White * interpolator) with { A = 0 }, rot + (MathHelper.PiOver4 * k), TextureRegistry.Star.Size() / 2f, 0.7f * MathF.Sin(Main.GlobalTimeWrappedHourly * interpolator * 5) * interpolator, SpriteEffects.None, 0f);
                    }
                }
                else
                {
                    Main.PlayerRenderer.DrawPlayer(Main.Camera, player, player.position - (Main.screenPosition - Main.screenLastPosition), player.fullRotation, player.fullRotationOrigin, 0);
                }
            }
        }
        public interface IDrawInInk
        {
            public void Shape();
        }
    }
    public class InkShaderData : ScreenShaderData
    {
        public const string ShaderKey = "InkScreen";
        public InkShaderData(Asset<Effect> shader, string passName)
            : base(shader, passName)
        {
        }
        public static void ToggleActivityIfNecessary()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            bool shouldBeActive = InkSystem.AnyActiveInk();

            if (shouldBeActive && !Filters.Scene[$"WizenkleBoss:{ShaderKey}"].IsActive())
                Filters.Scene.Activate($"WizenkleBoss:{ShaderKey}");

            if (!shouldBeActive && Filters.Scene[$"WizenkleBoss:{ShaderKey}"].IsActive())
                Filters.Scene.Deactivate($"WizenkleBoss:{ShaderKey}");
        }
        public override void Update(GameTime gameTime)
        {
            InkSystem.inkTargetByRequest.Request();

            if (InkSystem.inkTargetByRequest.IsReady && InkSystem.insideInkTargetByRequest.IsReady)
            {
                var player = Main.LocalPlayer.GetModPlayer<InkPlayer>();

                Shader.Parameters["embossColor"]?.SetValue(new Color(85, 25, 255, 255).ToVector4());

                Shader.Parameters["outlineColor"]?.SetValue(InkSystem.OutlineColor.ToVector4());

                Shader.Parameters["ScreenSize"]?.SetValue(new Vector2(Main.screenWidth, Main.screenHeight));

                Shader.Parameters["MaskThreshold"]?.SetValue(0.4f);

                Shader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

                Shader.Parameters["DrugStrength"]?.SetValue(player.Intoxication);
            }
        }
        public override void Apply()
        {
            var gd = Main.instance.GraphicsDevice;

            gd.SamplerStates[0] = SamplerState.LinearClamp;

            gd.Textures[1] = InkSystem.inkTargetByRequest.GetTarget();
            gd.SamplerStates[1] = SamplerState.LinearWrap;

            gd.Textures[2] = InkSystem.insideInkTargetByRequest.GetTarget();
            gd.SamplerStates[2] = SamplerState.LinearWrap;

            base.Apply();

            InkSystem.InsideInkTargetDrawnToThisFrame = false;
        }
    }
    [Autoload(Side = ModSide.Client)]
    public class InkEffect : ModSceneEffect
    {
        public override int Music => -1;
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals($"WizenkleBoss:{InkShaderData.ShaderKey}", isActive);
        }
        public override bool IsSceneEffectActive(Player player)
        {
            if (InkSystem.AnyActiveInk())
                return true;
            return false;
        }
    }
}
