using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;
using static WizenkleBoss.Common.StarRewrite.RealisticSkyCompatSystem;

namespace WizenkleBoss.Common.StarRewrite
{
    public static class RealisticSkyCompatHelper
    {
        /// <summary>
        /// fuck you jit errors (reaper)
        /// </summary>
        /// <param name="opacity"></param>
        /// <param name="backgroundMatrix"></param>
        /// <returns>True if the method runs AND realistic sky is enabled.</returns>
        public static bool DrawRealisticStars(float opacity, Matrix backgroundMatrix)
        {
            if (!RealisticSkyEnabled || !ModContent.GetInstance<VFXConfig>().DrawRealisticStars)
                return false;

            DrawRealisticStarsAtTheCorrectLayer(opacity, backgroundMatrix);
            return true;
        }

        /// <summary>
        /// fuck you jit errors (reaper)
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="screenSize"></param>
        /// <param name="sunMoonPosition"></param>
        /// <returns>True if the method runs AND the realistic sky config option is enabled AND realistic sky is enabled.</returns>
        public static bool DrawRealisticClouds(Vector2 worldPosition, Vector2 screenSize, Vector2 sunMoonPosition)
        {
            if (!RealisticSkyEnabled)
                return false;

            return DrawRealisticCloudsManually(worldPosition, screenSize, sunMoonPosition);
        }
    }
}
