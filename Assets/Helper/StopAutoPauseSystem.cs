using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Content.Projectiles.Misc;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Assets.Helper
{
    /// <summary>
    /// Vanilla's AutoPause system for whatever reason pauses when FancyUI is open, this also breaks sounds effects although I've been told theres workarounds for that.<br />
    /// Also I would prefer that the pussy shitfaces get to actually watch the stars move and change color over time.
    /// </summary>
    public class StopAutoPauseSystem : ModSystem
    {
        public override void OnModLoad()
        {
            On_Main.CanPauseGame += DisableAutoPause;
        }
        public override void OnModUnload()
        {
            On_Main.CanPauseGame -= DisableAutoPause;
        }
        public static bool ShouldNotPause() => TelescopeUISystem.inUI || StarMapUIHelper.inUI || DeepSpaceTransmitter.charge > 0.01f;
        private bool DisableAutoPause(On_Main.orig_CanPauseGame orig)
        {
            bool pause = orig();
            if (ShouldNotPause())
                return false;
            return pause;
        }
    }
}
