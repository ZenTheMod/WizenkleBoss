using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace WizenkleBoss.Common.Commands
{
    public class DebugTimeCommand : ModCommand
    {
        public override CommandType Type
            => CommandType.Chat;
        public override string Command
            => "timeprint";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("Day? " + Main.dayTime.ToString() + " Time: " + Main.time.ToString());

            Main.NewText("MaxDayTime " + Main.dayLength.ToString() + " MaxNightTime " + Main.nightLength.ToString());
        }
    }
}
