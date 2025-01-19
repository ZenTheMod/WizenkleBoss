using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using ReLogic.Content;

namespace WizenkleBoss.Content.NPCs.InkCreature
{
    public class InkCreatureAssetRegistry : ModSystem
    {
        public static Asset<Texture2D> Eye { get; private set; }
        public static Asset<Texture2D> Planet { get; private set; }
        public static Asset<Texture2D> Orbit { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Eye = LoadTexture2D("Eye");
            Planet = LoadTexture2D("Planet");
            Orbit = LoadTexture2D("Orbit");
        }

        private static Asset<Texture2D> LoadTexture2D(string TexturePath)
        {
                // if (Main.netMode == NetmodeID.Server)
                //     return default;
            return ModContent.Request<Texture2D>("WizenkleBoss/Assets/Textures/InkCreature/" + TexturePath);
        }
    }
}
