using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Gamepad;
using WizenkleBoss.Assets.Helper;

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
            if (MusicKiller.MuffleFactor >= 0.2f)
                MusicKiller.MuffleFactor = 0.2f;
            Player player = Main.LocalPlayer;
            if (player.dead || DistanceCheck || player.CCed || !player.active)
            {
                Main.menuMode = 0;
                IngameFancyUI.Close();
            }
        }
    }
}
