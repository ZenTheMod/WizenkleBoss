using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace WizenkleBoss.Common.StarRewrite
{
    public enum SupernovaProgress
    {
        None,
        Shrinking,
        Exploding,
        Destroyed
    }

    public class StarSystem : ModSystem
    {
        public static bool canDrawStars = false;

        public const int starCount = 700;

        public static readonly InteractableStar[] stars = new InteractableStar[starCount];
        public static readonly byte[] supernovae = new byte[starCount];

        public static float starRotation;
        public static float starAlpha;

        private const string supernovaeTag = "Supernovae";
        private const string rotationTag = "StarRotation";

        public override void Load() 
        { 
            GenerateStars();
            On_Star.UpdateStars += UpdateStarFields;
        }
        public override void Unload() => On_Star.UpdateStars -= UpdateStarFields;

        private void UpdateStarFields(On_Star.orig_UpdateStars orig)
        {
            starRotation += (float)(Main.dayRate / (Main.gameMenu? 10000 : 70000));
            starAlpha = GetStarAlpha();
            if (!canDrawStars)
                orig(); // skip calling orig to stop vanilla stars from updating.
        }

            // stops the game from drawing stuff too early
        public override void PostSetupContent() => canDrawStars = true;

        public override void OnWorldLoad() => ResetSky();
        public override void OnWorldUnload() => ResetSky();
        public override void ClearWorld() => ResetSky();

        #region saveLoadSync
        public override void SaveWorldData(TagCompound tag) 
        { 
            tag[supernovaeTag] = supernovae;
            tag[rotationTag] = starRotation;
        }
        public override void LoadWorldData(TagCompound tag) 
        {
            if (tag.ContainsKey(supernovaeTag))
                Array.Copy(tag.GetByteArray(supernovaeTag), supernovae, starCount);
            if (tag.ContainsKey(rotationTag))
                starRotation = tag.GetFloat(rotationTag);
        }

        public override void NetSend(BinaryWriter writer) 
        { 
            writer.Write(supernovae);
            writer.Write(starRotation);
        }
        public override void NetReceive(BinaryReader reader) 
        { 
            Array.Copy(reader.ReadBytes(starCount), supernovae, starCount);
            starRotation = reader.ReadSingle();
        }
        #endregion

            // idc but the stars will always be in the same spot
                // i aint saving a seed per world (i dont feel like it)
        private static void GenerateStars()
        {
            UnifiedRandom rand = new("seed".GetHashCode());

            ResetSky();

            stars[0] = new();
            for (int i = 1; i <= starCount - 1; i++)
                stars[i] = new(rand);
        }

        private static void ResetSky()
        {
            starRotation = 0f;
            if (supernovae != null)
                for (int i = 0; i <= starCount - 1; i++)
                    supernovae[i] = 0;
        }

            // and no this is not compatible with the cal biome.
        private static float GetStarAlpha()
        {
            float alpha;

                // Manually calculate the brightness based on time.
            if (Main.dayTime)
            {
                if (Main.time < 6700f)
                    alpha = 1 - ((float)Main.time / 6700f);
                else if (Main.time > 48000f)
                    alpha = ((float)Main.time - 48000f) / (54000f - 48000f);
                else
                    alpha = 0;
            }
            else
                alpha = 1; // Always visible at night.

                // The shimmer biome has an effect based on proximity so i have to account for that.
            if (Main.shimmerAlpha > 0f)
                alpha *= 1f - Main.shimmerAlpha;

                // Same goes for the graveyard minibiome.
            if (Main.GraveyardVisualIntensity > 0f)
                alpha *= 1f - Main.GraveyardVisualIntensity * 1.4f;

                // And brighten it based on proximity to the space layer.
            return MathHelper.Clamp(alpha + MathF.Pow(1f - Main.atmo, 3f), 0f, 1f);
        }
    }
}
