using log4net.Appender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Achievements;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using System.Collections;
using Terraria.ModLoader.IO;
using Terraria.UI.Gamepad;
using Terraria.Utilities;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Assets.Config;

namespace WizenkleBoss.Content.UI
{
    public partial class BarrierTelescopeUISystem : ModSystem
    {
        public static BarrierTelescopeUI barrierTelescopeUI = new();

        public static bool inUI => Main.InGameUI.CurrentState == barrierTelescopeUI;

        public static Vector2 telescopeTilePosition;

        public static Vector2 telescopeUIOffset;

        public static Vector2 telescopeUIOffsetVelocity;

        public static int blinkCounter = -10;

        private static int blinkFrame = -1;

        private static float windGlobal;

        private static bool prompt = true;
        private static void DrawTelescope()
        {
            telescopeTargetByRequest.Request();
            if (telescopeTargetByRequest.IsReady)
            {
                var frostyLensShader = Helper.FrostyLensShader;
                var gd = Main.instance.GraphicsDevice;

                gd.Textures[1] = TextureRegistry.Lichen;
                gd.SamplerStates[1] = SamplerState.LinearWrap;

                frostyLensShader.Value.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(telescopeTargetByRequest.GetTarget(), new Rectangle((int)((Main.screenWidth * Main.UIScale) / 2) - (int)(540 * Main.UIScale), (int)((Main.screenHeight * Main.UIScale) / 2) - (int)(540 * Main.UIScale), (int)(1080 * Main.UIScale), (int)(1080 * Main.UIScale)), Color.White);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (!inUI && blinkCounter <= -10)
                return;
            int fancyUIIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Fancy UI"));
            if (fancyUIIndex != -1)
            {
                layers.Insert(fancyUIIndex, new LegacyGameInterfaceLayer(
                    "WizenkleBoss: Telescope UI",
                    delegate
                    {
                        var snapshit = Main.spriteBatch.CaptureSnapshot();
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                        Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;
                        float colormult = blinkFrame >= 0 ? 1 : Utils.Remap(blinkCounter, -10, 6, 0, 1);

                        if (blinkFrame >= 0)
                        {
                            DrawTelescope();

                            Texture2D blinkOuter = TextureRegistry.BlinkOuter;
                            Vector2 blinkOuterOrigin = new(blinkOuter.Width * 0.5f, blinkOuter.Height * 0.5f);

                            Main.spriteBatch.Draw(blinkOuter, ScreenSize / 2f, null, Color.Black, 0, blinkOuterOrigin, 1.498f, SpriteEffects.None, 0f);

                            Texture2D blink = TextureRegistry.Blink;
                            Vector2 blinkOrigin = new(blink.Width * 0.5f, blink.Width * 0.5f);

                            Main.spriteBatch.Draw(blink, ScreenSize / 2f, blink.Frame(1, 4, 0, blinkFrame), Color.Black, 0, blinkOrigin, 1.5f, SpriteEffects.None, 0f);

                            Texture2D map = TextureRegistry.TelescopeMap;
                            Vector2 mapOrigin = new(map.Height * 0.5f, map.Height * 0.5f);
                            Vector2 mapPosition = new(blinkOrigin.X * 1.2f, blinkOrigin.Y * 1.2f);

                            float mapmult = Utils.Remap(blinkFrame, 0, 3, 0, 1);
                            Main.spriteBatch.Draw(map, ScreenSize / 2f + mapPosition, map.Frame(2, 1, 0, 0), Color.Gray * mapmult, 0, mapOrigin, 1.5f, SpriteEffects.None, 0f);
                            Main.spriteBatch.Draw(map, ScreenSize / 2f + mapPosition - (telescopeUIOffset * 0.045f), map.Frame(2, 1, 1, 0), Color.White * mapmult, 0, mapOrigin, 1.5f, SpriteEffects.None, 0f);
                        }
                        else
                        {
                            Main.spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle(0, 0, (int)ScreenSize.X + 20, (int)ScreenSize.Y + 20), null, Color.Black * colormult, 0, Vector2.Zero, SpriteEffects.None, 0f);
                        }

                        DynamicSpriteFont font = FontAssets.DeathText.Value;
                        Main.spriteBatch.DrawGenericBackButton(font, barrierTelescopeUI.BackPanel, ScreenSize, Language.GetTextValue("UI.Back"));

                        Vector2 fontSizeMovement = Helper.MeasureString(Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPrompt"), font);
                        Vector2 fontSizeMovementUnder = Helper.MeasureString(Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPromptUnder"), font);

                        if (ModContent.GetInstance<WizenkleBossConfig>().TelescopeMovementKeyPrompt && prompt && blinkFrame >= 0)
                        {
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPrompt"), ScreenSize / 2f, Color.White, 0, new(fontSizeMovement.X / 2f, fontSizeMovement.Y), Vector2.One * 0.8f);

                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPromptUnder"), ScreenSize / 2f + new Vector2(0, fontSizeMovement.Y), Color.Gray, 0, new(fontSizeMovementUnder.X / 2f, fontSizeMovementUnder.Y), Vector2.One * 0.4f);
                        }
                        if (ModContent.GetInstance<ZDebugConfig>().TelescopeDebugText)
                        {
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, telescopeUIOffset.ToString(), new Vector2(0, 5), Color.Gray, 0, Vector2.Zero, Vector2.One * 0.5f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, ((int)telescopeUIOffset.X).ToString(), new Vector2(0, 30), Color.Gray, 0, Vector2.Zero, Vector2.One * 0.5f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, ((int)telescopeUIOffset.Y).ToString(), new Vector2(0, 55), Color.Gray, 0, Vector2.Zero, Vector2.One * 0.5f);

                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, telescopeUIOffsetVelocity.ToString(), new Vector2(0, 105), Color.Gray, 0, Vector2.Zero, Vector2.One * 0.5f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, ((int)telescopeUIOffsetVelocity.X).ToString(), new Vector2(0, 130), Color.Gray, 0, Vector2.Zero, Vector2.One * 0.5f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, ((int)telescopeUIOffsetVelocity.Y).ToString(), new Vector2(0, 155), Color.Gray, 0, Vector2.Zero, Vector2.One * 0.5f);

                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, BarrierStarSystem.TheOneImportantThingInTheSky.ToString(), new Vector2(0, 195), Color.Gray, 0, Vector2.Zero, Vector2.One * 0.5f);

                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, BarrierStarSystem.Stars.Where(s => s.State > 0).ToString(), new Vector2(0, 220), Color.Gray, 0, Vector2.Zero, Vector2.One * 0.5f);
                        }

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(in snapshit);

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (inUI)
            {
                Vector2 normalized = Vector2.Zero;

                if ((PlayerInput.Triggers.JustPressed.Up || PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right || PlayerInput.Triggers.JustPressed.Down) && Main.rand.NextBool(2))
                    SoundEngine.PlaySound(AudioRegistry.TelescopePan);

                if ((PlayerInput.Triggers.Current.Up || PlayerInput.Triggers.Current.Left || PlayerInput.Triggers.Current.Right || PlayerInput.Triggers.Current.Down) && prompt && ModContent.GetInstance<WizenkleBossConfig>().TelescopeMovementKeyPrompt)
                    prompt = false;

                if (PlayerInput.Triggers.Current.Up)
                    normalized += new Vector2(0, 1);
                if (PlayerInput.Triggers.Current.Left)
                    normalized += new Vector2(1, 0);
                if (PlayerInput.Triggers.Current.Right)
                    normalized += new Vector2(-1, 0);
                if (PlayerInput.Triggers.Current.Down)
                    normalized += new Vector2(0, -1);

                normalized = normalized.SafeNormalize(Vector2.Zero);

                telescopeUIOffsetVelocity += normalized * 0.06f * Utils.Remap(ModContent.GetInstance<WizenkleBossConfig>().TelescopeMovementVelocity, 0f, 1f, 1f, 3f);
            }
            telescopeUIOffsetVelocity *= 0.96f;
            telescopeUIOffset += telescopeUIOffsetVelocity;
            telescopeUIOffset = telescopeUIOffset.SafeNormalize(Vector2.Zero) * Utils.Clamp(telescopeUIOffset.Length(), 0, 600);

            UpdateBlinkAnimation();
        }
        public void UpdateBlinkAnimation()
        {
            if (blinkCounter > -10)
                windGlobal += Main.WindForVisuals / 20f;

            if (!inUI)
            {
                if (blinkFrame >= 0)
                {
                    if (--blinkCounter <= 0)
                    {
                        blinkCounter = 5;
                        blinkFrame--;
                    }
                }
                else if (blinkCounter >= -10)
                {
                    blinkCounter--;
                }
                return;
            }

            if (blinkFrame >= 3)
                return;

            if (++blinkCounter >= 5)
            {
                blinkCounter = 0;
                blinkFrame++;
            }
        }
    }
}
