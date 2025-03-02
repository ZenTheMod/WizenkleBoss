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
using static WizenkleBoss.Common.StarRewrite.SunAndMoonSystem;
using Terraria.ID;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Common.ILDetourSystems;

namespace WizenkleBoss.Content.UI
{
    public partial class TelescopeUISystem : ModSystem
    {
        public static TelescopeUI telescopeUI = new();

        public static bool inUI => telescopeUI.InUI;

        public static Vector2 telescopeTilePosition;
        public static Vector2 telescopeUIPosition = Vector2.Zero;
        public static Vector2 telescopeUIVelocity = Vector2.Zero;

        public static int blinkCounter = -10;
        public static int blinkFrame = -1;

        private static bool prompt = true;

        public static float fadeValue => blinkFrame >= 0 ? 1 : Utils.Remap(blinkCounter, -10, 5, 0, 1);

        public static Matrix TelescopeMatrix => Matrix.CreateTranslation(TargetSize.X / 2f, TargetSize.Y / 2f, 0f) * Matrix.CreateTranslation(-telescopeUIPosition.X, -telescopeUIPosition.Y, 0f);

        public static int blindnessCounter = 0;

        public static void ResetValues()
        {
                // telescopeUIPosition = Vector2.Zero;
            telescopeUIVelocity = Vector2.Zero;
            blinkCounter = -10;
            blindnessCounter = 0;
        }

        private static void DrawTelescope()
        {
            Effect frostyLensShader = Shaders.FrostyLensShader.Value;
            var gd = Main.instance.GraphicsDevice;

            gd.Textures[1] = Textures.Lichen.Value;
            gd.SamplerStates[1] = SamplerState.LinearWrap;

            frostyLensShader.CurrentTechnique.Passes[0].Apply();

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
                        Main.spriteBatch.Draw(Textures.Pixel.Value, new Rectangle(0, 0, (int)Helper.UIScreenSize.X + 10, (int)Helper.UIScreenSize.Y + 10), Color.Black * fadeValue);

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
                        Main.spriteBatch.DrawGenericBackButton(font, telescopeUI.BackPanel, Helper.UIScreenSize, Language.GetTextValue("UI.Back"), alpha: fadeValue);

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

                float speed = Utils.Remap(ModContent.GetInstance<UIConfig>().TelescopeMovementVelocity / 100, 0f, 1f, 0.05f, 0.2f);

                    // Nest it in a fucking if bcus yes.
                if (Helper.DoPanningMovement(ref telescopeUIPosition, ref telescopeUIVelocity, 0.96f, speed, 1000, true, Sounds.TelescopePan, 2) 
                    && prompt && ModContent.GetInstance<UIConfig>().TelescopeMovementKeyPrompt)
                    prompt = false;

                BlindThoseWhoLookAtTheSun();
            }

            UpdateBlinkAnimation();
        }

        public static void UpdateBlinkAnimation()
        {
            if (inUI)
            {
                    // Gradual build up from -10, then it resets to 0 when it reaches 5.

                    // When the blinkFrame is -1, fadeValue uses blinkCounter as a range from -10 to 5, else its just 1.
                if (blinkFrame >= 3)
                    return;

                if (++blinkCounter >= 5)
                {
                    blinkCounter = 0;
                    blinkFrame++;
                }
            }
            else
            {
                    // Same thing in reverse.
                if (blinkCounter < -10)
                    return;

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
            }
        }

        public static void BlindThoseWhoLookAtTheSun()
        {
                // The moon doesn't hate you dw. (but I do (reaper))
            if (!Main.dayTime)
                return;

            Vector2 position = SunMoonPosition - (SceneAreaSize * 0.5f);
            position.X *= 1.2f;

            float blindness = blindnessCounter / 400f;
            if (BlindPlayerSystem.BlindnessInterpolator < blindness)
                BlindPlayerSystem.BlindnessInterpolator = blindness;

            if (blindnessCounter >= 400)
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<BlindnessBuff>(), 10800);
                Main.menuMode = 0;
                IngameFancyUI.Close();
                return;
            }

            float distance = Vector2.Distance(telescopeUIPosition, position);

            if (distance <= 100)
                blindnessCounter++;
            else
                blindnessCounter--;
        }
    }
}
