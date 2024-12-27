using System;
using NetEasy;
using Terraria;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.Packets
{
    [Serializable]
    public class RequestStars : Module
    {
        private readonly int fromWho;

        public RequestStars(int fromWho)
        {
            this.fromWho = fromWho;
        }
        protected override void Receive()
        {
            if (Main.dedServ)
            {
                    // HORRIBLE IDEA
                    // its probably FINE cus i only run it for joining players
                var sendstars = new SendStars(BarrierStarSystem.Stars, BarrierStarSystem.BigStar);
                sendstars.Send(toClient: fromWho, runLocally: false);
            }
        }
    }
    [Serializable]
    public class SendStars : Module
    {
        private readonly BarrierStar[] Stars;
        private readonly BarrierStar ImportantStar;

        public SendStars(BarrierStar[] Stars, BarrierStar ImportantStar)
        {
            this.Stars = Stars;
            this.ImportantStar = ImportantStar;
        }

        protected override void Receive()
        {
            BarrierStarSystem.Stars = Stars;
            BarrierStarSystem.BigStar = ImportantStar;
        }
    }
}
