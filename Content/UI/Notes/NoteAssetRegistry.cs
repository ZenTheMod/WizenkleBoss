using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using ReLogic.Content;

namespace WizenkleBoss.Content.UI.Notes
{
    public class NoteAssetRegistry : ModSystem
    {
        public static Asset<Texture2D>[] Base { get; private set; }
        public static Asset<Texture2D>[] Overlay { get; private set; }

        private const int Notes = 2;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Base = new Asset<Texture2D>[Notes];
            Overlay = new Asset<Texture2D>[Notes];
            for (int i = 0; i < Notes; i++)
            {
                Base[i] = LoadTexture2D("Base" + i);
                Overlay[i] = LoadTexture2D("Overlay" + i);
            }
        }

        private static Asset<Texture2D> LoadTexture2D(string TexturePath)
        {
                // if (Main.netMode == NetmodeID.Server)
                //     return default;
            if (ModContent.RequestIfExists("WizenkleBoss/Assets/Textures/Notes/" + TexturePath, out Asset<Texture2D> text))
                return text;
            else
                return ModContent.Request<Texture2D>("WizenkleBoss/Assets/Textures/MagicPixel");
        }
    }
}
