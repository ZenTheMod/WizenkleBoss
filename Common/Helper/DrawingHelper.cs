using Terraria;
using Terraria.GameContent;
using Terraria.Utilities;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using System;
using ReLogic.Graphics;
using Terraria.UI.Chat;
using ReLogic.Content;
using Terraria.ModLoader.Default;
using Terraria.UI;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using WizenkleBoss.Common.Helper;

namespace WizenkleBoss.Common.Helper
{
    public static partial class Helper
    {
        /// <summary>Resets the spritebatch to vanilla defaults.</summary>
        /// <param name="end">Weither to end the spritebatch before starting the new one.</param>
        public static void ResetToDefault(this SpriteBatch spriteBatch, bool end = true)
        {
            if (end)
                spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        /// <summary>
        /// Draws a slightly imperfect back button for fancy ui based on <see cref="Matrix.Identity"/> rather than a uiscale based system, useful for my various fancy ui systems.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="font"></param>
        /// <param name="panel"></param>
        /// <param name="ScreenSize"></param>
        /// <param name="text"></param>
        /// <param name="scale"></param>
        public static void DrawGenericBackButton(this SpriteBatch spriteBatch, DynamicSpriteFont font, UIElement panel, Vector2 ScreenSize, string text, float scale = 0.7f)
        {
            if (panel != null)
            {
                Vector2 position = new(ScreenSize.X / 2f, (ScreenSize.Y * panel.VAlign) + (panel.Top.Pixels * Main.UIScale));

                Vector2 textSize = MeasureString(text, font);

                Color StringShadowCol = panel.IsMouseHovering && Main.mouseLeft ? Color.White : Color.Black;
                Color StringCol = panel.IsMouseHovering && Main.mouseLeft ? Color.Black : (panel.IsMouseHovering ? Color.White : Color.Gray);

                if (!Main.inFancyUI)
                {
                    StringShadowCol = Color.Black * 0.5f;
                    StringCol = Color.White * 0.5f;
                }

                Vector2 origin = new(textSize.X / 2f, textSize.Y * 0.75f);
                spriteBatch.Draw(TextureRegistry.Ball, position - new Vector2(0, (textSize.Y / 2f) - 20), null, Color.Black * 0.5f, 0f, TextureRegistry.Ball.Size() / 2f, (textSize / TextureRegistry.Ball.Size()) * 1.2f, SpriteEffects.None, 0f);
                ChatManager.DrawColorCodedStringShadow(spriteBatch, font, text, position, StringShadowCol, 0, origin, Vector2.One * scale);
                ChatManager.DrawColorCodedString(spriteBatch, font, text, position, StringCol, 0, origin, Vector2.One * scale);
            }
        }
    }
}
