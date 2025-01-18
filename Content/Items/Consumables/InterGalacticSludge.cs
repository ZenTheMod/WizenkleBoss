using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.Dusts;
using WizenkleBoss.Content.Items.Dyes;

namespace WizenkleBoss.Content.Items.Consumables
{
    public class InterGalacticSludge : BaseInkItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = [
                new Color(85, 25, 255, 255)
            ];
        }
        public override void SetDefaults()
        {
            if (!Main.dedServ)
            {
                GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Overlay");
            }
            Item.width = 28;
            Item.height = 24;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 23;
            Item.useTime = 23;
            Item.useTurn = true;
            Item.UseSound = AudioRegistry.InkEffectDrinkStart;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;

            Item.value = Item.buyPrice(gold: 3);
            Item.buffType = ModContent.BuffType<InkDrugStatBuff>();
            Item.buffTime = 36000;
            base.SetDefaults();
        }
        public override bool CanUseItem(Player player)
        {
            return !(player.HasBuff<InkDrugStatBuff>() || player.HasBuff<InkDrugBuff>());
        }
        public override bool? UseItem(Player player)
        {
            ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
            for (int i = 0; i < 40; i++)
            {
                int dust = Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<InkDust>(), 0, -5, 0, Color.White, 2);
                Main.dust[dust].shader = shader;
            }
            return true;
        }
    }
}
