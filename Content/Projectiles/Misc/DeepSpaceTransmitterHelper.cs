using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Content.Dusts;
using MonoMod.Cil;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Content.Projectiles.Misc
{
    public class DeepSpaceTransmitterHelper : ModSystem
    {
            // There can only be ONE.
        public static float charge;
        public static float darkness;
        public static Vector2 Center;
        public class DeepSpaceTransmitterTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth / 2, Main.screenHeight / 2, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                Color color = Color.Lerp(new Color(255, 196, 255), new Color(255, 197, 147), charge) * darkness * 0.5f;
                color.A = 0;

                        // Draw the glowing part of the satellite dish.
                    // spriteBatch.Draw(ObservatorySatelliteDishTile.GlowTexture.Value, (Center - new Vector2(8) - Main.screenPosition) / 2f, null, color, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                    // Draw an even brighter bloom at the start of the laser.
                spriteBatch.Draw(TextureRegistry.Bloom.Value, (Center - Main.screenPosition) / 2f, null, color, 0f, TextureRegistry.Bloom.Value.Size() / 2, 0.6f * darkness + 0.01f, SpriteEffects.None, 0f);

                var Laser = Helper.TransmitShader;

                    // Param setup.
                Laser.Value.Parameters["globalTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
                Laser.Value.Parameters["laserColor"]?.SetValue(new Color(85, 0, 255, 255).ToVector4());
                Laser.Value.Parameters["baseColor"]?.SetValue(new Color(0, 0, 0, 0).ToVector4());

                    // Quantization
                Laser.Value.Parameters["stepSize"]?.SetValue(MathHelper.Lerp(18f, 4f, charge));

                    // Id like to mention this line in specific, I make charge use an exponential easing so that it extenuates the more 'pink'/'purple' hues rather than the 'sunset' colors.
                Laser.Value.Parameters["centerIntensity"].SetValue(Utils.Remap(MathF.Pow(2, 10 * (charge - 1)), 0f, 1f, 500f, 100000f));
                Laser.Value.Parameters["laserStartPercentage"].SetValue(MathHelper.Lerp(0.012f, 0.065f, charge));

                device.Textures[0] = TextureRegistry.Pixel.Value;

                    // I was fucking around in shadertoy for like an hour with custom textures and found that these two look the best.
                    // Also, for you bitchass theives who arent going to credit me, heres the shader :3 https://www.shadertoy.com/view/4fKyzt, or you can just ya-know, take it straight from the decomp you already have.
                device.Textures[1] = TextureRegistry.Space[1].Value;
                device.SamplerStates[1] = SamplerState.LinearWrap;

                device.Textures[2] = TextureRegistry.Wood.Value;
                if (StarMapUIHelper.TargetedStar != int.MaxValue && StarMapUIHelper.TargetedStar > -1)
                {
                    if (BarrierStarSystem.Stars[StarMapUIHelper.TargetedStar].Name.Contains("QUEER"))
                        device.Textures[2] = TextureRegistry.Rainbow.Value;
                }
                device.SamplerStates[2] = SamplerState.LinearWrap;

                Laser.Value.CurrentTechnique.Passes[0].Apply();
                
                    // Feel free to pr better ways I can do this :3
                foreach (var p in Main.ActiveProjectiles)
                {
                    if (p.ModProjectile is not IDrawLasers drawer)
                        continue;
                    drawer.Laser(spriteBatch, device);
                }

                Main.pixelShader.CurrentTechnique.Passes[0].Apply();

                foreach (var d in Main.dust.Where(d => d.active))
                {
                    if (DustLoader.GetDust(d.type) is not IDrawDustAboveDarkness drawer)
                        continue;
                    drawer.DrawAbove(spriteBatch, device, d);
                }

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }

            // Common rt stuff i do, 
            // TODO: make a proper rtcontentbyrequest loader.
        public static DeepSpaceTransmitterTargetContent deepSpaceTransmitterTargetByRequest;
        public override void Load()
        {
            deepSpaceTransmitterTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(deepSpaceTransmitterTargetByRequest);

            On_Main.DrawInfernoRings += DrawAfterInfernoRings;
        }

        public override void Unload()
        {
            Main.ContentThatNeedsRenderTargets.Remove(deepSpaceTransmitterTargetByRequest);

            On_Main.DrawInfernoRings -= DrawAfterInfernoRings;
        }

            // "this REEKS of CALAMITY CODE!"
            //                            - lion8cake

            // Using this detour as opposed to full IL edit, I honestly had no clue calamity uses similar detours everywhere, tbh it makes sense considering the placement of this method in Main.Do_Draw.
        private static void DrawAfterInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);

            if (Main.projectile.Where(p => p.active && p.ModProjectile is IDrawLasers).Any())
            {
                var snapshit = Main.spriteBatch.CaptureSnapshot();

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                    // Darken the screen to make the laser really POP.
                Main.spriteBatch.Draw(TextureRegistry.Pixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (darkness - 0.1f));

                    // Draw the laser rt.
                deepSpaceTransmitterTargetByRequest.Request();
                if (deepSpaceTransmitterTargetByRequest.IsReady)
                {
                    Main.spriteBatch.Draw(deepSpaceTransmitterTargetByRequest.GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0f);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(in snapshit);
            }
        }

            // Backup so that when you exit the world you dont get perma locked.
        public override void PostUpdateEverything()
        {
            if (charge > 0 || darkness > 0)
            {
                if (!Helper.AnyProjectiles(ModContent.ProjectileType<DeepSpaceTransmitter>()))
                {
                    charge = 0;
                    darkness = 0;
                }
            }
        }

            // Im doing this in the stupid rare chance gameraiders101 pulls a gameraiders101 and somehow breaks it idk :P
        public interface IDrawLasers
        {
            public void Laser(SpriteBatch spriteBatch, GraphicsDevice device);
        }

        public interface IDrawDustAboveDarkness 
        {
            public void DrawAbove(SpriteBatch spriteBatch, GraphicsDevice device, Dust dust);
        }
    }
}
