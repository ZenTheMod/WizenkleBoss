using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using WizenkleBoss.Common.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI.Chat;
using Terraria.GameContent;
using Terraria.DataStructures;
using WizenkleBoss.Content.TileEntities;

namespace WizenkleBoss.Content.UI
{
    public class PowerDisplayUISystem : ModSystem
    {
        public static float Timer = 0;
        public static bool inUI = false;
        public static Vector2 WorldPosition;
        public static Vector2 WorldOffset;
        public static float DisplayValue = 0.5f;
        public override void UpdateUI(GameTime gameTime)
        {
            if (inUI)
            {
                Point16 pos = WorldPosition.ToTileCoordinates16();
                TileEntity.ByPosition.TryGetValue(pos, out TileEntity tileEntity);

                if (tileEntity is PowerSourceTileEntity powerSource)
                    DisplayValue = (float)powerSource.Charge / 500;
                else
                    inUI = false;

                if (Main.LocalPlayer.Distance(WorldPosition) >= 130)
                    inUI = false;
                if (Timer < 1f)
                    Timer += 0.14f;
            }
            else if (!inUI && Timer > 0f)
                Timer -= 0.14f;
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (!inUI && Timer <= 0)
                return;

            int UIIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Smart Cursor Targets"));
            if (UIIndex != -1)
            {
                layers.Insert(UIIndex, new LegacyGameInterfaceLayer(
                    "WizenkleBoss: Power Display UI",
                    delegate
                    {
                        var snapshit = Main.spriteBatch.CaptureSnapshot();

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                        Texture2D texture = TextureRegistry.EnergyBar.Value;

                        Rectangle frame = texture.Frame(1, 2, 0, 1);

                        Vector2 position = WorldPosition + WorldOffset - Main.screenPosition - (Vector2.UnitY * 20 * (1f - MathF.Pow(2, -10 * Timer)));
                        Main.spriteBatch.Draw(texture, position - (frame.Size() / 2), frame, Color.White * Timer, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                        Rectangle frame2 = texture.Frame(1, 2, 0, 0);
                        frame2.Width = (int)(frame2.Width * DisplayValue);
                        Main.spriteBatch.Draw(texture, position - (frame.Size() / 2), frame2, Color.White * Timer, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                        Main.spriteBatch.Draw(TextureRegistry.Bloom.Value, position - (frame.Size() / 2) + (frame2.Size() / 2), null, (Color.LightGoldenrodYellow * 0.5f * DisplayValue * Timer) with { A = 0 }, 0f, TextureRegistry.Bloom.Size() / 2f, (frame.Size() / TextureRegistry.Ball.Size()) * 1.4f, SpriteEffects.None, 0f);

                        string display = ((int)(DisplayValue * 500)).ToString() + " / 500";

                        Vector2 textSize = ChatManager.GetStringSize(FontAssets.DeathText.Value, display, Vector2.One);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.DeathText.Value, display, position + (Vector2.UnitY * 4), Color.White * Timer, 0, textSize / 2, Vector2.One * 0.3f, spread: 1.5f);

                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(in snapshit);

                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }
    }
}
