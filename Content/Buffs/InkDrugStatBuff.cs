using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;

namespace WizenkleBoss.Content.Buffs
{
    public class InkDrugStatBuff : ModBuff
    {
        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            var snapshit = Main.spriteBatch.CaptureSnapshot();

            var barrierShader = Helper.ObjectBarrierShader;

            barrierShader.Value.Parameters["embossColor"]?.SetValue(new Color(85, 25, 255, 255).ToVector4());

            barrierShader.Value.Parameters["Size"]?.SetValue(new Vector2(32));

            barrierShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            barrierShader.Value.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, barrierShader.Value, Main.UIScaleMatrix);

            Main.spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(in snapshit);

            return false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.HasBuff<InkDrugBuff>())
            {
                player.DelBuff(buffIndex);
                return;
            }

            player.controlHook = false;
            player.controlTorch = false;
            player.controlSmart = false;
            player.controlMount = false;
            player.gravDir = 1f;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.GetModPlayer<InkPlayer>().Intoxication = MathHelper.Clamp(player.GetModPlayer<InkPlayer>().Intoxication + 0.04f, 0f, 1f);
        }
    }
}
