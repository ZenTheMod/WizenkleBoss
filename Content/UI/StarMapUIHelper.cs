using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
using Terraria.Localization;
using Terraria.DataStructures;
using WizenkleBoss.Content.Projectiles.Misc;
using System.Linq;

namespace WizenkleBoss.Content.UI
{
    public enum PromptState
    {
        MovementKeys,
        Zoom,
        Select,
        Fire,
        Error,
        None
    }
        // This is NOT actual error detection you nimrod
    /// <summary>
    /// Tells the 'terminal mode' in the ui what the fuck to display.
    /// </summary>
    public enum ContactingState
    {
        None,
        ContactingLowPower,
        ContactingHighPower,
        ErrorNoPower,
        ErrorStarNotFound
    }
    public class StarMapUIHelper : ModSystem
    {
        public static SatelliteUI satelliteUI = new();
        public static bool inUI => Main.InGameUI.CurrentState == satelliteUI;

        public static Vector2 CurrentTileWorldPosition;

        public static Vector2 UIPosition;
        public static Vector2 UIVelocity;
        public static float UIZoom = 1f;
        public static Matrix UIZoomMatrix =>
            Matrix.CreateScale(UIZoom) *
            Matrix.CreateTranslation(SatelliteUISystem.TargetSize.X / 2, SatelliteUISystem.TargetSize.Y / 2, 0);

        public static int TargetedStar = -1;
        public static int OldTargetedStar = -1;

        public static float SelectAnim = 0;
        public static float ScaleAnim = 0;
        public static float BootAnim = 0f;

        public static PromptState Prompt = 0;
        public static float FireAnim = 0f;
        public static float PromptAnim = 1f;

        public static ContactingState TerminalState;
        public static int TerminalLine = 0;
        public static float TerminalAnim = 0f;

        public static Vector2 ScreenSize => new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;

        internal const int MaxStarDistance = 830;
        public static bool CanTargetStar(bool Scaled = true)
        {
            if (satelliteUI.BackPanel == null || satelliteUI.ModConfigButton == null)
                return false;

            float Multiplier = Scaled ? Main.UIScale : 1;
            Vector2 CursorPos = ((Main.MouseScreen * Multiplier) - (new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) * Multiplier)) / 2;

