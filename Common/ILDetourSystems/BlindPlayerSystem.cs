using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class BlindPlayerSystem : ModSystem // ApplyColorOfTheSkiesToTiles
    {
        public static float BlindnessInterpolator = 0f;

        public override void Load()
        {
            On_Main.ApplyColorOfTheSkiesToTiles += BlindnessReal;
        }

        public override void Unload()
        {
            On_Main.ApplyColorOfTheSkiesToTiles -= BlindnessReal;
        }

        private void BlindnessReal(On_Main.orig_ApplyColorOfTheSkiesToTiles orig)
        {
            if (!Main.gameMenu)
            {
                if ((TelescopeUISystem.blinkFrame >= 0 && TelescopeUISystem.blindnessCounter > 0) || Main.LocalPlayer.HasBuff(ModContent.BuffType<BlindnessBuff>()))
                    Main.ColorOfTheSkies = Color.Lerp(Main.ColorOfTheSkies, new(1, 1, 1), BlindnessInterpolator);
                else
                    BlindnessInterpolator = 0f;
            }
            orig();
        }
    }
}
