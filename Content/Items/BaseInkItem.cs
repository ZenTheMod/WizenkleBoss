using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.Registries;
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

            var inkShader = Shaders.ObjectInkShader;

            inkShader.Value.Parameters["embossColor"]?.SetValue(InkSystem.InkColor.ToVector4());

            inkShader.Value.Parameters["Size"]?.SetValue(glowTexture.Size());

            inkShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            inkShader.Value.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, inkShader.Value, Main.GameViewMatrix.ZoomMatrix);

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

            var inkShader = Shaders.ObjectInkShader;

            inkShader.Value.Parameters["embossColor"]?.SetValue(InkSystem.InkColor.ToVector4());

            inkShader.Value.Parameters["Size"]?.SetValue(glowTexture.Size());

            inkShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            inkShader.Value.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, inkShader.Value, Main.UIScaleMatrix);

            Main.spriteBatch.Draw(glowTexture, position, null, Color.White, 0, glowTexture.Size() / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(in snapshit);
        }
    }
}
