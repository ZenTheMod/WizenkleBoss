using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Content.Projectiles;
using WizenkleBoss.Content.Rarities;

namespace WizenkleBoss.Content.Items.Notes
{
    public class AstronomyNotes : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;

            Item.useTime = 5;
            Item.useAnimation = 10;
            Item.autoReuse = true;

            Item.rare = ModContent.RarityType<StarboundRarity>();
            Item.noUseGraphic = true;
            Item.UseSound = AudioRegistry.NoteOpen;
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return true;

            return true;
        }
    }
}
