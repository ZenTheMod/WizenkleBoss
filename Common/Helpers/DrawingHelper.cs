using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.Helpers
{
    public static partial class Helper
    {
        public static Matrix HalfScale => Matrix.CreateScale(0.5f);

        public static void BeginHalfScale(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState)
        {
            spriteBatch.Begin(sortMode, blendState, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, HalfScale);
        }

        public static void RequestAndDrawRenderTarget(this SpriteBatch spriteBatch, ARenderTargetContentByRequest renderTarget)
        {
            spriteBatch.RequestAndDrawRenderTarget(renderTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White, Vector2.Zero);
        }
        public static void RequestAndDrawRenderTarget(this SpriteBatch spriteBatch, ARenderTargetContentByRequest renderTarget, Rectangle destination)
        {
            spriteBatch.RequestAndDrawRenderTarget(renderTarget, destination, Color.White, Vector2.Zero);
        }
        public static void RequestAndDrawRenderTarget(this SpriteBatch spriteBatch, ARenderTargetContentByRequest renderTarget, Rectangle destination, Color color)
        {
            spriteBatch.RequestAndDrawRenderTarget(renderTarget, destination, color, Vector2.Zero);
        }
        public static void RequestAndDrawRenderTarget(this SpriteBatch spriteBatch, ARenderTargetContentByRequest renderTarget, Rectangle destination, Color color, Vector2 origin)
        {
            renderTarget.Request();
            if (renderTarget.IsReady)
                spriteBatch.Draw(renderTarget.GetTarget(), destination, null, color, 0, origin, SpriteEffects.None, 0f);
        }
    }
}
