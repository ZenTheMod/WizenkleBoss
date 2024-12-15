using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using ReLogic.Content;
using ReLogic.Content.Sources;
using WizenkleBoss.Assets.Helper;

namespace WizenkleBoss
{
	public class WizenkleBoss : Mod
	{
        public override void PostSetupContent()
        {
            NetEasy.NetEasy.Register(this);
        }
            // ...And to handle any incoming packets.
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            NetEasy.NetEasy.HandleModule(reader, whoAmI);
        }
        public WizenkleBoss()
        {
                // Fuck you
            CloudAutoloadingEnabled = false;
        }
        public override void Load()
        {
            if (!Main.dedServ)
            {
                Helper.LoadShaders();
            }
        }
        public override IContentSource CreateDefaultContentSource()
        {
            if (!Main.dedServ)
            {
                AddContent(new OgvReader()); // This manual ILoadable adds readers to AssetReaderCollections.
            }

            return base.CreateDefaultContentSource();
        }
    }
}
