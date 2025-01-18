using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Content.Dusts;
using MonoMod.Cil;
using WizenkleBoss.Content.UI;
using WizenkleBoss.Common.Ink;
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

            Eye = LoadTexture2D("InkCreature/Eye");
            Planet = LoadTexture2D("InkCreature/Planet");
            Orbit = LoadTexture2D("InkCreature/Orbit");
        }

        private static Asset<Texture2D> LoadTexture2D(string TexturePath)
        {
                // if (Main.netMode == NetmodeID.Server)
                //     return default;
            return ModContent.Request<Texture2D>("WizenkleBoss/Assets/Textures/" + TexturePath);
        }
    }
}
