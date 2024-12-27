using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using WizenkleBoss.Assets.Config;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Content.Tiles;
using WizenkleBoss.Content.UI;
using static WizenkleBoss.Assets.Helper.InkSystem;
using static WizenkleBoss.Content.Projectiles.Misc.DeepSpaceTransmitter;

namespace WizenkleBoss.Content.Projectiles.Misc
{
        // Im doing this in the stupid rare chance gameraiders101 pulls a gameraiders101 and somehow breaks it idk :P
    public interface IDrawLasers
    {
        public void Laser(SpriteBatch spriteBatch, GraphicsDevice device);
    }
        // Super stupid in the long run.
    public enum LaserState
    {
        FadeIn,
        Open,
        FadeOut
    }
    public class DeepSpaceTransmitter : ModProjectile, IDrawLasers
    {
        public static float charge;
        public static float darkness;
        private static Vector2[] Points = new Vector2[40];
        private static Vector2 Center;
        public class DeepSpaceTransmitterTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth / 2, Main.screenHeight / 2, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                spriteBatch.Draw(ObservatorySatelliteDishTile.GlowTexture.Value, (Center - new Vector2(8) - Main.screenPosition) / 2f, null, (Color.Lerp(new Color(255, 196, 255), new Color(255, 197, 147), charge) * charge * 0.2f) with { A = 0 }, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Bloom, (Center - Main.screenPosition) / 2f, null, (Color.Lerp(new Color(255, 196, 255), new Color(255, 197, 147), charge) * darkness * 0.5f) with { A = 0 }, 0f, TextureRegistry.Bloom.Size() / 2, 0.6f * darkness + 0.01f, SpriteEffects.None, 0f);

                for (int i = 0; i < Points.Length - 1; i++)
                    spriteBatch.Draw(TextureRegistry.Bloom, (Points[i] - Main.screenPosition) / 2, null, (Color.Lerp(new Color(255, 196, 255), new Color(255, 197, 147), charge)) with { A = 0 }, 0, TextureRegistry.Bloom.Size() / 2, .03f, SpriteEffects.None, 0f);
                
                var Laser = Helper.TransmitShader;

                    // Param setup.
                Laser.Value.Parameters["globalTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
                Laser.Value.Parameters["laserColor"]?.SetValue(new Color(85, 0, 255, 255).ToVector4());
                Laser.Value.Parameters["baseColor"]?.SetValue(new Color(0, 0, 0, 0).ToVector4());

                    // Quantization
                Laser.Value.Parameters["stepSize"]?.SetValue(MathHelper.Lerp(18f, 3f, charge));

                    // Id like to mention this line in specific, I make charge use an exponential easing so that it extenuates the more 'pink'/'purple' hues rather than the 'sunset' colors.
                Laser.Value.Parameters["centerIntensity"].SetValue(Utils.Remap(MathF.Pow(2, 10 * (charge - 1)), 0f, 1f, 500f, 100000f));
                Laser.Value.Parameters["laserStartPercentage"].SetValue(MathHelper.Lerp(0.012f, 0.035f, charge));

                    // I was fucking around in shadertoy for like an hour with custom textures and found that these two look the best.
                    // Also, for you bitchass theives who arent going to credit me, heres the shader :3 https://www.shadertoy.com/view/4fKyzt
                device.Textures[1] = TextureRegistry.Space[1];
                device.SamplerStates[1] = SamplerState.LinearWrap;
                device.Textures[2] = TextureRegistry.Wood;
                device.SamplerStates[2] = SamplerState.LinearWrap;

                Laser.Value.CurrentTechnique.Passes[0].Apply();


                foreach (var p in Main.ActiveProjectiles)
                {
                    if (!p.active || p.ModProjectile is not IDrawLasers drawer)
                        continue;
                    drawer.Laser(spriteBatch, device);
                }

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }
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

        private static void DrawAfterInfernoRings(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);

