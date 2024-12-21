using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;
using WizenkleBoss.Assets.Config;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;

namespace WizenkleBoss.Content.UI
{
    public partial class ObservatorySatelliteDishUISystem : ModSystem
    {
            // THATS SICK AND TWIIISSSTTEDDDD :333333
        public static Vector2 TargetSize = new(540);
        public class SatelliteDishTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, (int)TargetSize.X, (int)TargetSize.Y, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                    // i hate my chud life
                float overallcolormultiplier = 2 - MathF.Pow(2f, 10 * (openAnimation - 1));

                spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle(0, 0, (int)TargetSize.X, (int)TargetSize.Y), new Color(11, 8, 18) * Utils.Remap(overallcolormultiplier, 1, 2, 1, 10));
                    // Stars.

                    // debug code:
                        //Vector2 osition = new Vector2((_target.Width / 2f) - 1400, (_target.Height / 2f) - 1400) + satelliteUIOffset;
                        //spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle((int)osition.X * 3, (int)osition.Y * 3, 2800 * 3, 2800 * 3), Color.Red * 0.4f);

                DynamicSpriteFont font = FontRegistry.SpaceMono;

                Vector2 Center = new(_target.Width / 2f, _target.Height / 2f);

                Color BloomColor = new Color(214, 196, 255) * 0.04f * overallcolormultiplier;
                BloomColor.A = 0;

                DrawStars(spriteBatch, Center, BloomColor, font);

                    // Draw the weird bracket selector inspired by you know what at this point.

                DrawSelection(spriteBatch, new(214, 196, 255), Center);

