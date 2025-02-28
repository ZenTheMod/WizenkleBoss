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
using Terraria.ModLoader.UI;
using Terraria.Localization;

namespace WizenkleBoss.Common.Config
{
    public abstract class BaseLockedBooleanElement : ConfigElement<bool>
    {
        /// <summary>
        /// The bool that can "lock" this bool.
        /// </summary>
        public abstract bool LockToggle { get; }

        /// <summary>
        /// This bool will be locked when LockToggle matches this state.
        /// </summary>
        public abstract bool LockMode { get; }

        private bool locked => LockToggle == LockMode;

        private static Asset<Texture2D> _toggleTexture;

        public override void OnBind()
        {
            base.OnBind();
            _toggleTexture = Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle");
            OnLeftClick += delegate
            {
                if (!locked)
                    Value = !Value;
            };
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            backgroundColor = locked ? (UICommon.DefaultUIBlue * 0.4f) : UICommon.DefaultUIBlue;

            base.DrawSelf(spriteBatch);

            Texture2D texture = _toggleTexture.Value;

            CalculatedStyle dimensions = GetDimensions();

            Color color = locked ? Color.Gray : Color.White;

            string text = Value ? Lang.menu[126].Value : Lang.menu[124].Value;

            if (locked)
                text += " " + Language.GetTextValue("Mods.WizenkleBoss.Configs.Locked");

            float offset = locked ? 110 : 60f;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, text, new Vector2(dimensions.X + dimensions.Width - offset, dimensions.Y + 8f), color, 0f, Vector2.Zero, new Vector2(0.8f));

            Rectangle sourceRectangle = new(Value ? ((texture.Width - 2) / 2 + 2) : 0, 0, (texture.Width - 2) / 2, texture.Height);
            Vector2 drawPosition = new(dimensions.X + dimensions.Width - sourceRectangle.Width - 10f, dimensions.Y + 8f);

            spriteBatch.Draw(texture, drawPosition, sourceRectangle, color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
        }
    }
}
