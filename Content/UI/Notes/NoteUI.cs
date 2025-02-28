using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI.Gamepad;
using Terraria;
using WizenkleBoss.Common.Helpers;
using ReLogic.Utilities;
using Terraria.UI;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Registries;

namespace WizenkleBoss.Content.UI.Notes
{
    public class NoteUI : BaseFancyUI
    {
        public UIElement MagnifyButton;
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

            MagnifyButton = new();
            MagnifyButton.Width.Set(85f / Main.UIScale, 0f);
            MagnifyButton.Height.Set(85f / Main.UIScale, 0f);

            MagnifyButton.VAlign = 1f;
            MagnifyButton.HAlign = 0.5f;
            MagnifyButton.Top.Set(-45f / Main.UIScale, 0f);
            MagnifyButton.Left.Set(Main.screenHeight / 3 / Main.UIScale, 0f);

            MagnifyButton.OnMouseOver += FadedMouseOver;
            MagnifyButton.OnLeftClick += MagnifyText;

            uIElement.Append(MagnifyButton);
        }

        private void MagnifyText(UIMouseEvent evt, UIElement listeningElement)
        {
            NoteUISystem.Magnified = !NoteUISystem.Magnified;
        }

        public override void OnActivate()
        {
            InitializeUI();

            SoundEngine.PlaySound(Sounds.NoteOpen);
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
    }
}
