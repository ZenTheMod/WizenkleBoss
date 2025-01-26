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

        public static bool Magnified;

        public static float Magnify;

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
                        Color color = Color.Lerp(Color.White, Color.Black, Magnify * 0.5f) * ease;

                        float scale = 1.5f;

                        Main.spriteBatch.Draw(noteBase, noteCenter, null, color, rotation, noteBase.Size() / 2f, scale, SpriteEffects.None, 0f);

                        DrawText(Main.spriteBatch, noteCenter, scale, noteBase.Size());

                        Main.spriteBatch.Draw(noteOverlay, noteCenter, null, color * (1 - Magnify), rotation, noteOverlay.Size() / 2f, scale, SpriteEffects.None, 0f);

                        BaseFancyUI.DrawGenericBackButton(Main.spriteBatch, FontAssets.DeathText.Value, noteUI.BackPanel, ScreenSize, Language.GetTextValue("UI.Back"));

                        DrawMagnifyButton();

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(in snapshit);

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        public static void DrawText(SpriteBatch spriteBatch, Vector2 center, float scale, Vector2 size)
        {
            Vector2 margin = new Vector2(17, 24) / scale;
                // Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;

            float lerp = Utils.Remap(interpolator, 0.94f, 1f, 0f, 1f);
            Color color = new Color(21, 30, 39) * lerp;

            SpriteFont font = FontRegistry.Microserif.Value;
            font.LineSpacing = 16;
            string log = Language.GetTextValue(CurrentNote.Text);

            TextSnippet[] snippets = [.. ChatManager.ParseMessage(log, color)];

            ChatManager.ConvertNormalSnippets(snippets);

            if (Magnify < 1f)
                Helper.DrawColorCodedString(spriteBatch, font, snippets, center, color * (1 - Magnify), 0f, (size / 2f) - margin, Vector2.One * scale, (size.X - 20) * scale, true);

            color *= Magnify;

            if (Magnify > 0f)
            {
                    // Main.spriteBatch.Draw(TextureRegistry.Ball.Value, new Rectangle(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y), color * 0.5f);

                Vector2 origin = ((size / 2f) - margin) * scale;
                ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.MouseText.Value, snippets, center + (Vector2.UnitY * 8), color, 0f, origin / 1.2f, Vector2.One * 1.2f, (size.X - 20) * scale);
                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, center + (Vector2.UnitY * 8), Color.White * lerp, 0f, origin / 1.2f, Vector2.One * 1.2f, out _, (size.X - 20) * scale, true);
            }
        }
        public static void DrawMagnifyButton()
        {
            UIElement magnify = noteUI.MagnifyButton;
            if (magnify != null)
            {
                Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight) * Main.UIScale;
                Vector2 position = new(ScreenSize.X / 2f + (magnify.Left.Pixels * Main.UIScale), (ScreenSize.Y * magnify.VAlign) + (magnify.Top.Pixels * Main.UIScale));

                Color color = magnify.IsMouseHovering ? Color.White : Color.Gray;
                color *= interpolator;

                Texture2D icon = TextureRegistry.MagnifyIcon.Value;

                Vector2 origin = new(icon.Width / 2f, icon.Height);
                Main.spriteBatch.Draw(icon, position, null, color, 0f, origin, Vector2.One, SpriteEffects.None, 0f);
            }
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

            if (Magnified)
                Magnify = MathHelper.Clamp(Magnify + 0.2f, 0f, 1f);
            else
                Magnify = MathHelper.Clamp(Magnify - 0.2f, 0f, 1f);
        }
    }
}
