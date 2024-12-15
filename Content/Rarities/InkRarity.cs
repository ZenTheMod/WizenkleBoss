using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;
using WizenkleBoss.Assets.Config;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;

namespace WizenkleBoss.Content.Rarities
{
    public class InkRarity : ModRarity
    {
        public override Color RarityColor => new Color(85, 25, 255, 255);
        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type;
        }
    }
    public class InkRarityGlobalItem : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (item.rare != ModContent.RarityType<InkRarity>() || !ModContent.GetInstance<WizenkleBossConfig>().TooltipEffects || !(line.Mod == "Terraria" && line.Name == "ItemName"))
                return base.PreDrawTooltipLine(item, line, ref yOffset);

            float X = line.X;
            float Y = line.Y;

            float rotation = line.Rotation;

            DynamicSpriteFont font = line.Font;

            Vector2 origin = line.Origin;
            Vector2 baseScale = line.BaseScale;

            Vector2 pos = new(X, Y);

            Vector2 fontSize = Helper.MeasureString(line.Text, font);

            var snapshit = Main.spriteBatch.CaptureSnapshot();

            var barrierShader = Helper.ObjectBarrierShader;

            barrierShader.Value.Parameters["embossColor"]?.SetValue(new Color(85, 25, 255, 255).ToVector4());

            barrierShader.Value.Parameters["Size"]?.SetValue(fontSize);

            barrierShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            barrierShader.Value.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, barrierShader.Value, Main.UIScaleMatrix);

            ChatManager.DrawColorCodedStringShadow(Main.spriteBatch, font, line.Text, pos, new Color(85, 25, 255, 255), rotation, origin, baseScale);
            //ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, pos, new Color(85, 25, 255, 255), rotation, origin, baseScale * 1.3f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(in snapshit);

            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, pos, Color.Black, rotation, origin, baseScale);

            return false;
        }
    }
}