            if (Main.projectile.Where(p => p.active && p.ModProjectile is IDrawLasers).Any())
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                Main.spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (darkness - 0.1f));

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                deepSpaceTransmitterTargetByRequest.Request();
                if (deepSpaceTransmitterTargetByRequest.IsReady)
                {
                    Main.spriteBatch.Draw(deepSpaceTransmitterTargetByRequest.GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0f);
                }
                Main.spriteBatch.ResetToDefault();
            }
        }

        public void Laser(SpriteBatch spriteBatch, GraphicsDevice device)
        {
            if (charge > 0)
                spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle((int)(Projectile.Center.X - Main.screenPosition.X) / 2, (int)(Projectile.Center.Y - Main.screenPosition.Y) / 2, 1500, (int)(420 * charge) + 4), null, Color.White, (-Vector2.One).ToRotation(), new Vector2(TextureRegistry.Pixel.Width * 0.01f, TextureRegistry.Pixel.Height / 2f), SpriteEffects.None, 0f);
        }
        public override string Texture => "WizenkleBoss/Assets/Textures/MagicPixel";
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 9000;

            Projectile.hide = true;
        }
        private LaserState laserState = 0;
        private bool SoundPlayed = false;
        private int counter;
        public override void AI()
        {
            Center = Projectile.Center;
            for (int i = 0; i < Points.Length - 1; i++)
            {
                if (Points[i] != Vector2.Zero)
                    Points[i] = Projectile.timeLeft > 8998 || laserState == LaserState.FadeOut ? Vector2.Zero : Vector2.Lerp(Points[i], Projectile.Center, 0.09f);
            }
            if (charge > 0.02)
            {
                if (BarrierTelescopeUISystem.inUI || StarMapUIHelper.inUI)
                {
                    Main.menuMode = 0;
                    IngameFancyUI.Close();
                }
            }
            switch (laserState)
            {
                default:
                    FadeInAI();
                    break;
                case LaserState.FadeIn:
                    FadeInAI();
                    break;
                case LaserState.Open:
                    OpenAI(); // omg just like the shitty ai company !!!!! yayyy i love stealing work and profiting off of it !!!!!!!!!!!!!!!!!!!!
                    break;
                case LaserState.FadeOut:
                    FadeOutAI();
                    break;
            }
        }
        private void FadeInAI()
        {
            if (Main.rand.NextBool())
            {
                int n = Main.rand.Next(Points.Length);
                if (Points[n].Distance(Projectile.Center) < 1 || Points[n].Length() < 2)
                {
                    Points[n] = Projectile.Center + Main.rand.NextVector2CircularEdge(224, 224);
                }
            }
            if (!SoundPlayed)
            {
                    // dont defean the elderly
                if (ModContent.GetInstance<WizenkleBossConfig>().LaserLoop)
                    SoundEngine.PlaySound(AudioRegistry.SateliteDeathray);
                SoundPlayed = true;
            }
            MusicKiller.MuffleFactor = 1f - ((float)counter / 200f);
            if (counter >= 200)
            {
                if (darkness >= 0.3f)
                    this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 18, 7, 2, 0);
                if (charge >= 0.1f && darkness >= 1f)
                {
                    laserState = LaserState.Open;
                    counter = 0;
                    return;
                }
                if (SoundPlayed && charge < 0.1f)
                    charge += 0.001f;
                if (darkness < 1f)
                    darkness += 0.03f;
            }
            else
            {
                counter++;
            }
        }
        private void OpenAI()
        {
            MusicKiller.MuffleFactor = 0f;
            this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 40, 40, 2, 0);
            if (++counter > 435)
            {
                laserState = LaserState.FadeOut;
                counter = 0;
                return;
            }
            if (charge < 1f)
            {
                charge += 0.2f;
            }
        }
        private void FadeOutAI()
        {
            MusicKiller.MuffleFactor = 0f;
            if (darkness <= 0f)
            {
                counter = 0;

                if (BarrierStarSystem.BigStar.State == SupernovaState.None && StarMapUIHelper.TargetedStar == int.MaxValue)
                    BarrierStarSystem.BigStar.State = SupernovaState.Shrinking;
                if (StarMapUIHelper.TargetedStar != int.MaxValue && StarMapUIHelper.TargetedStar > -1)
                {
                    if (BarrierStarSystem.Stars[StarMapUIHelper.TargetedStar].State == SupernovaState.None)
                        BarrierStarSystem.Stars[StarMapUIHelper.TargetedStar].State = SupernovaState.Shrinking;
                }
                Projectile.Kill();
            }
            else
            {
                darkness -= 0.0015f;
            }

            if (charge > 0.4f)
            {
                this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 12, 19, 2, 0);
                charge *= 0.98f;
            }
            else if (charge > 0f)
            {
                this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 2, 11, 2, 0);
                charge -= 0.002f;
            }
        }
    }
}
