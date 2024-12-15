using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Assets.Config;
using WizenkleBoss.Assets.Textures;
using Terraria.UI;
using WizenkleBoss.Assets.Helper;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Terraria.Localization;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace WizenkleBoss.Content.UI
{
    public partial class ObservatorySatelliteDishUISystem : ModSystem
    {
        public static ObservatorySatelliteDishUI observatorySatelliteDishUI = new();
        public static bool inUI => Main.InGameUI.CurrentState == observatorySatelliteDishUI;

        public static Vector2 satelliteTilePosition;

        public static Vector2 satelliteUIOffset;

        public static Vector2 satelliteUIOffsetVelocity;

        public static int targetedStarIndex = -1;

        public static int oldTargetedStarIndex = -1;

        public static float targetAnimation = 0;

        public static float openAnimation = 0;

        private static bool prompt = true;
        private static void DrawMap()
        {
            satelliteDishTargetByRequest.Request();
            if (satelliteDishTargetByRequest.IsReady)
            {
                var oldMonitorShader = Helper.OldMonitorShader;
                var gd = Main.instance.GraphicsDevice;

                oldMonitorShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly * 0.06f);
                oldMonitorShader.Value.Parameters["ScreenSize"]?.SetValue(TargetSize/2);

                oldMonitorShader.Value.Parameters["dithering"]?.SetValue(12);

                gd.Textures[1] = TextureRegistry.Dither;
                gd.SamplerStates[1] = SamplerState.LinearWrap;

                oldMonitorShader.Value.CurrentTechnique.Passes[0].Apply();

                float interpolator = MathF.Pow(2f, 10 * (openAnimation - 1));

                Rectangle frame = new((int)((Main.screenWidth * Main.UIScale) / 2), (int)((Main.screenHeight * Main.UIScale) / 2), (int)(1080 * Main.UIScale * interpolator), (int)(1080 * Main.UIScale * interpolator));

                Main.spriteBatch.Draw(satelliteDishTargetByRequest.GetTarget(), frame, null, Color.White * interpolator, 0, TargetSize / 2, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (!inUI && openAnimation == 0)
                return;
            int fancyUIIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Fancy UI"));
            if (fancyUIIndex != -1)
            {
                layers.Insert(fancyUIIndex, new LegacyGameInterfaceLayer(
                    "WizenkleBoss: Satellite Dish UI",
                    delegate
                    {
                        var snapshit = Main.spriteBatch.CaptureSnapshot();

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                        Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;

                        float interpolator = MathF.Pow(2f, 10 * (openAnimation - 1));

                        Main.spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y), Color.Black * interpolator);

                        DrawMap();

                        Main.spriteBatch.DrawGenericBackButton(FontAssets.DeathText.Value, observatorySatelliteDishUI.BackPanel, ScreenSize, Language.GetTextValue("UI.Back"));

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
                openAnimation = MathHelper.Clamp(openAnimation + 0.04f, 0f, 1f);
            if (!inUI && openAnimation > 0)
                openAnimation = MathHelper.Clamp(openAnimation - 0.14f, 0f, 1f);
            if (inUI || openAnimation > 0)
            {
                Vector2 normalized = Vector2.Zero;
                if (!UpdateSelection())
                {
                    TriggersPack triggers = PlayerInput.Triggers;

                    if ((triggers.Current.Up || triggers.Current.Left || triggers.Current.Right || triggers.Current.Down) && prompt && ModContent.GetInstance<WizenkleBossConfig>().TelescopeMovementKeyPrompt)
                        prompt = false;

                    if (triggers.Current.Up)
                        normalized += new Vector2(0, 1);
                    if (triggers.Current.Left)
                        normalized += new Vector2(1, 0);
                    if (triggers.Current.Right)
                        normalized += new Vector2(-1, 0);
                    if (triggers.Current.Down)
                        normalized += new Vector2(0, -1);

                    normalized = normalized.SafeNormalize(Vector2.Zero);
                }

                satelliteUIOffsetVelocity += normalized * 0.7f * Utils.Remap(ModContent.GetInstance<WizenkleBossConfig>().SatelliteMovementVelocity, 0f, 1f, 1.9f, 2.4f);
            }
            satelliteUIOffsetVelocity *= 0.50f;
            satelliteUIOffset += satelliteUIOffsetVelocity;
            satelliteUIOffset = satelliteUIOffset.SafeNormalize(Vector2.Zero) * Utils.Clamp(satelliteUIOffset.Length(), 0, 550);
            if (targetedStarIndex > -1)
            {
                BarrierStar star = targetedStarIndex == int.MaxValue ? BarrierStarSystem.TheOneImportantThingInTheSky : BarrierStarSystem.Stars[(int)MathHelper.Clamp(oldTargetedStarIndex, 0, BarrierStarSystem.Stars.Length - 1)];
                satelliteUIOffset = Vector2.Lerp(satelliteUIOffset, -star.Position, targetAnimation < 0.51f? 0.003f : 0.7f);
            }
        }
        public static bool UpdateSelection()
        {
            TriggersPack triggers = PlayerInput.Triggers;

            bool CurrentTriggerSelect = triggers.Current.Jump || triggers.Current.MouseLeft;
            bool JustPressedTriggerSelect = triggers.JustPressed.Jump || triggers.JustPressed.MouseLeft;
            bool JustReleasedTriggerSelect = triggers.JustReleased.Jump || triggers.JustReleased.MouseLeft;

            if (JustReleasedTriggerSelect && targetedStarIndex > -1)
            {
                SoundEngine.PlaySound(AudioRegistry.Select);
            }

            if ((triggers.JustPressed.Up || triggers.JustPressed.Left || triggers.JustPressed.Right || triggers.JustPressed.Down || JustPressedTriggerSelect) && targetedStarIndex > -1)
            {
                SoundEngine.PlaySound(AudioRegistry.Deselect);
                targetedStarIndex = -1;
                return true;
            }

            if (JustPressedTriggerSelect)
            {
                Vector2 ClosestPosition = Vector2.Zero;
                int ClosestIndex = -1;
                for (int i = 0; i <= BarrierStarSystem.Stars.Length - 1; i++)
                {
                    BarrierStar star = BarrierStarSystem.Stars[i];
                    Vector2 StarPositon = (TargetSize / 2f) + satelliteUIOffset + star.Position;

                    if (StarPositon.Distance(TargetSize / 2f) < 50)
                    {
                        if (StarPositon.Distance(TargetSize / 2f) < ClosestPosition.Distance(TargetSize / 2f))
                        {
                            ClosestPosition = StarPositon;
                            ClosestIndex = i;
                        }
                    }
                }

                BarrierStar bigstar = BarrierStarSystem.TheOneImportantThingInTheSky;

                    // Because my dumbass put the eldritch star seperatly to the rest of the array i have to suffer :D
                    // Basically just sets it to a value that it'd never reach normally.

                Vector2 Position = (TargetSize / 2f) + satelliteUIOffset + bigstar.Position;
                if (Position.Distance(TargetSize / 2f) < 50)
                    ClosestIndex = int.MaxValue;
                if (ClosestIndex > -1)
                {
                    targetedStarIndex = ClosestIndex;
                    oldTargetedStarIndex = ClosestIndex;
                }
            }

                // dumb anim
            if (targetedStarIndex == -1)
            {
                targetAnimation = MathHelper.Clamp(targetAnimation - 0.1f, 0f, 1f);
            }
            if (CurrentTriggerSelect && targetedStarIndex > -1)
            {
                targetAnimation = MathHelper.Clamp(targetAnimation + 0.03f, 0f, 0.5f);
            }
            if (!CurrentTriggerSelect && targetedStarIndex > -1)
            {
                targetAnimation = MathHelper.Clamp(targetAnimation + 0.15f, 0f, 1f);
            }

            return targetedStarIndex > -1;
        }
    }
}
