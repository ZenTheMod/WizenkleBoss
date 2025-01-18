using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.Config;

namespace WizenkleBoss.Common.Commands
{
    public class DebugResetInkRTs : ModCommand
    {
        public override CommandType Type
            => CommandType.Chat;
        public override string Command
            => "ResetRipples";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!ModContent.GetInstance<DebugConfig>().DebugColoredRipples)
            {
                Main.NewText("INSUFICIENT REQUIREMENTS");
                return;
            }
            InkRippleSystem.rippleTarget = null;
            InkRippleSystem._rippleTarget = null;
            Main.NewText("Ink ripple render targets have been reset.");
        }
    }
}
