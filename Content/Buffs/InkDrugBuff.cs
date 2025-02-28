using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.Registries;

namespace WizenkleBoss.Content.Buffs
{
    public class InkDrugBuff : ModBuff
    {
        public override bool RightClick(int buffIndex) => false;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int buffIndex, ref BuffDrawParams drawParams)
        {
            var snapshit = Main.spriteBatch.CaptureSnapshot();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            var inkShader = Shaders.ObjectInkShader;

            inkShader.Value.Parameters["embossColor"]?.SetValue(InkSystem.InkColor.ToVector4());

            inkShader.Value.Parameters["Size"]?.SetValue(new Vector2(32));

            inkShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            inkShader.Value.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(drawParams.Texture, drawParams.Position, drawParams.SourceRectangle, drawParams.DrawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(in snapshit);

            return false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<InkPlayer>().Intoxication = MathHelper.Clamp(player.GetModPlayer<InkPlayer>().Intoxication + 0.03f, 0f, 1f);
        }
    }
}
