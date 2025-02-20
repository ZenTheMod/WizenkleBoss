using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class DisableRetroLightingSystem : ModSystem
    {
        public override void Load()
        {
            On_Lighting.NextLightMode += LetsBeHonestHereYouWereNOTGainingANYPreformanceFromThisFuckYou;
            if (!Lighting.NotRetro)
                Lighting.Mode = LightMode.White;
        }

        public override void Unload()
        {
            On_Lighting.NextLightMode -= LetsBeHonestHereYouWereNOTGainingANYPreformanceFromThisFuckYou;
        }

        private void LetsBeHonestHereYouWereNOTGainingANYPreformanceFromThisFuckYou(On_Lighting.orig_NextLightMode orig)
        {
            if (!Lighting.NotRetro)
                Lighting.Mode = LightMode.White;
            orig();
        }
    }
}
