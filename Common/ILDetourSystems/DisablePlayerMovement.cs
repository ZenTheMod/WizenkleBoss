using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using WizenkleBoss.Content.UI;
using WizenkleBoss.Content.Projectiles.Misc;

namespace WizenkleBoss.Common.ILDetourSystems
{
    /// <summary>
    /// 
    /// 
    /// shut up i know this isnt a detour or il edit
    /// 
    /// 
    /// <para>Some things to note:</para>
    /// 1. How does one disable opening the map?? YOU DONT, <see cref="Player.controlMap">controlMap</see> doesn't do shit because useless bools are hip with the kids. :fire:<br />
    /// 2. Use <see cref="ModPlayer.PostUpdateBuffs">PostUpdateBuffs</see> to disable movement (other hooks prolly work but this one is the safest).<br />
    /// 3. Lag hell.<br />
    /// 4. <see cref="Player.controlMount">controlMount</see> prolly gets reset somewhere in the update hook (god knows where) but just set <see cref="Player.releaseMount">releaseMount</see> instead.<br />
    /// </summary>
    public class DisableMovementPlayer : ModPlayer
    {
            // TODO: put all this shit in ProcessTriggers.
        public override void UpdateEquips()
        {
            if (StopAutoPauseSystem.ShouldNotPause())
            {
                Player.controlMount = false;
                Player.releaseMount = false;
            }
        }

        public override void PostUpdateBuffs()
        {
            if (StopAutoPauseSystem.ShouldNotPause())
            {
                Player.releaseMount = false;

                Player.controlInv = false;
                Player.controlJump = false;
                Player.controlDown = false;
                Player.controlDownHold = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlUp = false;
                Player.controlUseItem = false;
                Player.controlUseTile = false;
                Player.controlThrow = false;

                Player.controlHook = false;

                Player.controlTorch = false;
                Player.controlSmart = false;
                Player.controlMount = false;
                Player.gravDir = 1f;
            }
        }
    }
}
