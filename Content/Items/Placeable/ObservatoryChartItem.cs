using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ID;
using ReLogic.Graphics;
using Terraria.GameContent;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Config;
using Terraria.Utilities;
using WizenkleBoss.Assets.Textures;
using Terraria.UI.Chat;
using WizenkleBoss.Content.Tiles;
using System.Collections.ObjectModel;
using WizenkleBoss.Content.Rarities;

namespace WizenkleBoss.Content.Items.Placeable
{
    public class ObservatoryChartItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ObservatoryChartTile>());
            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.value = 0;
        }
    }
}
