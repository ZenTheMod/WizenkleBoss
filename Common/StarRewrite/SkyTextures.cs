using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria;
using WizenkleBoss.Common.Config;

namespace WizenkleBoss.Common.StarRewrite
{
    public class SkyTextures : ModSystem
    {
        public static readonly Asset<Texture2D> Bloom = LoadTexture2D("SunBloom");
        public static readonly Asset<Texture2D> Sunglasses = LoadTexture2D("Sunglasses");

        public static readonly Asset<Texture2D>[] Moon = LoadTexture2Ds("Moon", 9);
        public static readonly Asset<Texture2D> MoonShadow = LoadTexture2D("MoonShadow");

        public static readonly Asset<Texture2D> SmileyMoon = LoadTexture2D("Moon_Smiley");
        public static readonly Asset<Texture2D> PumpkinMoon = LoadTexture2D("Moon_Pumpkin");
        public static readonly Asset<Texture2D> SnowMoon = LoadTexture2D("Moon_Snow");

            // public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<VFXConfig>().SunAndMoonRework;

        private static Asset<Texture2D> LoadTexture2D(string TexturePath)
        {
            if (Main.dedServ)
                return null;

            return ModContent.Request<Texture2D>("WizenkleBoss/Assets/Textures/Sky/" + TexturePath);
        }

        private static Asset<Texture2D>[] LoadTexture2Ds(string TexturePath, int count)
        {
            if (Main.dedServ)
                return null;

            Asset<Texture2D>[] textures = new Asset<Texture2D>[count];

            for (int i = 0; i < count; i++)
                textures[i] = ModContent.Request<Texture2D>("WizenkleBoss/Assets/Textures/Sky/" + TexturePath + i);

            return textures;
        }
    }
}
