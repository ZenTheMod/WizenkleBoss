using log4net.Appender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Registries;

namespace WizenkleBoss.Content.UI
{
    public partial class TelescopeUISystem : ModSystem
    {
        public static TelescopeUI telescopeUI = new();

        public static bool inUI => telescopeUI.InUI;

        public static Vector2 telescopeTilePosition;
        public static Vector2 telescopeUIOffset;
        public static Vector2 telescopeUIOffsetVelocity;

        public static int blinkCounter = -10;
        private static int blinkFrame = -1;

        private static bool prompt = true;

        private static void DrawTelescope()
        {
            var frostyLensShader = Shaders.FrostyLensShader;
            var gd = Main.instance.GraphicsDevice;

            gd.Textures[1] = Textures.Lichen.Value;
            gd.SamplerStates[1] = SamplerState.LinearWrap;

            frostyLensShader.Value.CurrentTechnique.Passes[0].Apply();

            int size = (int)(Helper.UIScreenSize.Y * 0.85f);
            Rectangle frame = new((int)Helper.HalfUIScreenSize.X, (int)Helper.HalfUIScreenSize.Y, size, size);

                // Request and draw the telescope star view.
            Main.spriteBatch.RequestAndDrawRenderTarget(telescopeTargetByRequest, frame, Color.White, TargetSize / 2);
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

                            // Draw a black base for everything else to sit on.
                        float colormult = blinkFrame >= 0 ? 1 : Utils.Remap(blinkCounter, -10, 6, 0, 1);
                        Main.spriteBatch.Draw(Textures.Pixel.Value, new Rectangle(0, 0, (int)Helper.UIScreenSize.X + 10, (int)Helper.UIScreenSize.Y + 10), Color.Black * colormult);

                            // this sb code is a bit goofy but idc
                        if (blinkFrame >= 0)
                            DrawTelescope();

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                        if (blinkFrame >= 0)
                        {
                            Texture2D blink = Textures.Blink.Value;
                            Vector2 blinkOrigin = new(blink.Width * 0.5f, blink.Width * 0.5f);

                            int blinkSize = (int)(Helper.UIScreenSize.Y * 0.85f);

                            Rectangle frame = new((int)Helper.HalfUIScreenSize.X, (int)Helper.HalfUIScreenSize.Y, blinkSize, blinkSize);

                                // Draw the eyelid opening animation.
                            Main.spriteBatch.Draw(blink, frame, blink.Frame(1, 4, 0, blinkFrame), Color.Black, 0, blinkOrigin, SpriteEffects.None, 0f);
                        }

                        DynamicSpriteFont font = FontAssets.DeathText.Value;
                        Main.spriteBatch.DrawGenericBackButton(font, telescopeUI.BackPanel, Helper.UIScreenSize, Language.GetTextValue("UI.Back"), alpha: colormult);

                            // Draw the movement key prompt.
                        if (ModContent.GetInstance<UIConfig>().TelescopeMovementKeyPrompt && prompt && blinkFrame >= 0)
                        {
                            string promptTitle = Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPrompt");
                            string promptSubitle = Language.GetTextValue("Mods.WizenkleBoss.UI.Telescope.MovementKeyPromptUnder");

                            Vector2 fontSizePromptTitle = ChatManager.GetStringSize(font, promptTitle, Vector2.One);
                            Vector2 fontSizePromptSubitle = ChatManager.GetStringSize(font, promptSubitle, Vector2.One);

                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, promptTitle, Helper.HalfUIScreenSize, Color.White, 0, new(fontSizePromptTitle.X / 2f, fontSizePromptTitle.Y), Vector2.One * 0.8f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, promptSubitle, Helper.HalfUIScreenSize + new Vector2(0, fontSizePromptTitle.Y), Color.Gray, 0, new(fontSizePromptSubitle.X / 2f, fontSizePromptSubitle.Y), Vector2.One * 0.4f);
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
                Main.LocalPlayer.mouseInterface = true;
                Vector2 normalized = Vector2.Zero;

                if ((PlayerInput.Triggers.JustPressed.Up || PlayerInput.Triggers.JustPressed.Left || PlayerInput.Triggers.JustPressed.Right || PlayerInput.Triggers.JustPressed.Down) && Main.rand.NextBool(2))
                    SoundEngine.PlaySound(Sounds.TelescopePan);

                if ((PlayerInput.Triggers.Current.Up || PlayerInput.Triggers.Current.Left || PlayerInput.Triggers.Current.Right || PlayerInput.Triggers.Current.Down) && prompt && ModContent.GetInstance<UIConfig>().TelescopeMovementKeyPrompt)
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

                telescopeUIOffsetVelocity += normalized * 0.06f * Utils.Remap(ModContent.GetInstance<UIConfig>().TelescopeMovementVelocity / 100, 0f, 1f, 1f, 3f);
            }
            telescopeUIOffsetVelocity *= 0.96f;
            telescopeUIOffset += telescopeUIOffsetVelocity;
            telescopeUIOffset = telescopeUIOffset.SafeNormalize(Vector2.Zero) * Utils.Clamp(telescopeUIOffset.Length(), 0, 1000);

            UpdateBlinkAnimation();
        }
        public static void UpdateBlinkAnimation()
        {
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
