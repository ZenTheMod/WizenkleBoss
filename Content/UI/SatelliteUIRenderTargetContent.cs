﻿using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helpers;
using static WizenkleBoss.Content.UI.StarMapUIHelper;

namespace WizenkleBoss.Content.UI
{
    [Autoload(Side = ModSide.Client)]
    public partial class SatelliteUISystem : ModSystem
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

                    // i hate my chud life
                float overallcolormultiplier = 2 - MathF.Pow(2f, 10 * (ScaleAnim - 1));

                if (!ModContent.GetInstance<UIConfig>().SatelliteMidnightMode)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                        // Draw the background, i would normally use a different clear color, but that seems to break for some users.
                    spriteBatch.Draw(TextureRegistry.Pixel.Value, new Rectangle(0, 0, (int)TargetSize.X, (int)TargetSize.Y), new Color(11, 8, 18) * Utils.Remap(overallcolormultiplier, 1, 2, 1, 10));

                        // USE the matrix
                    spriteBatch.End();
                }
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, null, UIZoomMatrix);

                Vector2 Center = new(_target.Width / 2f, _target.Height / 2f);

                SpriteFont font = FontRegistry.SpaceMono.Value;

                Color BloomColor = new Color(214, 196, 255) * 0.04f * overallcolormultiplier;
                BloomColor.A = 0;

                bool InTerminal = TerminalState != ContactingState.None;

                    // Draw the stars.
                if (BootAnim > 0.3f && !InTerminal && MapAccess)
                    DrawStars(spriteBatch, BloomColor, font);

                    // Draw the weird bracket selector inspired by you know what at this point.
                if (BootAnim > 0.4f && !InTerminal && MapAccess)
                    DrawSelection(spriteBatch, new(214, 196, 255));

                    // Reset the sb to remove the matrix.
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                if (BootAnim > 0.8f && !InTerminal && MapAccess)
                    DrawTextPrompt(spriteBatch, Center + new Vector2(0, Center.Y / 2), BloomColor, font);

                if (InTerminal && MapAccess)
                    DrawLog(spriteBatch, BloomColor, font);


                bool cursormode = ModContent.GetInstance<UIConfig>().SatelliteUseMousePosition;

                if (BootAnim > 0.95f && CanTargetStar(false))
                    spriteBatch.Draw(cursormode ? TextureRegistry.Cursor.Value : TextureRegistry.Circle.Value, cursormode ? (Main.MouseScreen - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2)) / 2 + Center : Center, null, cursormode ? Color.White : Color.Gray with { A = 0 }, 0, cursormode ? Vector2.Zero : TextureRegistry.Circle.Value.Size() / 2, cursormode ? 1f : 0.1f, SpriteEffects.None, 0f);

                    // Draw the error when you forget to plug in your router
                if (!MapAccess && BootAnim > 0.4f)
                    DrawFullScreenError(spriteBatch, font, Center);

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }
        public static void DrawFullScreenError(SpriteBatch spriteBatch, SpriteFont font, Vector2 Center)
        {
            string error = Language.GetTextValue("Mods.WizenkleBoss.UI.SatelliteDish.NoMap");
            Vector2 textSize = Helper.GetStringSize(font, error, Vector2.One);

            spriteBatch.Draw(TextureRegistry.Bloom.Value, Center, null, (Color.Red * 0.1f) with { A = 0 }, 0f, TextureRegistry.Bloom.Size() / 2f, (TextureRegistry.ConsoleError[1].Value.Size() / TextureRegistry.Ball.Size()) * 9f, SpriteEffects.None, 0f);
            if (Main.GlobalTimeWrappedHourly * 60 % 50 < 25)
                spriteBatch.Draw(TextureRegistry.ConsoleError[1].Value, Center, null, Color.Red, 0, TextureRegistry.ConsoleError[1].Value.Size() / 2, 1, SpriteEffects.None, 0f);

            Helper.DrawColorCodedString(spriteBatch, font, error, Center + new Vector2(0, 20 + TextureRegistry.ConsoleError[1].Value.Height / 2), Color.Red, 0, textSize / 2, Vector2.One * 0.4f);
        }
        public static void DrawLog(SpriteBatch spriteBatch, Color BloomColor, SpriteFont font)
        {
            bool easteregg = Main.LocalPlayer.name == "Jim" || Main.LocalPlayer.name == "Wither";

            int line = TerminalLine <= 9 ? TerminalLine : TerminalLine - 9;
            for (int i = 0; i <= TerminalLines.Length - 1; i++)
            {
                if (i > line)
                    return;
                Vector2 textSize = Helper.GetStringSize(font, TerminalLines[i], Vector2.One);
                Vector2 position = new(30, 30 + (textSize.Y * i * 0.25f));
                bool error = TerminalState > ContactingState.ContactingHighPower && TerminalLine > 9 && i > 1;

                spriteBatch.Draw(TextureRegistry.Bloom.Value, position + (textSize / 2f * 0.22f), null, error ? (Color.Red * 0.1f) with { A = 0 } : (BloomColor * 2f), 0f, TextureRegistry.Bloom.Value.Size() / 2f, (textSize / TextureRegistry.Ball.Size()) * 1.4f, SpriteEffects.None, 0f);

                if (error && easteregg)
                    ChatManager.DrawColorCodedString(spriteBatch, FontAssets.DeathText.Value, TerminalLines[i], position, Color.Red, 0, Vector2.Zero, Vector2.One * 0.22f);
                else
                    Helper.DrawColorCodedString(spriteBatch, font, TerminalLines[i], position, error ? Color.Red : Color.White, 0, Vector2.Zero, Vector2.One * 0.22f);
            }
        }
        public static void DrawTextPrompt(SpriteBatch spriteBatch, Vector2 Center, Color BloomColor, SpriteFont font)
        {
            if (PromptAnim == 0)
                return;

            string PromptText = Prompt switch
            {
                PromptState.MovementKeys => Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPrompt"),
                PromptState.Zoom => Language.GetTextValue("Mods.WizenkleBoss.UI.SatelliteDish.ZoomKeyPrompt"),
                PromptState.Select => Language.GetTextValue("Mods.WizenkleBoss.UI.SatelliteDish.SelectKeyPrompt"),
                PromptState.Fire => Language.GetTextValue("Mods.WizenkleBoss.UI.SatelliteDish.ContactPrompt"),
                PromptState.Error => Language.GetTextValue("Mods.WizenkleBoss.UI.SatelliteDish.ErrorPrompt"),
                _ => " "
            };

            Vector2 fontSizePrompt = Helper.GetStringSize(font, PromptText, Vector2.One);

            Vector2 fontSizeMovementUnder = Helper.GetStringSize(font, Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPromptUnder"), Vector2.One);

            float size = 0.4f * PromptAnim;

            float heightmultiplier = Prompt >= PromptState.Fire ? 1.3f : 1.6f;
            Rectangle backing = new(-(int)(Center.X * 2f), (int)((Center.Y - (fontSizePrompt.Y * size)) - (17 * PromptAnim)), (int)(Center.X * 6f), (int)((fontSizePrompt.Y * heightmultiplier * size) + (34 * PromptAnim)));
            spriteBatch.Draw(TextureRegistry.Pixel.Value, backing with { Y = (int)((Center.Y - (fontSizePrompt.Y * size)) - (19 * PromptAnim)), Height = (int)((fontSizePrompt.Y * heightmultiplier * size) + (38 * PromptAnim)) }, BloomColor * 10f);
            Color backingColor = ModContent.GetInstance<UIConfig>().SatelliteMidnightMode ? Color.Black : new Color(11, 8, 18);
            spriteBatch.Draw(TextureRegistry.Pixel.Value, backing, backingColor);
            spriteBatch.Draw(TextureRegistry.Bloom.Value, backing, BloomColor * 4f);

            if (Prompt == PromptState.None)
                return;

            Helper.DrawColorCodedStringWithShadow(spriteBatch, font, PromptText, Center, Color.White, 0, new(fontSizePrompt.X / 2f, Prompt >= PromptState.Fire ? fontSizePrompt.Y * 0.75f : fontSizePrompt.Y), Vector2.One * size);

            if (Prompt < PromptState.Fire)
                Helper.DrawColorCodedString(spriteBatch, font, Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPromptUnder"), Center, Color.Gray with { A = 0 }, 0, new(fontSizeMovementUnder.X / 2f, 0), Vector2.One * size / 1.3f);
        }
        public static void DrawSelection(SpriteBatch spriteBatch, Color color)
        {
            float overallcolormultiplier = 2 - MathF.Pow(2f, 10 * (ScaleAnim - 1));

            if (SelectAnim == 0 || OldTargetedStar == -1)
                return;
            color.A = 0;

            Color offsetColor = (SelectAnim > 0.5f ? Color.Lerp(color, Color.White with { A = 0 }, SelectAnim) : Color.Lerp(Color.Black with { A = 0 }, color, SelectAnim)) * overallcolormultiplier;

                // ExponentialInOut easing to make the animation feel a little snappier.
            float interpolator = SelectAnim < 0.5f ? MathF.Pow(2f, 10 * SelectAnim * 2f - 10) : 2f - MathF.Pow(2f, 10 * SelectAnim * -2f + 10);

            BarrierStar star = GetStarFromIndex(TargetedStar);
            Vector2 position = star.Position + UIPosition;

            Vector2 origin = TextureRegistry.Circle.Value.Size() / 2;

            spriteBatch.Draw(TextureRegistry.Circle.Value, position, null, color * FireAnim, 0, origin, 0.28f * interpolator, SpriteEffects.None, 0f);
            spriteBatch.Draw(TextureRegistry.Circle.Value, position, null, new Color(11, 8, 18) * MathHelper.Clamp(FireAnim * 4, 0, 1), 0, origin, 0.26f * interpolator, SpriteEffects.None, 0f);

            spriteBatch.Draw(TextureRegistry.Circle.Value, position, null, color * FireAnim * 2, 0, origin, 0.26f * interpolator * FireAnim, SpriteEffects.None, 0f);

            Vector2 offsetPosition = new(position.X + MathHelper.Lerp(1, 5, interpolator + (MathHelper.Clamp(FireAnim * 4, 0, 1) * 2.1f * (interpolator - 1))), position.Y);

            spriteBatch.Draw(TextureRegistry.Bracket.Value, offsetPosition, null, offsetColor * interpolator, 0, TextureRegistry.Bracket.Value.Size() / 2, 0.06f * interpolator, SpriteEffects.FlipHorizontally, 0f);

            offsetPosition = new Vector2(position.X - MathHelper.Lerp(1, 5, interpolator + (MathHelper.Clamp(FireAnim * 4, 0, 1) * 2.1f * (interpolator - 1))), position.Y);

            if (BootAnim > 0.45f)
                spriteBatch.Draw(TextureRegistry.Bracket.Value, offsetPosition, null, offsetColor * interpolator, 0, TextureRegistry.Bracket.Value.Size() / 2, 0.06f * interpolator, SpriteEffects.None, 0f);
        }
        public static void DrawStars(SpriteBatch spriteBatch, Color BloomColor, SpriteFont font)
        {
            float overallcolormultiplier = 2 - MathF.Pow(2f, 10 * (ScaleAnim - 1));

            foreach (var star in BarrierStarSystem.Stars.Where(s => s.State <= SupernovaState.Shrinking))
            {
                bool currentStar = BarrierStarSystem.Stars[(int)MathHelper.Clamp(TargetedStar, 0, BarrierStarSystem.Stars.Length - 1)] == star && !(TargetedStar == -1 || TargetedStar == int.MaxValue);

                    // Draw the dots slightly bigger.
                float starSize = MathF.Max(star.BaseSize * star.SupernovaSize, 0.35f) * 1.3f;

                Vector2 position = star.Position + UIPosition;

                if (position.Length() > 900 || star.Position.Length() > 830)
                    continue;

                    // Makes the grey stars not cover the brighter more important stars.
                Color color = (star.BaseSize < 0.55f  && !currentStar ? Color.Gray with { A = 0 } : Color.White) * overallcolormultiplier;

                DrawStar(spriteBatch, BloomColor, star, color);
                bool CurrentStar = !(OldTargetedStar == -1 || OldTargetedStar == int.MaxValue) && BarrierStarSystem.Stars[(int)MathHelper.Clamp(OldTargetedStar, 0, BarrierStarSystem.Stars.Length - 1)] == star;
                DrawText(spriteBatch, star, font, color, CurrentStar);
            }

            BarrierStar bigstar = BarrierStarSystem.BigStar;
            if (bigstar.State <= SupernovaState.Shrinking)
            {
                Vector2 position = bigstar.Position + UIPosition;

                DrawStar(spriteBatch, BloomColor, bigstar, Color.White);
                DrawText(spriteBatch, bigstar, font, Color.White, OldTargetedStar == int.MaxValue, 1.15f);

                if (Main.GlobalTimeWrappedHourly * 60 % 50 < 25)
                {
                    Vector2 warningPosition = position + (-Vector2.UnitY * 40f);

                    Vector2 warningSize = Helper.GetStringSize(font, "!", Vector2.One);

                    float textScale = Utils.Remap(position.Length(), 0, 300, 0.4f, 0.5f);

                    if (BootAnim > 0.8f)
                        Helper.DrawColorCodedStringShadow(spriteBatch, font, "!", warningPosition, Color.Black, 0, new Vector2(warningSize.X / 8f, 0), Vector2.One * textScale * 1.2f);
                    if (BootAnim > 0.7f)
                        Helper.DrawColorCodedString(spriteBatch, font, "!", warningPosition, Color.Red * overallcolormultiplier, 0, new Vector2(warningSize.X / 8f, 0), Vector2.One * textScale * 1.2f);
                }
            }
        }
        internal static void DrawText(SpriteBatch spriteBatch, BarrierStar star, SpriteFont font, Color color, bool CurrentStar, float Scale = 1f)
        {
            Vector2 position = star.Position + UIPosition;
            if (position.Length() > 300)
                return;

            float interpolator = SelectAnim < 0.5f ? MathF.Pow(2f, 10 * SelectAnim * 2f - 10) : 2f - MathF.Pow(2f, 10 * SelectAnim * -2f + 10);

            float offset = CurrentStar ? MathHelper.Lerp(1, 1.65f, interpolator) : 1;

            float extraoffset = CurrentStar ? 1 + (MathHelper.Clamp(FireAnim * 3, 0, 1) * 0.6f) : 1;

            Vector2 textPosition = position + (Vector2.UnitY * 4 * offset * extraoffset);

            float size = MathF.Max(star.BaseSize * star.SupernovaSize, 0.35f) * 1.3f;
            float textScale = Utils.Remap(position.Length(), 0, 300, 0.3f, 0.2f);

            Vector2 textSize = Helper.GetStringSize(font, star.Name, Vector2.One);

            if (star.BaseSize > 0.85f || CurrentStar)
                Helper.DrawColorCodedStringShadow(spriteBatch, font, star.Name, textPosition, Color.Black, 0, new Vector2(textSize.X / 2f, 0), Vector2.One * textScale * MathF.Min(size * offset, 2) * Scale);

            if (BootAnim > 0.75f)
                Helper.DrawColorCodedString(spriteBatch, font, star.Name, textPosition, color, 0, new Vector2(textSize.X / 2f, 0), Vector2.One * textScale * MathF.Min(size * offset, 2) * Scale);
        }
        internal static void DrawStar(SpriteBatch spriteBatch, Color BloomColor, BarrierStar star, Color color)
        {
            float size = MathF.Max(star.BaseSize * star.SupernovaSize, 0.35f) * 1.3f;
            Vector2 position = star.Position + UIPosition;

            if (BootAnim > 0.7f)
            {
                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, BloomColor, 0, TextureRegistry.Bloom.Value.Size() / 2, size * 1.2f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, BloomColor * 0.3f, 0, TextureRegistry.Bloom.Value.Size() / 2, size * 2.2f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureRegistry.Bloom.Value, position, null, BloomColor * 0.05f, 0, TextureRegistry.Bloom.Value.Size() / 2, size * 4.4f, SpriteEffects.None, 0f);
            }

            if (BootAnim > 0.55f)
                spriteBatch.Draw(TextureRegistry.Circle.Value, position, null, color, 0, TextureRegistry.Circle.Value.Size() / 2, 0.2f * size, SpriteEffects.None, 0f);
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
