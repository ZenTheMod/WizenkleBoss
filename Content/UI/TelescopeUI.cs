using log4net.Appender;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Achievements;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using Terraria.Utilities;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Common.Helper;

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
