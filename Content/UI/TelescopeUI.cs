using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI.Gamepad;
using WizenkleBoss.Common.Helpers;

namespace WizenkleBoss.Content.UI
{
    public class TelescopeUI : BaseFancyUI
    {
        public override bool DistanceCheck => Main.LocalPlayer.Center.Distance(TelescopeUISystem.telescopeTilePosition) >= 140;
        public override void OnActivate()
        {
            TelescopeUISystem.telescopeUIOffset = Vector2.Zero;
            TelescopeUISystem.telescopeUIOffsetVelocity = Vector2.Zero;
            TelescopeUISystem.blinkCounter = -10;

            InitializeUI();

            SoundEngine.PlaySound(AudioRegistry.TelescopeOpen);
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

            SoundEngine.PlaySound(AudioRegistry.TelescopeClose);
            Main.playerInventory = false;
        }
    }
}