                DrawTextPrompt(spriteBatch, Center + new Vector2(0, Center.Y / 2), BloomColor);

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }
        public static void DrawTextPrompt(SpriteBatch spriteBatch, Vector2 Center, Color BloomColor)
        {
            if (promptclose == 0)
                return;
            DynamicSpriteFont font = FontRegistry.SpaceMono;

            string PromptText = prompt switch
            {
                ComplexPromptState.MovementKeys => Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPrompt"),
                ComplexPromptState.Zoom => Language.GetTextValue("Mods.WizenkleBoss.UI.SatelliteDish.ZoomKeyPrompt"),
                ComplexPromptState.Select => Language.GetTextValue("Mods.WizenkleBoss.UI.SatelliteDish.SelectKeyPrompt"),
                _ => " "
            };

            Vector2 fontSizePrompt = Helper.MeasureString(PromptText, font);

            Vector2 fontSizeMovementUnder = Helper.MeasureString(Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPromptUnder"), font);

            float size = 0.4f * promptclose;

            Rectangle backing = new(-(int)(Center.X * 2f), (int)((Center.Y - (fontSizePrompt.Y * size)) - (17 * promptclose)), (int)(Center.X * 6f), (int)((fontSizePrompt.Y * 1.2f * size) + (34 * promptclose)));
            spriteBatch.Draw(TextureRegistry.Pixel, backing with { Y = (int)((Center.Y - (fontSizePrompt.Y * size)) - (19 * promptclose)), Height = (int)((fontSizePrompt.Y * 1.2f * size) + (38 * promptclose)) }, BloomColor * 10f);
            spriteBatch.Draw(TextureRegistry.Pixel, backing, new Color(11, 8, 18));
            spriteBatch.Draw(TextureRegistry.Bloom, backing, BloomColor * 4f);

            if (prompt == ComplexPromptState.None)
                return;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, PromptText, Center, Color.White, 0, new(fontSizePrompt.X / 2f, fontSizePrompt.Y), Vector2.One * size);

            ChatManager.DrawColorCodedString(spriteBatch, font, Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPromptUnder"), Center, Color.Gray with { A = 0 }, 0, new(fontSizeMovementUnder.X / 2f, 0), Vector2.One * size / 1.3f);
        }
        public static void DrawSelection(SpriteBatch spriteBatch, Color color, Vector2 Center)
        {
            float overallcolormultiplier = 2 - MathF.Pow(2f, 10 * (openAnimation - 1));

            if (targetAnimation == 0 || oldTargetedStarIndex == -1)
                return;
            color.A = 0;

            Color offsetColor = (targetAnimation > 0.5f ? Color.Lerp(color, Color.White with { A = 0 }, targetAnimation) : Color.Lerp(Color.Black with { A = 0 }, color, targetAnimation)) * overallcolormultiplier;

                // ExponentialInOut easing to make the animation feel a little snappier.
            float interpolator = targetAnimation < 0.5f ? MathF.Pow(2f, 10 * targetAnimation * 2f - 10) : 2f - MathF.Pow(2f, 10 * targetAnimation * -2f + 10);
            interpolator *= satelliteUIZoom;

            BarrierStar bigstar = oldTargetedStarIndex == int.MaxValue ? BarrierStarSystem.TheOneImportantThingInTheSky : BarrierStarSystem.Stars[oldTargetedStarIndex];

            Vector2 position = satelliteUIOffset + bigstar.Position;
            position *= satelliteUIZoom;
            position += Center;

            Vector2 offsetPosition = new(position.X + MathHelper.Lerp(1, 5, interpolator), position.Y);

            spriteBatch.Draw(TextureRegistry.Bracket, offsetPosition, null, offsetColor * interpolator, 0, TextureRegistry.Bracket.Size() / 2, 0.06f * interpolator, SpriteEffects.FlipHorizontally, 0f);

            offsetPosition = new Vector2(position.X - MathHelper.Lerp(1, 5, interpolator), position.Y);

            spriteBatch.Draw(TextureRegistry.Bracket, offsetPosition, null, offsetColor * interpolator, 0, TextureRegistry.Bracket.Size() / 2, 0.06f * interpolator, SpriteEffects.None, 0f);
        }
        public static void DrawStars(SpriteBatch spriteBatch, Vector2 Center, Color BloomColor, DynamicSpriteFont font)
        {
            float overallcolormultiplier = 2 - MathF.Pow(2f, 10 * (openAnimation - 1));

            float interpolator = targetAnimation < 0.5f ? MathF.Pow(2f, 10 * targetAnimation * 2f - 10) : 2f - MathF.Pow(2f, 10 * targetAnimation * -2f + 10);
            foreach (var star in BarrierStarSystem.Stars.Where(s => s.State <= SupernovaState.Shrinking))
            {
                bool currentStar = BarrierStarSystem.Stars[(int)MathHelper.Clamp(targetedStarIndex, 0, BarrierStarSystem.Stars.Length - 1)] == star && !(targetedStarIndex == -1 || targetedStarIndex == int.MaxValue);

                float starSize = MathF.Max(star.BaseSize * star.SupernovaSize, 0.35f) * 1.3f * satelliteUIZoom;

                Vector2 starPosition = satelliteUIOffset + star.Position;
                starPosition *= satelliteUIZoom;
                starPosition += Center;

                if (starPosition.Distance(Center) > 900 || star.Position.Length() > 830)
                    continue;

                    // Makes the grey stars not cover the brighter more important stars.
                Color color = (star.BaseSize < 0.55f  && !currentStar ? Color.Gray with { A = 0 } : Color.White) * overallcolormultiplier;

                spriteBatch.Draw(TextureRegistry.Bloom, starPosition, null, BloomColor, 0, TextureRegistry.Bloom.Size() / 2, starSize * 1.2f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom, starPosition, null, BloomColor * 0.3f, 0, TextureRegistry.Bloom.Size() / 2, starSize * 2.2f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom, starPosition, null, BloomColor * 0.05f, 0, TextureRegistry.Bloom.Size() / 2, starSize * 4.4f, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Circle, starPosition, null, color, 0, TextureRegistry.Circle.Size() / 2, starSize * 0.2f, SpriteEffects.None, 0f);

                    // don't draw a fuckton of text asshole
                if (starPosition.Distance(Center) > 300)
                    continue;

                float offset = oldTargetedStarIndex == -1 || oldTargetedStarIndex == int.MaxValue? 1 : (BarrierStarSystem.Stars[(int)MathHelper.Clamp(oldTargetedStarIndex, 0, BarrierStarSystem.Stars.Length - 1)] == star ? MathHelper.Lerp(1, 1.75f, interpolator) : 1);

                Vector2 textPosition = starPosition + (Vector2.UnitY * 4 * offset * satelliteUIZoom);

                float textSize = Utils.Remap(starPosition.Distance(Center), 0, 300, 0.2f, 0.3f) * satelliteUIZoom;

                Vector2 textOrigin = Helper.MeasureString(star.Name, font);

                if (star.BaseSize > 0.85f || currentStar)
                    ChatManager.DrawColorCodedStringShadow(spriteBatch, font, star.Name, textPosition, Color.Black, 0, new Vector2(textOrigin.X / 2f, 0), Vector2.One * textSize * MathF.Min(starSize * offset / satelliteUIZoom, 2));

                ChatManager.DrawColorCodedString(spriteBatch, font, star.Name, textPosition, color, 0, new Vector2(textOrigin.X / 2f, 0), Vector2.One * textSize * MathF.Min(starSize * offset / satelliteUIZoom, 2));
            }

            BarrierStar bigstar = BarrierStarSystem.TheOneImportantThingInTheSky;
            if (bigstar.State <= SupernovaState.Shrinking)
            {
                float size = bigstar.SupernovaSize * satelliteUIZoom;
                Vector2 position = satelliteUIOffset + bigstar.Position;
                position *= satelliteUIZoom;
                position += Center;

                spriteBatch.Draw(TextureRegistry.Bloom, position, null, BloomColor * 3f, 0, TextureRegistry.Bloom.Size() / 2, 0.8f * size, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Bloom, position, null, BloomColor * 0.4f, 0, TextureRegistry.Bloom.Size() / 2, 1.5f * size, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Bloom, position, null, BloomColor * 0.08f, 0, TextureRegistry.Bloom.Size() / 2, 3.1f * size, SpriteEffects.None, 0f);

                spriteBatch.Draw(TextureRegistry.Circle, position, null, Color.White, 0, TextureRegistry.Circle.Size() / 2, 0.2f * size, SpriteEffects.None, 0f);

                if (position.Distance(Center) < 300)
                {
                    float offset = oldTargetedStarIndex == int.MaxValue ? MathHelper.Lerp(0.8f, 1.2f, interpolator) : 0.8f;

                    Vector2 textPosition = position + (Vector2.UnitY * 6 * offset * satelliteUIZoom);

                    float textSize = Utils.Remap(position.Distance(Center), 0, 300, 0.3f, 0.4f) * satelliteUIZoom;

                    Vector2 textOrigin = Helper.MeasureString(bigstar.Name, font);

                    ChatManager.DrawColorCodedStringShadow(spriteBatch, font, bigstar.Name, textPosition, Color.Black, 0, new Vector2(textOrigin.X / 2f, 0), Vector2.One * textSize * offset);

                    ChatManager.DrawColorCodedString(spriteBatch, font, bigstar.Name, textPosition, Color.White, 0, new Vector2(textOrigin.X / 2f, 0), Vector2.One * textSize * offset);

                    if ((Main.GlobalTimeWrappedHourly * 60) % 50 < 25)
                    {
                        Vector2 warningPosition = position + (-Vector2.UnitY * 25 * satelliteUIZoom);

                        Vector2 warningOrigin = Helper.MeasureString("!", font);

                        ChatManager.DrawColorCodedStringShadow(spriteBatch, font, "!", warningPosition, Color.Black, 0, new Vector2(warningOrigin.X / 2f, 0), Vector2.One * textSize * 1.3f);

                        ChatManager.DrawColorCodedString(spriteBatch, font, "!", warningPosition, Color.Red * overallcolormultiplier, 0, new Vector2(warningOrigin.X / 2f, 0), Vector2.One * textSize * 1.3f);
                    }
                }
            }
            bool cursormode = ModContent.GetInstance<WizenkleBossConfig>().SatelliteUseMousePosition;
            spriteBatch.Draw(cursormode? TextureRegistry.Cursor : TextureRegistry.Circle, cursormode ? (Main.MouseScreen - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2)) / 2 + Center : Center, null, cursormode ? Color.White : Color.Gray with { A = 0 }, 0, cursormode ? Vector2.Zero : TextureRegistry.Circle.Size() / 2, cursormode ? 1f : 0.05f, SpriteEffects.None, 0f);
        }
        public static SatelliteDishTargetContent satelliteDishTargetByRequest;
        public override void Load()
        {
            satelliteDishTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(satelliteDishTargetByRequest);
        }
        public override void Unload()
        {
            Main.ContentThatNeedsRenderTargets.Remove(satelliteDishTargetByRequest);
        }
    }
}
