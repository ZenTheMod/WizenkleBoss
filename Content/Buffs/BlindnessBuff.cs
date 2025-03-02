using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.Registries;
using WizenkleBoss.Common.ILDetourSystems;

namespace WizenkleBoss.Content.Buffs
{
    public class BlindnessBuff : ModBuff
    {
            // Make it a proper debuff.
        public override bool RightClick(int buffIndex) => false;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                int time = player.buffTime[buffIndex];
                BlindPlayerSystem.BlindnessInterpolator = Utils.Remap(time, 3600f, 0f, 1f, 0f);
            }
        }
    }
}
