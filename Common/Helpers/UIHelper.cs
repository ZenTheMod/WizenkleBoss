using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.UI.Chat;
using Terraria.UI;
using Terraria;
using WizenkleBoss.Common.Registries;
using WizenkleBoss.Content.UI;
using Terraria.GameInput;
using Terraria.Audio;

namespace WizenkleBoss.Common.Helpers
{
    public static partial class Helper
    {
        /// <summary>
        /// Runs the generic vanilla method as well as demounting, and a few smaller fixes.
        /// <code>
        /// if (player.mount.Active)
        ///     player.mount.Dismount(player);
        /// Main.ClosePlayerChat();
        /// Main.mouseRightRelease = false;
        /// Main.ingameOptionsWindow = false;
        /// Main.chatText = string.Empty;
        /// IngameFancyUI.CoverNextFrame();
		/// Main.playerInventory = false;
		/// Main.editChest = false;
		/// Main.npcChatText = "";
		/// Main.inFancyUI = true;
		/// IngameFancyUI.ClearChat();
        /// Main.InGameUI.SetState(uiState);
        /// </code>
        /// </summary>
        /// <param name="state">you'd think i'd abstract this method sooner lmao</param>
        /// <param name="player">you bitch</param>
        public static void GenericOpenFancyUI(BaseFancyUI state, Player player)
        {
            if (player.mount.Active)
                player.mount.Dismount(player);

            Main.ClosePlayerChat();

            // Fixes some bugs with menus opened via right click.
            Main.mouseRightRelease = false;

            Main.ingameOptionsWindow = false;
            Main.chatText = string.Empty;

            // WOOOO ABSTRACTION TO VANILLA CLASS
            IngameFancyUI.OpenUIState(state);
        }

        /// <summary>
        /// Draws a slightly imperfect back button for fancy ui based on <see cref="Matrix.Identity"/> rather than a uiscale based system, useful for my various fancy ui systems.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="font"></param>
        /// <param name="panel"></param>
        /// <param name="ScreenSize"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        public static void DrawGenericBackButton(this SpriteBatch spriteBatch, DynamicSpriteFont font, UIElement panel, Vector2 ScreenSize, string text, float scale = 0.7f, float alpha = 1f)
        {
            if (panel != null)
            {
                Vector2 position = new(ScreenSize.X / 2f, (ScreenSize.Y * panel.VAlign) + (panel.Top.Pixels * Main.UIScale));

                Vector2 textSize = ChatManager.GetStringSize(font, text, Vector2.One);

                Color StringShadowCol = panel.IsMouseHovering && Main.mouseLeft ? Color.White : Color.Black;
                Color StringCol = panel.IsMouseHovering && Main.mouseLeft ? Color.Black : (panel.IsMouseHovering ? Color.White : Color.Gray);

                if (!Main.inFancyUI)
                {
                    StringShadowCol = Color.Black * 0.5f;
                    StringCol = Color.White * 0.5f;
                }

                Vector2 origin = new(textSize.X / 2f, textSize.Y * 0.75f);
                spriteBatch.Draw(Textures.Ball.Value, position - new Vector2(0, (textSize.Y / 2f) - 20), null, Color.Black * 0.5f * alpha, 0f, Textures.Ball.Size() / 2f, (textSize / Textures.Ball.Size()) * 1.2f, SpriteEffects.None, 0f);
                ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, position, StringShadowCol * alpha, 0, origin, Vector2.One * scale);
                ChatManager.DrawColorCodedString(spriteBatch, font, text, position, StringCol * alpha, 0, origin, Vector2.One * scale);
            }
        }

        public static bool DoPanningMovement(ref Vector2 position, ref Vector2 velocity, float slowdown, float speed, float maxRadius, bool restrictToNegative, SoundStyle? sound = null, int soundChance = 2)
        {
            Vector2 normalized = Vector2.Zero;

            TriggersSet JustPressed = PlayerInput.Triggers.JustPressed;
            bool anyJustPressed = JustPressed.Up || JustPressed.Left || JustPressed.Right || JustPressed.Down;

            TriggersSet Current = PlayerInput.Triggers.JustPressed;
            bool anyCurrent = Current.Up || Current.Left || Current.Right || Current.Down;

            if (anyJustPressed && Main.rand.NextBool(soundChance) && sound != null)
                SoundEngine.PlaySound(sound);

            if (PlayerInput.Triggers.Current.Up)
                normalized += new Vector2(0, -1);
            if (PlayerInput.Triggers.Current.Left)
                normalized += new Vector2(-1, 0);
            if (PlayerInput.Triggers.Current.Right)
                normalized += new Vector2(1, 0);
            if (PlayerInput.Triggers.Current.Down)
                normalized += new Vector2(0, 1);

            normalized = normalized.SafeNormalize(Vector2.Zero);

            velocity += normalized * speed;
            velocity *= slowdown;

            position += velocity;
            position = position.SafeNormalize(Vector2.Zero) * Utils.Clamp(position.Length(), 0, maxRadius);

            if (restrictToNegative && position.Y > 0f)
            { 
                position.Y = 0f;
                velocity.Y = 0f;
            }

            return anyCurrent || anyJustPressed;
        }

        // Random shit I wanna remember:
            // Assigning to Main.hoverItemName to make text appear next to the mouse doesn't work in fancy UI
            // You have to use Main.instance.MouseText(str)
    }
}
