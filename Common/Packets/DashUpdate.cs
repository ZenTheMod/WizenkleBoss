using System;
using Microsoft.Xna.Framework;
using NetEasy;
using Terraria;
using Terraria.ID;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.Packets
{
    [Serializable]
    public class DashUpdate : Module
    {
        private readonly int fromWho;
        private readonly int Value;
        private readonly Vector2 Velocity;

        public DashUpdate(int fromWho, int Value, Vector2 Velocity)
        {
            this.fromWho = fromWho;
            this.Value = Value;
            this.Velocity = Velocity;
        }
        protected override void Receive()
        {
            Main.player[fromWho].GetModPlayer<InkPlayer>().InkDashCooldown = Value;
            Main.player[fromWho].GetModPlayer<InkPlayer>().DashVelocity = Velocity;

            if (Main.netMode == NetmodeID.Server)
                Send(-1, Sender, false);
        }
    }
}
