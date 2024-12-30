using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Gamepad;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Common.Helper;
using static WizenkleBoss.Content.UI.StarMapUIHelper;

namespace WizenkleBoss.Content.UI
{
    public class SatelliteUI : BaseFancyUI
    {
        public UIElement ModConfigButton;
        public override void InitializeUI()
        {
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

            ModConfigButton = new();
            ModConfigButton.Width.Set(85f / Main.UIScale, 0f);
            ModConfigButton.Height.Set(85f / Main.UIScale, 0f);

            ModConfigButton.VAlign = 1f;
            ModConfigButton.HAlign = 0.5f;
            ModConfigButton.Top.Set(-25f / Main.UIScale, 0f);
            ModConfigButton.Left.Set(-(Main.screenHeight / 2 / Main.UIScale), 0f);

            ModConfigButton.OnMouseOver += FadedMouseOver;
            ModConfigButton.OnLeftClick += OpenConfig;

            uIElement.Append(ModConfigButton);
        }
        private void OpenConfig(UIMouseEvent evt, UIElement listeningElement)
        {
            ModContent.GetInstance<WizenkleBossConfig>().Open(onClose: () => {
                IngameFancyUI.OpenUIState(this);
            }, scrollToOption: nameof(WizenkleBossConfig.TelescopeMovementKeyPrompt), centerScrolledOption: true);
        }
        public override bool DistanceCheck => Main.LocalPlayer.Center.Distance(CurrentTileWorldPosition) >= 200;
        public override void OnActivate()
        {
            SoundEngine.PlaySound(AudioRegistry.HumStart with { Volume = 0.2f });
            InitializeUI();

            if (TargetedStar > -1)
                Prompt = PromptState.Fire;

            BootAnim = 0;
            TerminalState = ContactingState.None;

            if (PlayerInput.UsingGamepadUI)
                UILinkPointNavigator.ChangePoint(3002);
        }
        public override void OnDeactivate()
        {
            ActiveSound DIE = SoundEngine.FindActiveSound(SoundID.MenuClose);
            DIE?.Stop();
            DIE = SoundEngine.FindActiveSound(SoundID.MenuOpen);
            DIE?.Stop();

                // stop my own sounds too
            DIE = SoundEngine.FindActiveSound(AudioRegistry.HumStart);
            DIE?.Stop();

            Main.playerInventory = false;
        }
    }
}
