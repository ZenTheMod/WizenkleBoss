using Microsoft.Xna.Framework.Graphics;
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
using Terraria.Graphics.Effects;
using Terraria.GameContent.Skies;
using Terraria.GameContent.Events;
using WizenkleBoss.Common.Registries;

namespace WizenkleBoss.Common.MenuStyles
{
    public class InkPondMenu : ModMenu
    {
        public override string DisplayName => Language.GetTextValue("Mods.WizenkleBoss.MenuStyles.InkPond");
        public override Asset<Texture2D> SunTexture => Textures.Invis;
        public override Asset<Texture2D> MoonTexture => Textures.Invis;

        private static Vector2 OldMouseScreen;

        public static bool HeavyRain => ModContent.GetInstance<VFXConfig>().TitleScreenHeavyRain;

        public static bool InMenu => MenuLoader.CurrentMenu == ModContent.GetInstance<InkPondMenu>() && Main.gameMenu;

        public override void Update(bool isOnTitleScreen)
        {
                // Random rain droplets (why am I procrastinating doing proper shit writing comments ??)
                // Like seriously the chances anyone reads this is so abysmal.
                // If somehow you are reading this lmk on discord. (@z_e_n_.)
            if (Main.rand.NextBool(19) || HeavyRain)
            {
                Vector2 rain = new(Main.rand.NextFloat(0, Main.screenWidth), Main.rand.NextFloat(0, Main.screenHeight));
                InkRippleSystem.QueueRipple(rain, HeavyRain ? 0.3f : 0.02f, Vector2.One * Main.rand.NextFloat(0.05f, HeavyRain ? 0.7f : 0.4f));
            }
        }

        public override void OnSelected() => OldMouseScreen = Main.MouseScreen;

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
                // no i will not rename snapshit to snapshot <3
            var snapshit = spriteBatch.CaptureSnapshot();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            spriteBatch.Draw(Textures.Pixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * (HeavyRain ? 1f : 0.9f));

                // Mouse ripple trail
            Vector2 MouseScale = Vector2.One * 0.4f * Main.UIScale;
            InkRippleSystem.QueueRipple(Main.MouseScreen, 0.8f, MouseScale);

                // Make it a bit more fluid when moving eradically.
            if (OldMouseScreen.Distance(Main.MouseScreen) > 400)
            {
                for (float i = 0.05f; i < 1f; i += 0.05f)
                    InkRippleSystem.QueueRipple(Vector2.Lerp(Main.MouseScreen, OldMouseScreen, i), 0.8f, MouseScale);
            }
            else if (OldMouseScreen.Distance(Main.MouseScreen) > 200)
            {
                for (float i = 0.1f; i < 1f; i += 0.1f)
                    InkRippleSystem.QueueRipple(Vector2.Lerp(Main.MouseScreen, OldMouseScreen, i), 0.8f, MouseScale);
            }
            else if (OldMouseScreen.Distance(Main.MouseScreen) > 60)
            {
                for (float i = 0.25f; i < 1f; i += 0.25f)
                    InkRippleSystem.QueueRipple(Vector2.Lerp(Main.MouseScreen, OldMouseScreen, i), 0.8f, MouseScale);
            }

                // bwaaaa
            OldMouseScreen = Main.MouseScreen;

                // Actually draw the ripples.
            InkRippleSystem.requestedThisFrame = true;
            if (InkRippleSystem.isReady)
            {
                var Ink = Shaders.WaterProcessor;

                Ink.Value.Parameters["InkColor"]?.SetValue(InkSystem.InkColor.ToVector4());
                Ink.Value.Parameters["RippleStrength"]?.SetValue(20f);

                Ink.Value.CurrentTechnique.Passes[1].Apply();
                spriteBatch.Draw(InkRippleSystem.rippleTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            }

            var inkShader = Shaders.ObjectInkShader;

            inkShader.Value.Parameters["embossColor"]?.SetValue(InkSystem.InkColor.ToVector4());
            inkShader.Value.Parameters["Size"]?.SetValue(Logo.Value.Size());
            inkShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            inkShader.Value.CurrentTechnique.Passes[0].Apply();

            Vector2 logoOffset = new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 0.3f), MathF.Cos(Main.GlobalTimeWrappedHourly * 0.07f)) * 13f;
            Vector2 logoPosition = logoDrawCenter + logoOffset;
            spriteBatch.Draw(Logo.Value, logoPosition, null, Color.White, 0f, new Vector2(Logo.Value.Width / 2f, Logo.Value.Height / 2f), logoScale, SpriteEffects.None, 0f);

                // add a deep well beneath the tml logo.
            InkRippleSystem.QueueRipple(logoPosition, 0.3f, Vector2.One * logoScale * 4.3f);

                // Refer to DisableCustomSkiesSystem for a slight bit more info, the short of it is this:
                    // Theres a common bug pertaining to the credits menu that no one seems to care enough to fix.
                    // When using ModMenus that have custom backgrounds (like calamity mid (cus its mid)) it doesnt display the credits text due to it being a background layer.
                    // So I chose to manually draw it instead, not the most efficient for sure, but idc.
            if (SkyManager.Instance["CreditsRoll"].IsActive() && SkyManager.Instance["CreditsRoll"] is CreditsRollSky creditsRollSky)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                Main.CurrentFrameFlags.Hacks.CurrentBackgroundMatrixForCreditsRoll = Main.UIScaleMatrix;

                creditsRollSky.Draw(spriteBatch, 0, 2);
            }

            spriteBatch.End();
            spriteBatch.Begin(in snapshit);
            return false;
        }
    }
}
