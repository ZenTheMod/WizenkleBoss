﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Ink;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.ID;

namespace WizenkleBoss.Common.MenuStyles
{
    public class InkPondMenu : ModMenu
    {
        public override string DisplayName => Language.GetTextValue("Mods.WizenkleBoss.MenuStyles.InkPond");
        public override Asset<Texture2D> SunTexture => TextureRegistry.Invis;
        public override Asset<Texture2D> MoonTexture => TextureRegistry.Invis;

        private static Vector2 OldMouseScreen;

        private static bool hovering;

        public override void Update(bool isOnTitleScreen)
        {
            bool heavyRain = ModContent.GetInstance<VFXConfig>().TitleScreenHeavyRain;
            if (Main.rand.NextBool(19) || heavyRain)
            {
                Vector2 rain = new(Main.rand.NextFloat(0, Main.screenWidth), Main.rand.NextFloat(0, Main.screenHeight));
                InkRippleSystem.QueueRipple(rain * Main.UIScale, heavyRain ? 0.3f : 0.02f, Vector2.One * Main.rand.NextFloat(0.05f, heavyRain ? 0.7f : 0.4f));
            }
            if (isOnTitleScreen)
            {
                string text = Language.GetTextValue("Mods.WizenkleBoss.MenuStyles.InkPondRainConfig");
                Vector2 size = Helper.MeasureString(text, FontAssets.DeathText.Value) * Main.UIScale * 0.4f;
                Vector2 position = new(Main.screenWidth - size.X - 12, 12);
                Vector2 mousePosition = Main.MouseScreen;

                bool hovered = hovering;
                if (new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y).Contains((int)mousePosition.X, (int)mousePosition.Y))
                    hovering = true;
                else
                    hovering = false;

                if (hovered != hovering)
                    SoundEngine.PlaySound(SoundID.MenuTick);

                if (Main.mouseLeft && hovering)
                {
                    ModContent.GetInstance<VFXConfig>().Open(onClose: () =>
                    {
                        Main.menuMode = 0;
                    }, scrollToOption: nameof(VFXConfig.TitleScreenHeavyRain), centerScrolledOption: true);
                }
            }
            else
                hovering = false;
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            var snapshit = spriteBatch.CaptureSnapshot();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            bool heavyRain = ModContent.GetInstance<VFXConfig>().TitleScreenHeavyRain;

            spriteBatch.Draw(TextureRegistry.Pixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (heavyRain ? 1f : 0.9f));

            InkRippleSystem.QueueRipple(Main.MouseScreen * Main.UIScale, 0.8f, Vector2.One * 0.4f);

            if (OldMouseScreen.Distance(Main.MouseScreen) > 100)
                InkRippleSystem.QueueRipple(Vector2.Lerp(Main.MouseScreen, OldMouseScreen, 0.5f) * Main.UIScale, 0.8f, Vector2.One * 0.4f);

            OldMouseScreen = Main.MouseScreen;

            InkRippleSystem.requestedThisFrame = true;
            if (InkRippleSystem.isReady)
            {
                var Ink = Helper.WaterInkColorizer;

                Ink.Value.Parameters["InkColor"]?.SetValue(InkSystem.InkColor.ToVector4());
                Ink.Value.Parameters["RippleStrength"]?.SetValue(20f);

                Ink.Value.CurrentTechnique.Passes[1].Apply();
                spriteBatch.Draw(InkRippleSystem.rippleTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            }

            var barrierShader = Helper.ObjectBarrierShader;

            barrierShader.Value.Parameters["embossColor"]?.SetValue(InkSystem.InkColor.ToVector4());
            barrierShader.Value.Parameters["Size"]?.SetValue(Logo.Value.Size());
            barrierShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            barrierShader.Value.CurrentTechnique.Passes[0].Apply();

            Vector2 logoOffset = new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 0.3f), MathF.Cos(Main.GlobalTimeWrappedHourly * 0.07f)) * 13f;
            Vector2 logoPosition = logoDrawCenter + logoOffset;
            spriteBatch.Draw(Logo.Value, logoPosition, null, Color.White, 0f, new Vector2(Logo.Value.Width / 2f, Logo.Value.Height / 2f), logoScale, SpriteEffects.None, 0f);

            InkRippleSystem.QueueRipple(logoPosition * Main.UIScale, 0.3f, Vector2.One * logoScale * 5f);
                
                // BAD IDEA
            if (Main.menuMode != 0 || !heavyRain)
            {
                spriteBatch.End();
                spriteBatch.Begin(in snapshit);
            }
            if (Main.menuMode == 0)
            {
                string text = Language.GetTextValue("Mods.WizenkleBoss.MenuStyles.InkPondRainConfig");

                Color color = hovering ? Main.OurFavoriteColor : Color.Gray;

                Vector2 size = Helper.MeasureString(text, FontAssets.DeathText.Value);

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, new Vector2(Main.screenWidth - 12, 12), color, 0f, new Vector2(size.X, 0), Vector2.One * 0.4f);
            }
            return false;
        }
    }
}
