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

namespace WizenkleBoss.Content.UI.Notes
{
    public class NoteUI : BaseFancyUI
    {
        public override void OnActivate()
        {
            InitializeUI();

            SoundEngine.PlaySound(AudioRegistry.NoteOpen);
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
