﻿using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.Tiles;
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
