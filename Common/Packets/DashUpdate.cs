using System;
using NetEasy;
using Terraria;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.Packets
{
    [Serializable]
    public class DashUpdate : Module
    {
        private readonly byte fromWho;
        private readonly byte Value;

        public DashUpdate(byte fromWho, byte Value)
        {
            this.fromWho = fromWho;
            this.Value = Value;
        }

        protected override void Receive()
        {
            Main.player[fromWho].GetModPlayer<InkPlayer>().InkDashCooldown = Value;
        }
    }
}
