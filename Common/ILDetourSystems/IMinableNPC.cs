using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public interface IMinableNPC
    {
        public void OnMined(int damageDone, Player player);
    }
}
