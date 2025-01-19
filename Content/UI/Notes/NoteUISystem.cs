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
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
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

        public static Note CurrentNote = new(0, "");

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

                        Vector2 NoteCenter = Vector2.Lerp(new Vector2(ScreenSize.X / 2f, Main.screenHeight * 2.3f), ScreenSize / 2f, ease);

                        Texture2D noteBase = NoteAssetRegistry.Base[CurrentNote.Texture].Value;
                        Texture2D noteOverlay = NoteAssetRegistry.Overlay[CurrentNote.Texture].Value;
                        Main.spriteBatch.Draw(noteBase, NoteCenter, null, Color.White, Utils.AngleLerp(-MathHelper.PiOver2, 0, ease), noteBase.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        Main.spriteBatch.DrawGenericBackButton(FontAssets.DeathText.Value, noteUI.BackPanel, ScreenSize, Language.GetTextValue("UI.Back"));

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(in snapshit);

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (inUI)
            {
                Main.LocalPlayer.mouseInterface = true;
                interpolator = MathHelper.Clamp(interpolator + 0.04f, 0f, 1f);
            }
            if (!inUI && interpolator > 0)
                interpolator = MathHelper.Clamp(interpolator - 0.14f, 0f, 1f);
        }
    }
}
