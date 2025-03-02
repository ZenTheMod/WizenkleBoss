using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public static bool DrawRealisticStars(GraphicsDevice device, float opacity, Vector2 screenSize, Vector2 sunPosition, Matrix backgroundMatrix, float globalTime, float falloffsize)
        {
            if (!RealisticSkyEnabled || !ModContent.GetInstance<VFXConfig>().DrawRealisticStars)
                return false;

            DrawRealisticStarsAtTheCorrectLayer(device, opacity, screenSize, sunPosition, backgroundMatrix, globalTime, falloffsize);
            return true;
        }

        /// <summary>
        /// fuck you jit errors (reaper)
        /// </summary>
        /// <param name="device"></param>
        /// <param name="opacity"></param>
        public static void ApplyStarAtmosphereShader(GraphicsDevice device, float opacity)
        {
            if (!RealisticSkyEnabled)
                return;

            ApplyAtmosphereShader(device, opacity);
        }

        /// <summary>
        /// fuck you jit errors (reaper)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="screenWidth"></param>
        public static void DrawRealisticGalaxy(Vector2 position, float screenWidth)
        {
            if (!RealisticSkyEnabled || !ModContent.GetInstance<VFXConfig>().DrawRealisticStars)
                return;

            DrawGalaxy(position, screenWidth);
        }

        /// <summary>
        /// fuck you jit errors (reaper)
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <param name="screenSize"></param>
        /// <param name="sunMoonPosition"></param>
        /// <returns>True if the method runs AND the realistic sky config option is enabled AND realistic sky is enabled.</returns>
        public static bool DrawRealisticClouds(Vector2 worldPosition, Rectangle dimentions, Vector2 sunMoonPosition)
        {
            if (!RealisticSkyEnabled)
                return false;

            return DrawRealisticCloudsManually(worldPosition, dimentions, sunMoonPosition);
        }
    }
}
