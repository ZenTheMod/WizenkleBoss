using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using WizenkleBoss.Content.Rarities;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Content.Items.Dyes;

namespace WizenkleBoss.Content.Items.Accessories
{
    public class ExtraDimensionalArtifact : BaseInkItem
    {
        public override void SetDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Overlay");
            }
            Item.rare = ModContent.RarityType<InkRarity>();
            Item.width = 30;
            Item.height = 24;
            Item.accessory = true;
        }
    }
}
