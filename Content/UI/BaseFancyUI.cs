﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Gamepad;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Content.UI.Notes;

namespace WizenkleBoss.Content.UI
{
    /// <summary>
    /// A very dumbed down basic FancyUI UIState, it literally has one panel :sob:
    /// </summary>
    public abstract class BaseFancyUI : UIState
    {
        /// <summary>
        /// The panel in question:
        /// </summary>
        public UIElement BackPanel;
        public virtual bool DistanceCheck => false;
        public virtual void InitializeUI()
        {
                // i had an overlap bug :sob:
            RemoveAllChildren();

            UIElement uIElement = new();
            uIElement.Width.Set(0f, 1f);
            uIElement.MaxWidth.Set(0, 1f);
            uIElement.MinWidth.Set(0f, 1f);
            uIElement.Top.Set(0f, 0f);
            uIElement.Height.Set(0f, 1f);
            uIElement.HAlign = 0.5f;
            Append(uIElement);

            BackPanel = new();
            BackPanel.Width.Set(140f / Main.UIScale, 0f);
            BackPanel.Height.Set(50f / Main.UIScale, 0f);
            BackPanel.VAlign = 1f;
            BackPanel.HAlign = 0.5f;
            BackPanel.Top.Set(-45f / Main.UIScale, 0f);

            BackPanel.OnMouseOver += FadedMouseOver;
            BackPanel.OnLeftClick += GoBackClick;

            uIElement.Append(BackPanel);
        }
        internal void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
        public override void OnActivate()
        {
            InitializeUI();
            if (PlayerInput.UsingGamepadUI)
                UILinkPointNavigator.ChangePoint(3002);
        }
        public override void OnDeactivate()
        {
                // stops the stupid fucking menu close sfx.
            ActiveSound DIE = SoundEngine.FindActiveSound(SoundID.MenuClose);
            DIE?.Stop();
            DIE = SoundEngine.FindActiveSound(SoundID.MenuOpen);
            DIE?.Stop();

            Main.playerInventory = false;
        }
        internal void GoBackClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.menuMode = 0;
            IngameFancyUI.Close();
        }
        public override void Update(GameTime gameTime)
        {
            if (MusicKiller.MuffleFactor >= 0.1f)
                MusicKiller.MuffleFactor = 0.1f;
            Player player = Main.LocalPlayer;
            if (player.dead || DistanceCheck || player.CCed || !player.active)
            {
                Main.menuMode = 0;
                IngameFancyUI.Close();
            }
        }

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
            // Assigning to Main.hoverItemName to make text appear next to the mouse doesn't work in fancy UI
            // You have to use Main.instance.MouseText(str)
    }
}
