using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Common.Textures;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.Dusts;
using WizenkleBoss.Content.Items.Dyes;
using WizenkleBoss.Content.Rarities;

namespace WizenkleBoss.Content.Items
{
    // This is just the free copypaste work :3
    public abstract class BaseInkItem : ModItem
    {
        public Asset<Texture2D> GlowTexture;
        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<InkRarity>();
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            if (GlowTexture == null)
                return;
            Texture2D glowTexture = GlowTexture.Value;

            var snapshit = Main.spriteBatch.CaptureSnapshot();

            var barrierShader = Helper.ObjectBarrierShader;

            barrierShader.Value.Parameters["embossColor"]?.SetValue(new Color(85, 25, 255, 255).ToVector4());

            barrierShader.Value.Parameters["Size"]?.SetValue(glowTexture.Size());

            barrierShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            barrierShader.Value.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, barrierShader.Value, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(glowTexture, Item.position - Main.screenPosition + (glowTexture.Size() / 2), null, Color.White, rotation, glowTexture.Size() / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(in snapshit);
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (GlowTexture == null)
                return;
            Texture2D glowTexture = GlowTexture.Value;

            var snapshit = Main.spriteBatch.CaptureSnapshot();

            var barrierShader = Helper.ObjectBarrierShader;

            barrierShader.Value.Parameters["embossColor"]?.SetValue(new Color(85, 25, 255, 255).ToVector4());

            barrierShader.Value.Parameters["Size"]?.SetValue(glowTexture.Size());

            barrierShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            barrierShader.Value.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, barrierShader.Value, Main.UIScaleMatrix);

            Main.spriteBatch.Draw(glowTexture, position, null, Color.White, 0, glowTexture.Size() / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(in snapshit);
        }
    }
}
