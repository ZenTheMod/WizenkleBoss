using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helpers;

namespace WizenkleBoss.Content.UI.Notes
{
    public readonly struct Note(int texture, string text)
    {
        public readonly int Texture = texture;
        public readonly string Text = text;
    }

    public class NoteUISystem : ModSystem
    {
        public static NoteUI noteUI = new();

        public static bool inUI => Main.InGameUI.CurrentState == noteUI;

        public static Note CurrentNote = new();

        public static float interpolator = 0f;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (!inUI && interpolator <= 0)
                return;
            int fancyUIIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Fancy UI"));
            if (fancyUIIndex != -1)
            {
                layers.Insert(fancyUIIndex, new LegacyGameInterfaceLayer(
                    "WizenkleBoss: Telescope UI",
                    delegate
                    {
                        var snapshit = Main.spriteBatch.CaptureSnapshot();
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                        Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;

                        float ease = 2f * interpolator * (1f - (interpolator / 2f));

                        Main.spriteBatch.Draw(TextureRegistry.Pixel.Value, new Rectangle(0, 0, (int)ScreenSize.X + 20, (int)ScreenSize.Y + 20), null, Color.Black * ease * 0.6f, 0, Vector2.Zero, SpriteEffects.None, 0f);

                        Vector2 noteCenter = Vector2.Lerp(new Vector2(ScreenSize.X / 2f, Main.screenHeight * 1.6f), ScreenSize / 2f, ease);

                        Texture2D noteBase = NoteAssetRegistry.Base[CurrentNote.Texture].Value;
                        Texture2D noteOverlay = NoteAssetRegistry.Overlay[CurrentNote.Texture].Value;

                        float rotation = Utils.AngleLerp(-MathHelper.PiOver2, 0, ease);
                        Color color = Color.White * ease;

                        float scale = 1.5f;

                        Main.spriteBatch.Draw(noteBase, noteCenter, null, color, rotation, noteBase.Size() / 2f, scale, SpriteEffects.None, 0f);

                        DrawText(Main.spriteBatch, new Color(21, 30, 39) * Utils.Remap(interpolator, 0.94f, 1f, 0f, 1f), noteCenter, scale, noteBase.Size());

                        Main.spriteBatch.Draw(noteOverlay, noteCenter, null, color, rotation, noteOverlay.Size() / 2f, scale, SpriteEffects.None, 0f);

                        Main.spriteBatch.DrawGenericBackButton(FontAssets.DeathText.Value, noteUI.BackPanel, ScreenSize, Language.GetTextValue("UI.Back"));

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(in snapshit);

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        public static void DrawText(SpriteBatch spriteBatch, Color color, Vector2 center, float scale, Vector2 size)
        {
            Vector2 margin = new Vector2(10, 24) / scale;
            SpriteFont font = FontRegistry.Microserif.Value;
            font.LineSpacing = 16;
            string log = Language.GetTextValue(CurrentNote.Text);

            TextSnippet[] snippets = [.. ChatManager.ParseMessage(log, color)];

            ChatManager.ConvertNormalSnippets(snippets);
            Helper.DrawColorCodedString(spriteBatch, font, snippets, center, color, 0f, (size / 2f) - margin, Vector2.One * scale, (size.X - 11) * scale);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (inUI)
            {
                Main.LocalPlayer.mouseInterface = true;
                interpolator = MathHelper.Clamp(interpolator + 0.02f, 0f, 1f);
            }
            if (!inUI && interpolator > 0)
                interpolator = MathHelper.Clamp(interpolator - 0.09f, 0f, 1f);
        }
    }
}
