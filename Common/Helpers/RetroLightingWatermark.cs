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
using Terraria.Audio;
using Terraria.ID;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Ink;
using Terraria.Graphics.Light;

namespace WizenkleBoss.Common.Helpers
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
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                        Main.spriteBatch.Draw(TextureRegistry.Pixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.5f);
                        Main.spriteBatch.Draw(TextureRegistry.Ball.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);

                        DynamicSpriteFont font = FontAssets.DeathText.Value;
                        string text = Language.GetTextValue($"Mods.WizenkleBoss.rant");

                        Vector2 center = new(Main.screenWidth / 2f, Main.screenHeight / 2f);
                        Vector2 textSize = ChatManager.GetStringSize(font, text, Vector2.One * 0.5f);
                        Vector2 position = new(center.X - textSize.X / 2f, center.Y - textSize.Y / 2f);
                        Rectangle switchTextRect = new((int)position.X, (int)position.Y, (int)textSize.X, (int)textSize.Y / 2);

                        bool hovering = switchTextRect.Contains(Main.mouseX, Main.mouseY);

                        if (hovering)
                        {
                            if (Main.mouseLeftRelease && Main.mouseLeft)
                            {
                                SoundEngine.PlaySound(SoundID.MenuTick);
                                Lighting.Mode = LightMode.Color;
                            }
                        }
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, position, hovering ? Main.OurFavoriteColor : Color.White, 0f, Vector2.Zero, Vector2.One * 0.5f);

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
