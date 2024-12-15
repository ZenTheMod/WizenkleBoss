using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.Dusts;
using WizenkleBoss.Content.Items.Dyes;

namespace WizenkleBoss.Assets.Helper
{
    public class InkPlayer : ModPlayer
    {
        public float Intoxication = 0;
        public bool InkyArtifact = false;
        public override void ResetEffects()
        {
            InkyArtifact = false;
        }
        public override void UpdateDead()
        {
            InkyArtifact = false;
        }
        public override void OnEnterWorld()
        {
            if (Player.HasBuff<InkDrugBuff>() || Player.HasBuff<InkDrugStatBuff>())
            {
                Player.ClearBuff(ModContent.BuffType<InkDrugBuff>());
                Player.ClearBuff(ModContent.BuffType<InkDrugStatBuff>());
            }
        }
        public override void PostUpdate()
        {
            if (!Player.HasBuff<InkDrugBuff>() && !Player.HasBuff<InkDrugStatBuff>())
                Intoxication = MathHelper.Clamp(Intoxication - 0.01f, 0f, 1f);
        }
        public void DrawGoo()
        {
            Main.spriteBatch.Draw(TextureRegistry.Bloom, new Rectangle(Main.screenWidth / 2, Main.screenHeight / 2, (int)(Main.screenWidth * Intoxication * 4.3f), (int)(Main.screenHeight * Intoxication * 3.3f)), null, (Color.White * Intoxication), 0, TextureRegistry.Bloom.Size() / 2f, SpriteEffects.None, 0f);
        }
        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (Player.HasBuff<InkDrugStatBuff>() && InkSystem.InsideInkTargetDrawnToThisFrame && Player.Center.Distance(Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2)) < 900)
            {
                foreach (PlayerDrawLayer layer in PlayerDrawLayerLoader.Layers)
                {
                    layer.Hide();
                }
            }
        }
        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player.HasBuff<InkDrugStatBuff>())
            {
                Player.ClearBuff(ModContent.BuffType<InkDrugStatBuff>());
                ArmorShaderData shader = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<InkDye>());
                for (int i = 0; i < 40; i++)
                {
                    int dust = Dust.NewDust(Player.position, Player.width, Player.height, ModContent.DustType<InkDust>(), 0, -5, 0, Color.White, 2);
                    Main.dust[dust].shader = shader;
                }
            }
        }
    }
}
