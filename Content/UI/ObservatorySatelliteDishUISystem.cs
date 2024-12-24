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
using Terraria.DataStructures;
using WizenkleBoss.Content.Projectiles.Misc;
using static System.Net.Mime.MediaTypeNames;

namespace WizenkleBoss.Content.UI
{
    public enum ComplexPromptState
    {
        MovementKeys,
        Zoom,
        Select,
        Fire,
        Error,
        None
    }
    public enum ContactingState
    {
        None,
        ContactingLowPower,
        ContactingHighPower,
        ErrorNoPower,
        ErrorStarNotFound
    }
    public partial class ObservatorySatelliteDishUISystem : ModSystem
    {
        public static ObservatorySatelliteDishUI observatorySatelliteDishUI = new();
        public static bool inUI => Main.InGameUI.CurrentState == observatorySatelliteDishUI;

        public static Vector2 satelliteTilePosition;

        public static Vector2 satelliteUIOffset;
        public static float satelliteUIZoom = 1f;

        public static Vector2 satelliteUIOffsetVelocity;

        public static int targetedStarIndex = -1;

        public static int oldTargetedStarIndex = -1;

        public static float targetAnimation = 0;

        public static float openAnimation = 0;

        public static float boot = 0f;

        public static ComplexPromptState prompt = 0;
        public static float confirmationTimer = 0f;

        public static float promptclose = 1f;

        public static ContactingState ConsoleState;
        public static int logline = 0;
        public static float logtimer = 0f;

        public static bool CanTargetStar(bool Scaled = true)
        {
            if (observatorySatelliteDishUI.BackPanel == null || observatorySatelliteDishUI.BatterySwapper == null || observatorySatelliteDishUI.ModConfigButton == null)
                return false;

            float Multiplier = Scaled ? Main.UIScale : 1;
            Vector2 CursorPos = ((Main.MouseScreen * Multiplier) - (new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) * Multiplier)) / 2;

            return CursorPos.X > -TargetSize.X / 2 && CursorPos.X < TargetSize.X / 2 &&
                CursorPos.Y > -TargetSize.Y / 2 && CursorPos.Y < TargetSize.Y / 2 &&
                inUI && ModContent.GetInstance<WizenkleBossConfig>().SatelliteUseMousePosition &&
                !observatorySatelliteDishUI.BackPanel.IsMouseHovering &&
                !observatorySatelliteDishUI.BatterySwapper.IsMouseHovering &&
                !observatorySatelliteDishUI.ModConfigButton.IsMouseHovering;
        }
        public override void OnWorldLoad()
        {
            satelliteUIOffset = Vector2.Zero;
            satelliteUIZoom = 1f;
            targetedStarIndex = -1;
            oldTargetedStarIndex = -1;
        }
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

                Rectangle frame = new((int)((Main.screenWidth * Main.UIScale) / 2), (int)((Main.screenHeight * Main.UIScale) / 2), (int)(Main.screenHeight * interpolator * Main.UIScale), (int)(Main.screenHeight * interpolator * Main.UIScale));

