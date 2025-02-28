using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI.Gamepad;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Registries;

namespace WizenkleBoss.Content.UI
{
    public class TelescopeUI : BaseFancyUI
    {
        public override bool DistanceCheck => Main.LocalPlayer.Center.Distance(TelescopeUISystem.telescopeTilePosition) >= 140;
        public override void OnActivate()
        {
            TelescopeUISystem.telescopeUIPosition = Vector2.Zero;
            TelescopeUISystem.telescopeUIVelocity = Vector2.Zero;
            TelescopeUISystem.blinkCounter = -10;

            InitializeUI();

            SoundEngine.PlaySound(Sounds.TelescopeOpen);
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

            SoundEngine.PlaySound(Sounds.TelescopeClose);
            Main.playerInventory = false;
        }
    }
}
