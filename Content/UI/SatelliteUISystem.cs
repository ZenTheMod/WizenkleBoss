using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using WizenkleBoss.Common.Helper;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Localization;
using static WizenkleBoss.Content.UI.StarMapUIHelper;

namespace WizenkleBoss.Content.UI
{
    public partial class SatelliteUISystem : ModSystem
    {
        public override void OnWorldLoad()
        {
            UIPosition = Vector2.Zero;
            UIZoom = 1f;
            TargetedStar = -1;
            OldTargetedStar = -1;
        }
        private static void DrawMap()
        {
            satelliteDishTargetByRequest.Request();
            if (satelliteDishTargetByRequest.IsReady)
            {
                var oldMonitorShader = Helper.OldMonitorShader;
                var gd = Main.instance.GraphicsDevice;

                oldMonitorShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly * 0.06f);
                oldMonitorShader.Value.Parameters["ScreenSize"]?.SetValue(TargetSize/2);

                oldMonitorShader.Value.Parameters["dithering"]?.SetValue(12);

                gd.Textures[1] = TextureRegistry.Dither;
                gd.SamplerStates[1] = SamplerState.LinearWrap;

                oldMonitorShader.Value.CurrentTechnique.Passes[0].Apply();

                float interpolator = MathF.Pow(2f, 10 * (ScaleAnim - 1));

                Rectangle frame = new((int)(ScreenSize.X / 2), (int)(ScreenSize.Y / 2), (int)(ScreenSize.Y * interpolator), (int)(ScreenSize.Y * interpolator));

                Main.spriteBatch.Draw(satelliteDishTargetByRequest.GetTarget(), frame, null, Color.White * interpolator, 0, TargetSize / 2, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (!inUI && ScaleAnim == 0)
                return;
            int fancyUIIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Fancy UI"));
            if (fancyUIIndex != -1)
            {
                layers.Insert(fancyUIIndex, new LegacyGameInterfaceLayer(
                    "WizenkleBoss: Satellite UI",
                    delegate
                    {
                        var snapshit = Main.spriteBatch.CaptureSnapshot();

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                        float interpolator = MathF.Pow(2f, 10 * (ScaleAnim - 1));
                        Main.spriteBatch.Draw(TextureRegistry.Pixel, new Rectangle(0, 0, (int)ScreenSize.X + 30, (int)ScreenSize.Y + 30), Color.Black * interpolator);

                        DrawMap();

                        Main.spriteBatch.DrawGenericBackButton(FontAssets.DeathText.Value, satelliteUI.BackPanel, ScreenSize, Language.GetTextValue("UI.Back"), 0.8f);

                        DrawConfigButton();

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
            if (inUI || ScaleAnim > 0 && !Main.dedServ)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (TerminalState != ContactingState.None)
                    HandleTerminal();
                else
                {
                    UpdatePrompt();
                    HandleInput();
                    UpdateSelection();
                }
            }
            HandleAnimations();
        }
    }
}
