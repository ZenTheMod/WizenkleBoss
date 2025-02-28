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
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.VideoPlayback;

namespace WizenkleBoss
{
	public class WizenkleBoss : Mod
	{
        public override void PostSetupContent() => NetEasy.NetEasy.Register(this);
        public override void HandlePacket(BinaryReader reader, int whoAmI) => NetEasy.NetEasy.HandleModule(reader, whoAmI);

        public WizenkleBoss()
        {
                // Fuck you
            CloudAutoloadingEnabled = false;
            GoreAutoloadingEnabled = false;
        }

        public override IContentSource CreateDefaultContentSource()
        {
            if (!Main.dedServ)
                AddContent(new OgvReader()); // This manual ILoadable adds readers to AssetReaderCollections.

            return base.CreateDefaultContentSource();
        }
    }
}
