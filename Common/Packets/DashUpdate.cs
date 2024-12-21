using System;
using Microsoft.Xna.Framework;
using NetEasy;
using Terraria;
using Terraria.ID;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.Packets
{
    [Serializable]
    public class DashUpdate : Module
    {
        private readonly int fromWho;
        private readonly int Value;

        public DashUpdate(int fromWho, int Value)
        {
            this.fromWho = fromWho;
            this.Value = Value;
        }
        protected override void Receive()
        {
            Main.player[fromWho].GetModPlayer<InkPlayer>().InkDashCooldown = Value;

            if (Main.netMode == NetmodeID.Server)
                Send(-1, Sender, false);
        }
    }
    [Serializable]
    public class DashVelUpdate : Module
    {
        private readonly int fromWho;
        private readonly Vector2 Velocity;

        public DashVelUpdate(int fromWho, Vector2 Velocity)
        {
            this.fromWho = fromWho;
            this.Velocity = Velocity;
        }
        protected override void Receive()
        {
            Main.player[fromWho].GetModPlayer<InkPlayer>().DashVelocity = Velocity;

            if (Main.netMode == NetmodeID.Server)
                Send(-1, Sender, false);
        }
    }
}
