using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader.Config.UI;
using Terraria.UI.Chat;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using WizenkleBoss.Common.Helpers;
using System.Numerics;
using Terraria.ModLoader.UI;

namespace WizenkleBoss.Common.Config
{
    public abstract class BaseImageBooleanElement : ConfigElement<bool>
    {
        public abstract float TextureHeight { get; }

        public BaseImageBooleanElement()
        {
            Width.Set(0, 1f);
            Height.Set(30 + TextureHeight, 0);
            Recalculate();
        }

        public abstract void CustomDraw(SpriteBatch spriteBatch);

        private static Asset<Texture2D> _toggleTexture;

        public override void OnBind()
        {
            base.OnBind();
            _toggleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle");
            OnLeftClick += delegate
            {
                Value = !Value;
            };
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Texture2D texture = _toggleTexture.Value;

            CalculatedStyle dimensions = GetDimensions();

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Value ? Lang.menu[126].Value : Lang.menu[124].Value, new Vector2(dimensions.X + dimensions.Width - 60f, dimensions.Y + 8f), Color.White, 0f, Vector2.Zero, new Vector2(0.8f));

            Rectangle sourceRectangle = new(Value ? ((texture.Width - 2) / 2 + 2) : 0, 0, (texture.Width - 2) / 2, texture.Height);
            Vector2 drawPosition = new(dimensions.X + dimensions.Width - sourceRectangle.Width - 10f, dimensions.Y + 8f);

            spriteBatch.Draw(texture, drawPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);

            CustomDraw(spriteBatch);
        }
    }
}
