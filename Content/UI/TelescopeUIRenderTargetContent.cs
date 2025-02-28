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

                Vector2 center = new(_target.Width / 2f, _target.Height / 2f);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                    // Draw the Sky.
                Texture2D skyTexture = TextureAssets.Background[Main.background].Value;
                Rectangle destinationRectangle = new(0, 0, _target.Width, _target.Height);

                spriteBatch.Draw(skyTexture, destinationRectangle, Main.ColorOfTheSkies);

                    // Draw the stars.
                DrawStars(spriteBatch, center + telescopeUIOffset, starAlpha);

                    // Store a million god damn values.
                Vector2 position = SunMoonPosition + telescopeUIOffset;
                position /= 1.5f;
                Color color = SunMoonColor;
                float rotation = SunMoonRotation;
                float scale = SunMoonScale / 1.5f;
                float centerX = Main.screenWidth / 2f;

                Color skyColor = Main.ColorOfTheSkies.MultiplyRGB(new Color(128, 168, 248));
                Color moonShadowColor = ModContent.GetInstance<VFXConfig>().TransparentMoonShadow ? Color.Transparent : skyColor;
                Color moonColor = skyColor.MultiplyRGB(color) * 16f * scale;

                    // For easier shader fuckery.
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                    // Draw the sun/moon.
                if (Main.dayTime)
                    SunAndMoonRendererSystem.DrawSun(spriteBatch, position, color, rotation, scale, centerX, device);
                else
                    SunAndMoonRendererSystem.DrawMoon(spriteBatch, position, color, rotation, scale, moonColor, moonShadowColor, device);

                    // Draw the lame clouds
                if (!RealisticSkyCompatSystem.DrawRealisticCloudsManually(Vector2.Zero, TargetSize, position))
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
                        return;

                    InteractableStar star = stars[i];

                    Vector2 position = center + star.position.RotatedBy(starRotation);
                    position /= 1.5f;

                    float rotation = star.rotation;
                    float twinkle = (MathF.Sin(star.twinkle + Main.GlobalTimeWrappedHourly / 12) * 0.2f) + 1f;
                    float scale = star.baseSize * (1 - star.compression) * twinkle;

                    Color color = star.GetColor() * star.baseSize * alpha;
                    Texture2D texture = TextureAssets.Star[star.starType].Value;
                    Vector2 origin = texture.Size() / 2f;

                    spriteBatch.Draw(texture, position, null, color, rotation, origin, scale / 2f, SpriteEffects.None, 0f);

                    spriteBatch.Draw(flare, position, null, (color * 0.13f) with { A = 0 }, 0, flareOrigin, scale / 4.5f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(flare, position, null, (color * 0.6f) with { A = 0 }, 0, flareOrigin, scale / 7f, SpriteEffects.None, 0f);
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