                Main.spriteBatch.Draw(satelliteDishTargetByRequest.GetTarget(), frame, null, Color.White * interpolator, 0, TargetSize / 2, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            }
        }
        private static void DrawBatteryButton()
        {
            UIElement batteryicon = observatorySatelliteDishUI.BatterySwapper;
            if (batteryicon != null)
            {
                Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;

                Vector2 position = new(ScreenSize.X / 2f + (batteryicon.Left.Pixels * Main.UIScale), (ScreenSize.Y * batteryicon.VAlign) + (batteryicon.Top.Pixels * Main.UIScale));

                Color color = batteryicon.IsMouseHovering ? Color.White : Color.Gray;
                color *= openAnimation;

                Vector2 origin = new(TextureRegistry.BatteryIcon.Width / 2f, TextureRegistry.BatteryIcon.Height);
                Main.spriteBatch.Draw(TextureRegistry.BatteryIcon, position, null, color, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
            }
        }
        private static void DrawConfigButton()
        {
            UIElement config = observatorySatelliteDishUI.ModConfigButton;
            if (config != null)
            {
                Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;

                Vector2 position = new(ScreenSize.X / 2f + (config.Left.Pixels * Main.UIScale), (ScreenSize.Y * config.VAlign) + (config.Top.Pixels * Main.UIScale));

                Color color = config.IsMouseHovering ? Color.White : Color.Gray;
                color *= openAnimation;

                Vector2 origin = new(TextureRegistry.ConfigIcon.Width / 2f, TextureRegistry.ConfigIcon.Height);
                Main.spriteBatch.Draw(TextureRegistry.ConfigIcon, position, null, color, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
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

                        Main.spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle(0, 0, (int)ScreenSize.X + 30, (int)ScreenSize.Y + 30), Color.Black * interpolator);

                        DrawMap();

                        Main.spriteBatch.DrawGenericBackButton(FontAssets.DeathText.Value, observatorySatelliteDishUI.BackPanel, ScreenSize, Language.GetTextValue("UI.Back"), 0.8f);

                        DrawBatteryButton();
                        DrawConfigButton();

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
                openAnimation = MathHelper.Clamp(openAnimation + 0.04f, 0f, 1f);
                boot = MathHelper.Clamp(boot + 0.006f, 0f, 1f);
            }
            if (!inUI && openAnimation > 0)
                openAnimation = MathHelper.Clamp(openAnimation - 0.14f, 0f, 1f);
            if (inUI || openAnimation > 0)
            {
                if (ConsoleState != ContactingState.None)
                {
                    if (logtimer >= 1)
                    {
                            // ADD SFX HERE DUMBASS
                        logline++;
                        logtimer = 0;
                    }
                    if (ConsoleState == ContactingState.ContactingLowPower || ConsoleState == ContactingState.ContactingHighPower)
                    {
                        if (logline <= 15)
                            logtimer += Main.rand.NextFloat(0.01f, 0.12f);
                        else
                            logtimer += 0.032f;

                        if (logline > 22)
                        {
                            ConsoleState = ContactingState.None;
                            Main.menuMode = 0;
                            logline = 0;
                            IngameFancyUI.Close();
                            Projectile.NewProjectile(new EntitySource_TileInteraction(Main.LocalPlayer, (int)satelliteTilePosition.X / 16, (int)satelliteTilePosition.Y / 16), satelliteTilePosition, Vector2.Zero, ModContent.ProjectileType<DeepSpaceTransmitter>(), 0, 0, Main.myPlayer);
                        }
                    }
                    if (ConsoleState == ContactingState.ErrorNoPower || ConsoleState == ContactingState.ErrorStarNotFound)
                    {
                        if (logline > 65)
                        {
                            prompt = ComplexPromptState.Error;
                            ConsoleState = ContactingState.None;
                            logline = 0;
                            boot = 0.4f;
                            return;
                        }
                        if (logline <= 11)
                            logtimer += Main.rand.NextFloat(0.01f, 0.12f);
                        else
                            logtimer += 0.4f;
                    }
                    return;
                }

                TriggersPack triggers = PlayerInput.Triggers;
                if (triggers.Current.ViewZoomIn)
                    satelliteUIZoom = MathHelper.Clamp(satelliteUIZoom + 0.02f, 1f, 1.6f);
                if (triggers.Current.ViewZoomOut)
                    satelliteUIZoom = MathHelper.Clamp(satelliteUIZoom - 0.02f, 1f, 1.6f);

                Vector2 normalized = Vector2.Zero;
                UpdatePrompt();
                if (!UpdateSelection())
                {
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

                satelliteUIOffsetVelocity += normalized * 0.7f * Utils.Remap(ModContent.GetInstance<WizenkleBossConfig>().SatelliteMovementVelocity, 0f, 1f, 1.9f, 3.1f);
            }
            satelliteUIOffsetVelocity *= 0.50f;
            satelliteUIOffset += satelliteUIOffsetVelocity;
            satelliteUIOffset = satelliteUIOffset.SafeNormalize(Vector2.Zero) * Utils.Clamp(satelliteUIOffset.Length(), 0, 550);

            if (targetedStarIndex > -1)
            {
                BarrierStar star = targetedStarIndex == int.MaxValue ? BarrierStarSystem.TheOneImportantThingInTheSky : BarrierStarSystem.Stars[(int)MathHelper.Clamp(oldTargetedStarIndex, 0, BarrierStarSystem.Stars.Length - 1)];
                satelliteUIOffset = Vector2.Lerp(satelliteUIOffset, -star.Position, targetAnimation < 0.51f ? 0.003f : 0.7f);
            }
        }
        public static void UpdatePrompt()
        {
            if (prompt == ComplexPromptState.None)
            {
                promptclose = MathHelper.Clamp(promptclose - 0.05f, 0f, 1f);
                return;
            }
            if (prompt != ComplexPromptState.None)
            {
                promptclose = MathHelper.Clamp(promptclose + 0.05f, 0f, 1f);
            }
            if (!ModContent.GetInstance<WizenkleBossConfig>().TelescopeMovementKeyPrompt && prompt < ComplexPromptState.Fire)
            {
                prompt = ComplexPromptState.None;
                return;
            }
            bool hovering = CanTargetStar();
            TriggersPack triggers = PlayerInput.Triggers;
            switch (prompt) 
            {
                case ComplexPromptState.MovementKeys:
                    {
                        promptclose = 1f;
                        if (triggers.Current.Up || triggers.Current.Left || triggers.Current.Right || triggers.Current.Down)
                            prompt = ComplexPromptState.Zoom;
                        break;
                    }
                case ComplexPromptState.Zoom:
                    {
                        if (triggers.Current.ViewZoomIn || triggers.Current.ViewZoomOut)
                            prompt = ComplexPromptState.Select;
                        break;
                    }
                case ComplexPromptState.Select:
                    {
                        if ((triggers.Current.Jump || triggers.Current.MouseLeft) && hovering)
                            prompt = ComplexPromptState.None;
                        break;
                    }
                case ComplexPromptState.Fire:
                    {
                        if (triggers.JustReleased.MouseRight && hovering && confirmationTimer == 1f)
                        {
                            confirmationTimer = 0f;
                            prompt = ComplexPromptState.None;
                            logline = 0;
                            if (targetedStarIndex != int.MaxValue && targetedStarIndex > -1)
                            {
                                if (BarrierStarSystem.Stars[targetedStarIndex].State >= SupernovaState.Expanding)
                                {
                                    ConsoleState = ContactingState.ErrorStarNotFound;
                                    break;
                                }
                            }
                            if (BarrierStarSystem.TheOneImportantThingInTheSky.State >= SupernovaState.Expanding && targetedStarIndex == int.MaxValue)
                            {
                                ConsoleState = ContactingState.ErrorStarNotFound;
                                break;
                            }
                            ConsoleState = ContactingState.ContactingHighPower;
                                //if (lackOFpOWER)
                                //    ConsoleState = ContactingState.ErrorStarNotFound;
                        }
                        if (triggers.Current.MouseRight && hovering && openAnimation >= 0.8f)
                        {
                            confirmationTimer = MathHelper.Clamp(confirmationTimer + 0.02f, 0f, 1f);
                        }
                        else
                        {
                            confirmationTimer = MathHelper.Clamp(confirmationTimer - 0.1f, 0f, 1f);
                        }
                        break;
                    }
                default:
                {
                    break;
                }
            }
        }
        public static bool UpdateSelection()
        {
            if (!inUI)
                return targetedStarIndex > -1;

            bool hovering = CanTargetStar();

            TriggersPack triggers = PlayerInput.Triggers;

            bool CurrentTriggerSelect = (triggers.Current.Jump || triggers.Current.MouseLeft) && hovering;
            bool JustPressedTriggerSelect = (triggers.JustPressed.Jump || triggers.JustPressed.MouseLeft) && hovering;
            bool JustReleasedTriggerSelect = (triggers.JustReleased.Jump || triggers.JustReleased.MouseLeft) && hovering;

            if (prompt < ComplexPromptState.Fire || ConsoleState != ContactingState.None)
                return false;

            if (JustReleasedTriggerSelect && targetedStarIndex > -1)
            {
                SoundEngine.PlaySound(AudioRegistry.Select);
                prompt = ComplexPromptState.Fire;
                confirmationTimer = 0f;
            }

            if ((triggers.JustPressed.Up || triggers.JustPressed.Left || triggers.JustPressed.Right || triggers.JustPressed.Down || JustPressedTriggerSelect) && targetedStarIndex > -1)
            {
                SoundEngine.PlaySound(AudioRegistry.Deselect);
                targetedStarIndex = -1;
                prompt = ComplexPromptState.None;
                return true;
            }

            if (JustPressedTriggerSelect)
            {
                Vector2 centerpos = TargetSize / 2f;
                if (ModContent.GetInstance<WizenkleBossConfig>().SatelliteUseMousePosition)
                    centerpos += Vector2.Clamp(((Main.MouseScreen * Main.UIScale) - (new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) * Main.UIScale)) / 2, -(TargetSize / 2f), TargetSize / 2f);
                Vector2 ClosestPosition = Vector2.Zero;
                int ClosestIndex = -1;
                for (int i = 0; i <= BarrierStarSystem.Stars.Length - 1; i++)
                {
                    BarrierStar star = BarrierStarSystem.Stars[i];
                    Vector2 starPosition = satelliteUIOffset + star.Position;
                    starPosition *= satelliteUIZoom;
                    starPosition += TargetSize / 2f;

                    if (starPosition.Distance(centerpos) < 50 * satelliteUIZoom && star.Position.Length() < 830)
                    {
                        if (starPosition.Distance(centerpos) < ClosestPosition.Distance(centerpos))
                        {
                            ClosestPosition = starPosition;
                            ClosestIndex = i;
                        }
                    }
                }

                BarrierStar bigstar = BarrierStarSystem.TheOneImportantThingInTheSky;

                    // Because my dumbass put the eldritch star seperatly to the rest of the array i have to suffer :D
                    // Basically just sets it to a value that it'd never reach normally.

                Vector2 position = satelliteUIOffset + bigstar.Position;
                position *= satelliteUIZoom;
                position += TargetSize / 2f;

                if (position.Distance(centerpos) < 50 * satelliteUIZoom)
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
