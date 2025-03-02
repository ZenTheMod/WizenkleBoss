using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using System.Security.Principal;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Registries;
using WizenkleBoss.Common.StarRewrite;
using static WizenkleBoss.Common.StarRewrite.StarSystem;
using static WizenkleBoss.Common.StarRewrite.SunAndMoonSystem;
using static WizenkleBoss.Common.StarRewrite.RealisticSkyCompatHelper;

namespace WizenkleBoss.Content.UI
{
    public partial class TelescopeUISystem : ModSystem
    {
        public static Vector2 TargetSize = new(540);
        public class TelescopeTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, (int)TargetSize.X, (int)TargetSize.Y, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, TelescopeMatrix);

                    // Draw the Sky.
                Texture2D skyTexture = TextureAssets.Background[Main.background].Value;
                Rectangle destinationRectangle = new(-1300, -1300, 2600, 1600);

                spriteBatch.Draw(skyTexture, destinationRectangle, Main.ColorOfTheSkies);

                    // Draw the stars.
                DrawStars(spriteBatch, Vector2.Zero, starAlpha);

                DrawRealisticGalaxy(destinationRectangle.Size() * 0.6f, 2600);

                    // Store a million god damn values.
                Vector2 position = SunMoonPosition - (SceneAreaSize * 0.5f);
                position.X *= 1.2f;

                Color color = SunMoonColor;
                float rotation = SunMoonRotation;
                float scale = SunMoonScale;

                if (Main.dayTime)
                    scale += MathF.Pow(2, 10 * ((blindnessCounter / 400f) - 1)) * 10f;

                float distanceFromCenter = MathF.Abs(position.X) / (SceneAreaSize.X * 0.5f);
                float distanceFromTop = (SunMoonPosition.Y + 50) / SceneAreaSize.Y;

                Color skyColor = Main.ColorOfTheSkies.MultiplyRGB(new Color(128, 168, 248));
                Color moonShadowColor = ModContent.GetInstance<VFXConfig>().TransparentMoonShadow ? Color.Transparent : skyColor;
                Color moonColor = skyColor.MultiplyRGB(color) * 16f * scale;

                    // For easier shader fuckery.
                if (RealisticSkyCompatSystem.RealisticSkyEnabled)
                {
                        // Could not compile input layout! Error Code: The parameter is incorrect (0x80070057)
                        // Could not compile input layout! Error Code: The parameter is incorrect (0x80070057)
                        // Could not compile input layout! Error Code: The parameter is incorrect (0x80070057)
                        // Could not compile input layout! Error Code: The parameter is incorrect (0x80070057)
                        // Could not compile input layout! Error Code: The parameter is incorrect (0x80070057)
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Matrix.CreateTranslation(-1, -1, 0f));

                    DrawRealisticStars(device, starAlpha, destinationRectangle.Size(), position, Matrix.Identity, Main.GlobalTimeWrappedHourly, Main.eclipse ? 0.05f : 0.3f);
                }

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, TelescopeMatrix);

                    // Draw the sun/moon.
                if (Main.dayTime)
                    SunAndMoonRendererSystem.DrawSun(spriteBatch, position, color, rotation, scale, distanceFromCenter, distanceFromTop, device);
                else
                    SunAndMoonRendererSystem.DrawMoon(spriteBatch, position, color, rotation, scale, moonColor, moonShadowColor, device);

                    // Draw the lame clouds
                if (!DrawRealisticClouds(Vector2.Zero, destinationRectangle, position + new Vector2(1300, 1300)));
                {
                        // Draw the normal clouds
                    //djisbvshivbfusdh
                }

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }

            private static void DrawStars(SpriteBatch spriteBatch, Vector2 center, float alpha)
            {
                Texture2D flare = Textures.Star.Value;
                Vector2 flareOrigin = flare.Size() / 2f;
                for (int i = 1; i < starCount - 1; i++)
                {
                    if (supernovae[i] >= (byte)SupernovaProgress.Exploding)
                        continue;

                    InteractableStar star = stars[i];

                    Vector2 position = center + star.position.RotatedBy(starRotation);

                    float rotation = star.rotation;
                    float twinkle = (MathF.Sin(star.twinkle + Main.GlobalTimeWrappedHourly / 12) * 0.2f) + 1f;
                    float scale = star.baseSize * (1 - star.compression) * twinkle;

                    Color color = star.GetColor() * star.baseSize * alpha;
                    Texture2D texture = TextureAssets.Star[star.starType].Value;
                    Vector2 origin = texture.Size() / 2f;

                    spriteBatch.Draw(texture, position, null, color, rotation, origin, scale / 1.3f, SpriteEffects.None, 0f);

                    spriteBatch.Draw(flare, position, null, (color * 0.13f) with { A = 0 }, 0, flareOrigin, scale / 2.5f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(flare, position, null, (color * 0.6f) with { A = 0 }, 0, flareOrigin, scale / 5f, SpriteEffects.None, 0f);
                }
            }
        }

        public static TelescopeTargetContent telescopeTargetByRequest;
        public override void Load()
        {
            telescopeTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(telescopeTargetByRequest);
        }
        public override void Unload()
        {
            Main.ContentThatNeedsRenderTargets.Remove(telescopeTargetByRequest);
        }
    }
}
