using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using WizenkleBoss.Common.MenuStyles;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class DisableCustomSkiesSystem : ModSystem
    {
        public override void Load()
        {
            On_SkyManager.DrawDepthRange += On_SkyManager_DrawDepthRange;
        }

        public override void Unload()
        {
            On_SkyManager.DrawDepthRange -= On_SkyManager_DrawDepthRange;
        }

        private void On_SkyManager_DrawDepthRange(On_SkyManager.orig_DrawDepthRange orig, SkyManager self, SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (!InkPondMenu.InMenu)
                orig(self, spriteBatch, minDepth, maxDepth);
        }
    }
}
