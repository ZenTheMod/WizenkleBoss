using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ReLogic.Graphics;

namespace WizenkleBoss.Common.Helper
{
    public class RetroLightingWatermark : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Main.dedServ)
                return;
            if (!Main.gameMenu && !Lighting.NotRetro)
            {
                int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
                if (mouseTextIndex != -1)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "WizenkleBoss: retrolightingsmhsmhmyhead",
                    delegate
                    {
                        var snapshit = Main.spriteBatch.CaptureSnapshot();
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                        DynamicSpriteFont font = FontRegistry.Papyrus;
                        Vector2 center = new(Main.screenWidth / 2f * Main.UIScale, Main.screenHeight / 2f * Main.UIScale);
                        string text = Language.GetTextValue($"Mods.WizenkleBoss.rant");
                        Vector2 size = font.MeasureString(text);

                        Main.spriteBatch.Draw(TextureRegistry.Pixel.Value, new Rectangle(0, 0, (int)(Main.screenWidth * Main.UIScale), (int)(Main.screenHeight * Main.UIScale)), Color.Black * 0.6f);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, center, Color.White, 0f, size * 0.5f, Vector2.One);

                        text = Language.GetTextValue($"Mods.WizenkleBoss.rant2");
                        size = font.MeasureString(text);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(Main.screenWidth / 2f * Main.UIScale, Main.screenHeight * Main.UIScale - size.Y), Color.White, 0f, size * 0.5f, Vector2.One);

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(in snapshit);

                        return true;
                    },
                    InterfaceScaleType.UI));
                }
            }
        }
    }
}
