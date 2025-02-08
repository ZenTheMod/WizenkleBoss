using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace WizenkleBoss.Common.Ink
{
    public interface ITakeDamageViaInkDash
    {
        public void OnDashedInto(int damageDone, Vector2 incomingVelocity, Player player);
    }
}
