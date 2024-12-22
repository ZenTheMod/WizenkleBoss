using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Gamepad;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;

namespace WizenkleBoss.Content.UI
{
    public class ObservatorySatelliteDishUI : BaseFancyUI
    {
        public override bool DistanceCheck => Main.LocalPlayer.Center.Distance(ObservatorySatelliteDishUISystem.satelliteTilePosition) >= 200;
        public override void OnActivate()
        {
            SoundEngine.PlaySound(AudioRegistry.HumStart with { Volume = 0.2f });
            InitializeUI();

            if (ObservatorySatelliteDishUISystem.targetedStarIndex > -1)
                ObservatorySatelliteDishUISystem.prompt = ComplexPromptState.Fire;

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
