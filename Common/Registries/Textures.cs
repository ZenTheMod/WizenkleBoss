using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria;

namespace WizenkleBoss.Common.Registries
{
    public class Textures : ModSystem
    {
        public static readonly Asset<Texture2D> Invis = LoadTexture2D("MagicPixel");
        public static readonly Asset<Texture2D> Pixel = LoadTexture2D("NotSoMagicPixel");
        public static readonly Asset<Texture2D>[] Cosmos = [LoadTexture2D("Cosmos"), LoadTexture2D("Cosmos2")];
        public static readonly Asset<Texture2D> Bloat = LoadTexture2D("Noise/Bloat");
        public static readonly Asset<Texture2D> Roar = LoadTexture2D("Roar");
        public static readonly Asset<Texture2D> Shockwave = LoadTexture2D("Shockwave");
        public static readonly Asset<Texture2D>[] Space = [LoadTexture2D("Space"), LoadTexture2D("Space2"), LoadTexture2D("Space3")];
        public static readonly Asset<Texture2D> Star = LoadTexture2D("Stars/Star");
        public static readonly Asset<Texture2D> Bloom = LoadTexture2D("Bloom");
        public static readonly Asset<Texture2D> Ball = LoadTexture2D("Ball");
        public static readonly Asset<Texture2D> Circle = LoadTexture2D("Circle");
        public static readonly Asset<Texture2D> Bracket = LoadTexture2D("Bracket");
        public static readonly Asset<Texture2D> Tech = LoadTexture2D("Noise/Tech");
        public static readonly Asset<Texture2D> WavyNoise = LoadTexture2D("Noise/WavyNoise");
        public static readonly Asset<Texture2D> Lichen = LoadTexture2D("Noise/Lichen");
        public static readonly Asset<Texture2D> Dither = LoadTexture2D("Noise/Dither");
        public static readonly Asset<Texture2D> Wood = LoadTexture2D("Noise/Wood");
        public static readonly Asset<Texture2D> Rainbow = LoadTexture2D("Noise/PRIDEMONTH");
        public static readonly Asset<Texture2D> Blink = LoadTexture2D("Telescope/Blink");
        public static readonly Asset<Texture2D> BlinkOuter = LoadTexture2D("Telescope/BlinkOuter");
        public static readonly Asset<Texture2D> InkDash = LoadTexture2D("InkDash");
        public static readonly Asset<Texture2D> TextBoxStars = LoadTexture2D("Telescope/TextBoxStars");
        public static readonly Asset<Texture2D> TelescopeMap = LoadTexture2D("Telescope/Map");
        public static readonly Asset<Texture2D> Cursor = LoadTexture2D("Cursor");
        public static readonly Asset<Texture2D>[] ConsoleError = [LoadTexture2D("Icons/NotConnectedToPower"), LoadTexture2D("Icons/NotConnectedToSatelliteDish")];
        public static readonly Asset<Texture2D> ConfigIcon = LoadTexture2D("Icons/Settings");
        public static readonly Asset<Texture2D> MagnifyIcon = LoadTexture2D("Icons/Magnify");
        public static readonly Asset<Texture2D> EnergyBar = LoadTexture2D("Icons/PowerBar");
        private static Asset<Texture2D> LoadTexture2D(string TexturePath)
        {
            if (Main.dedServ)
                return null; // uuuuuuuuuhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
            return ModContent.Request<Texture2D>("WizenkleBoss/Assets/Textures/" + TexturePath);
        }
    }
}
