using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Utilities;
using Terraria;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Ink;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Chat.Commands;
using Terraria.UI.Chat;
using Terraria.ModLoader.UI;
using Terraria.UI;
using WizenkleBoss.Common.Registries;

namespace WizenkleBoss.Common.Config
{
    public class TooltipEffectsImageBooleanElement : BaseImageBooleanElement
    {
        public override float TextureHeight => 40;

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            string text = Language.GetTextValue("Mods.WizenkleBoss.Configs.VFXConfig.TooltipEffects.ExampleText");

            CalculatedStyle dimensions = GetDimensions();

            Vector2 boxPosition = dimensions.Position() + new Vector2(5, 30);

            Color uiBlueDark = new(41, 54, 96);
            Color panelColor = IsMouseHovering ? uiBlueDark * 1.5f : uiBlueDark;

            DrawPanel2(spriteBatch, boxPosition, TextureAssets.SettingsPanel.Value, dimensions.Width - 10, dimensions.Height - 35, panelColor);

            SpriteFont font = Fonts.Starlight.Value;

            Vector2 origin = Vector2.Zero;
            Vector2 baseScale = Vector2.One * 0.9f;

            Vector2 fontSize = Helper.GetStringSize(font, text, Vector2.One);
            Vector2 pos = dimensions.Position() + new Vector2(18, 40);

            Vector2 starBoxPos = pos - new Vector2(7, 5);
            Vector2 starOrigin = Textures.Star.Value.Size() / 2;

            float colmultiplier = MathHelper.Lerp(1.3f, 0.9f, Main.GameUpdateCount % 60 / 60f);

            if (!Value)
            {
                ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, text, pos, Color.Black, 0, origin, baseScale);
                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, text, pos, ChatManager.WaveColor(Color.White), 0, origin, baseScale);
                return;
            }

            spriteBatch.Draw(Textures.TextBoxStars.Value, new Rectangle((int)starBoxPos.X, (int)starBoxPos.Y, (int)(Textures.TextBoxStars.Value.Width * 1.2f), (int)(fontSize.Y * 0.9f) + 10), null, (InkSystem.OutlineColor * colmultiplier) with { A = 0 }, 0, Vector2.Zero, SpriteEffects.None, 0f);

            Helper.DrawColorCodedStringShadow(spriteBatch, font, text, pos + (Vector2.UnitY * 6), InkSystem.OutlineColor with { A = 0 }, 0, origin, baseScale * 1.3f);
            Helper.DrawColorCodedString(spriteBatch, font, text, pos + (Vector2.UnitY * 6), Color.Black, 0, origin, baseScale * 1.3f);

            UnifiedRandom rand = new(Main.LocalPlayer.name.GetHashCode());
            int sparkleCount = rand.Next((int)fontSize.X / 6, (int)fontSize.X / 4) + 1;

            for (int i = 0; i < sparkleCount; i++)
            {
                Color color = InkSystem.OutlineColor;
                Color color2 = color * 0.75f;

                Vector2 v = new(rand.NextFloat(fontSize.X), rand.NextFloat(fontSize.Y * 0.8f));

                float lifeTime = Main.GlobalTimeWrappedHourly * 5.6f + rand.NextFloat(MathHelper.Pi * 14f);
                lifeTime %= MathHelper.Pi * 14f;

                float starRotation = rand.NextFloat(0, MathHelper.TwoPi);
                if (!(lifeTime > MathHelper.TwoPi))
                {
                    float sinValue = (float)Math.Sin((double)lifeTime);
                    Color white = (new Color(200 + color.R / 20, 200 + color.G / 20, 200 + color.B / 20, 255) * sinValue) with { A = 0 };

                    Vector2 starPosition = new Vector2(pos.X, pos.Y - lifeTime * 1f + 2f) + v;
                    spriteBatch.Draw(Textures.Star.Value, starPosition, null, white, starRotation, starOrigin, lifeTime / MathHelper.Pi * 0.66f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Textures.Star.Value, starPosition, null, white * 0.5f, starRotation, starOrigin, lifeTime / MathHelper.Pi, SpriteEffects.None, 0f);

                    float scale3 = lifeTime / MathHelper.TwoPi * 1.5f;
                    Color col2 = (color2 * sinValue) with { A = 0 };

                    starPosition = pos + v;
                    spriteBatch.Draw(Textures.Star.Value, starPosition, null, col2, starRotation, starOrigin, scale3 * 0.9f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Textures.Star.Value, starPosition, null, col2, starRotation, starOrigin, scale3 * 0.6f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(Textures.Star.Value, starPosition, null, col2, starRotation, starOrigin, scale3 * 0.55f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