            return CursorPos.X > -SatelliteUISystem.TargetSize.X / 2 && CursorPos.X < SatelliteUISystem.TargetSize.X / 2 &&
                CursorPos.Y > -SatelliteUISystem.TargetSize.Y / 2 && CursorPos.Y < SatelliteUISystem.TargetSize.Y / 2 &&
                inUI && ModContent.GetInstance<WizenkleBossConfig>().SatelliteUseMousePosition &&
                !satelliteUI.BackPanel.IsMouseHovering &&
                !satelliteUI.ModConfigButton.IsMouseHovering;
        }
        internal static BarrierStar GetStarFromIndex(int index)
        {
            if (index == int.MaxValue)
                return BarrierStarSystem.BigStar;
            else
                return BarrierStarSystem.Stars[(int)MathHelper.Clamp(index, 0, BarrierStarSystem.Stars.Length - 1)];
        }
        internal static void DrawConfigButton()
        {
            UIElement config = satelliteUI.ModConfigButton;
            if (config != null)
            {
                Vector2 position = new(ScreenSize.X / 2f + (config.Left.Pixels * Main.UIScale), (ScreenSize.Y * config.VAlign) + (config.Top.Pixels * Main.UIScale));

                Color color = config.IsMouseHovering ? Color.White : Color.Gray;
                color *= ScaleAnim;

                Vector2 origin = new(TextureRegistry.ConfigIcon.Width / 2f, TextureRegistry.ConfigIcon.Height);
                Main.spriteBatch.Draw(TextureRegistry.ConfigIcon, position, null, color, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
            }
        }
        public static void HandleTerminalText()
        {
            if (TerminalAnim >= 1)
            {
                    // ADD SFX HERE DUMBASS
                TerminalLine++;
                TerminalAnim = 0;
            }
            if (TerminalState == ContactingState.ContactingLowPower || TerminalState == ContactingState.ContactingHighPower)
            {
                if (TerminalLine <= 15)
                    TerminalAnim += Main.rand.NextFloat(0.01f, 0.12f);
                else
                    TerminalAnim += 0.032f;

                if (TerminalLine > 22)
                {
                    TerminalState = ContactingState.None;
                    Main.menuMode = 0;
                    TerminalLine = 0;
                    IngameFancyUI.Close();

                        // Spawn God 
                    Projectile.NewProjectile(new EntitySource_TileInteraction(Main.LocalPlayer, (int)CurrentTileWorldPosition.X / 16, (int)CurrentTileWorldPosition.Y / 16), CurrentTileWorldPosition, Vector2.Zero, ModContent.ProjectileType<DeepSpaceTransmitter>(), 0, 0, Main.myPlayer, TargetedStar);
                }
            }
            if (TerminalState == ContactingState.ErrorNoPower || TerminalState == ContactingState.ErrorStarNotFound)
            {
                    // Gives it some time to sink in that you fucked up bitch
                if (TerminalLine > 65)
                {
                    Prompt = PromptState.Error;
                    TerminalState = ContactingState.None;
                    TerminalLine = 0;

                        // Give it a 'rebooting' effect.
                    BootAnim = 0.4f;
                    return;
                }
                if (TerminalLine <= 11)
                    TerminalAnim += Main.rand.NextFloat(0.01f, 0.12f);
                else
                    TerminalAnim += 0.4f;
            }
        }
        public static void HandleInput()
        {
            HandleZoom();
            TriggersPack triggers = PlayerInput.Triggers;

            Vector2 normalized = Vector2.Zero;

            if (triggers.Current.Up)
                normalized += new Vector2(0, 1);
            if (triggers.Current.Left)
                normalized += new Vector2(1, 0);
            if (triggers.Current.Right)
                normalized += new Vector2(-1, 0);
            if (triggers.Current.Down)
                normalized += new Vector2(0, -1);

            normalized = normalized.SafeNormalize(Vector2.Zero);

            UIVelocity += normalized * 0.7f * Utils.Remap(ModContent.GetInstance<WizenkleBossConfig>().SatelliteMovementVelocity, 0f, 1f, 2.3f, 4.6f); // WOOOO HARD CODED VALUES
            UIVelocity *= 0.5f;
            UIPosition += UIVelocity;
            UIPosition = UIPosition.SafeNormalize(Vector2.Zero) * Utils.Clamp(UIPosition.Length(), 0, 550);

            if (TargetedStar > -1)
            {
                BarrierStar star = GetStarFromIndex(TargetedStar);
                UIPosition = Vector2.Lerp(UIPosition, -star.Position, SelectAnim < 0.51f ? 0.003f : 0.7f);
            }
        }
        internal static void HandleZoom()
        {
            TriggersPack triggers = PlayerInput.Triggers;
            if (triggers.Current.ViewZoomIn)
                UIZoom = MathHelper.Clamp(UIZoom + 0.02f, 1f, 1.6f);
            if (triggers.Current.ViewZoomOut)
                UIZoom = MathHelper.Clamp(UIZoom - 0.02f, 1f, 1.6f);
        }
        public static void UpdateSelection()
        {
            bool hovering = CanTargetStar();

            if (!hovering)
                return;

            TriggersPack triggers = PlayerInput.Triggers;

            bool CurrentTriggerSelect = (triggers.Current.Jump || triggers.Current.MouseLeft) && hovering;
            bool JustPressedTriggerSelect = (triggers.JustPressed.Jump || triggers.JustPressed.MouseLeft) && hovering;
            bool JustReleasedTriggerSelect = (triggers.JustReleased.Jump || triggers.JustReleased.MouseLeft) && hovering;

            if (Prompt < PromptState.Fire)
                return;

            if (JustReleasedTriggerSelect && TargetedStar > -1)
            {
                SoundEngine.PlaySound(AudioRegistry.Select);
                Prompt = PromptState.Fire;
                FireAnim = 0f;
            }

            if ((triggers.JustPressed.Up || triggers.JustPressed.Left || triggers.JustPressed.Right || triggers.JustPressed.Down || JustPressedTriggerSelect) && TargetedStar > -1)
            {
                SoundEngine.PlaySound(AudioRegistry.Deselect);
                TargetedStar = -1;
                Prompt = PromptState.None;
                return;
            }

            if (!JustPressedTriggerSelect)
                return;

            Vector2 centerpos = SatelliteUISystem.TargetSize / 2f;

            if (ModContent.GetInstance<WizenkleBossConfig>().SatelliteUseMousePosition)
                centerpos += (Main.MouseScreen - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2)) * Main.UIScale / 2;

            Vector2 ClosestPosition = Vector2.Zero;

            int ClosestStar = -1;
            for (int i = 0; i <= BarrierStarSystem.Stars.Length - 1; i++)
            {
                BarrierStar star = BarrierStarSystem.Stars[i];
                Vector2 starPosition = Vector2.Transform(star.Position + UIPosition, UIZoomMatrix);

                if (starPosition.Distance(centerpos) < 50 * UIZoom && star.Position.Length() < MaxStarDistance)
                {
                    if (starPosition.Distance(centerpos) < ClosestPosition.Distance(centerpos))
                    {
                        ClosestPosition = starPosition;
                        ClosestStar = i;
                    }
                }
            }

            BarrierStar bigstar = BarrierStarSystem.BigStar;

                // Because my dumbass put the eldritch star seperatly to the rest of the array i have to suffer :D
                // Basically just sets it to a value that it'd never reach normally.
            Vector2 bigStarPosition = Vector2.Transform(bigstar.Position + UIPosition, UIZoomMatrix);

            if (bigStarPosition.Distance(centerpos) < 50 * UIZoom && bigstar.Position.Length() < MaxStarDistance && bigStarPosition.Distance(centerpos) < ClosestPosition.Distance(centerpos))
                ClosestStar = int.MaxValue;

            if (ClosestStar > -1)
            {
                TargetedStar = ClosestStar;
                OldTargetedStar = ClosestStar;
            }
        }
        public static void UpdatePrompt()
        {
            if (!ModContent.GetInstance<WizenkleBossConfig>().TelescopeMovementKeyPrompt && Prompt < PromptState.Fire)
            {
                Prompt = PromptState.None;
                return;
            }
            bool hovering = CanTargetStar();
            TriggersPack triggers = PlayerInput.Triggers;
            switch (Prompt)
            {
                case PromptState.MovementKeys:
                    {
                        PromptAnim = 1f;
                        if (triggers.Current.Up || triggers.Current.Left || triggers.Current.Right || triggers.Current.Down)
                            Prompt = PromptState.Zoom;
                        break;
                    }
                case PromptState.Zoom:
                    {
                        if (triggers.Current.ViewZoomIn || triggers.Current.ViewZoomOut)
                            Prompt = PromptState.Select;
                        break;
                    }
                case PromptState.Select:
                    {
                        if ((triggers.Current.Jump || triggers.Current.MouseLeft) && hovering)
                            Prompt = PromptState.None;
                        break;
                    }
                case PromptState.Fire:
                    {
                        if (triggers.JustReleased.MouseRight && hovering && FireAnim == 1f)
                        {
                            FireAnim = 0f;
                            Prompt = PromptState.None;
                            TerminalLine = 0;
                            if (TargetedStar != int.MaxValue && TargetedStar > -1)
                            {
                                if (BarrierStarSystem.Stars[TargetedStar].State != SupernovaState.None)
                                {
                                    TerminalState = ContactingState.ErrorStarNotFound;
                                    break;
                                }
                            }
                            if (BarrierStarSystem.BigStar.State != SupernovaState.None && TargetedStar == int.MaxValue)
                            {
                                TerminalState = ContactingState.ErrorStarNotFound;
                                break;
                            }
                            TerminalState = ContactingState.ContactingHighPower;
                                // if (lackOFpOWER)
                                //     ConsoleState = ContactingState.ErrorStarNotFound;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
        public static void HandleAnimations()
        {
            bool hovering = CanTargetStar();

            TriggersPack triggers = PlayerInput.Triggers;

            bool CurrentTriggerSelect = (triggers.Current.Jump || triggers.Current.MouseLeft) && hovering;

                // Overall animation for when you open the ui
            if (inUI)
            {
                ScaleAnim = MathHelper.Clamp(ScaleAnim + 0.04f, 0f, 1f);
                BootAnim = MathHelper.Clamp(BootAnim + 0.006f, 0f, 1f);
            }
            if (!inUI && ScaleAnim > 0)
                ScaleAnim = MathHelper.Clamp(ScaleAnim - 0.14f, 0f, 1f);

                // Rest of this hellhole only runs if youre in the ui anyway
            if (!(inUI || ScaleAnim > 0))
                return;

                // The two brackets
            if (TargetedStar == -1)
                SelectAnim = MathHelper.Clamp(SelectAnim - 0.1f, 0f, 1f);

            if (CurrentTriggerSelect && TargetedStar > -1)
                SelectAnim = MathHelper.Clamp(SelectAnim + 0.03f, 0f, 0.5f);

            if (!CurrentTriggerSelect && TargetedStar > -1)
                SelectAnim = MathHelper.Clamp(SelectAnim + 0.15f, 0f, 1f);

                // The circular ring for when youre holding right click
            if (triggers.Current.MouseRight && hovering && ScaleAnim >= 0.8f)
                FireAnim = MathHelper.Clamp(FireAnim + 0.02f, 0f, 1f);
            else
                FireAnim = MathHelper.Clamp(FireAnim - 0.1f, 0f, 1f);

                // The text prompt
            if (Prompt == PromptState.None)
                PromptAnim = MathHelper.Clamp(PromptAnim - 0.05f, 0f, 1f);

            if (Prompt != PromptState.None)
                PromptAnim = MathHelper.Clamp(PromptAnim + 0.05f, 0f, 1f);
        }
    }
}
