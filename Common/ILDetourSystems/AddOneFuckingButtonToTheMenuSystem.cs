using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helpers;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.MenuStyles;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class AddOneFuckingButtonToTheMenuSystem : ModSystem
    {
        public static ILHook ImGonnaEMITDELEGATEFromTheChandelier;

        public override void Load()
        {
            MethodInfo MenuDraw = typeof(MenuLoader).GetMethod("UpdateAndDrawModMenuInner", BindingFlags.Static | BindingFlags.NonPublic);
            ImGonnaEMITDELEGATEFromTheChandelier = new(MenuDraw, IWantedToDrawHereBecauseFuckYou);
            ImGonnaEMITDELEGATEFromTheChandelier?.Apply();
        }

        public override void Unload()
        {
            ImGonnaEMITDELEGATEFromTheChandelier?.Dispose();
        }

        private static void IWantedToDrawHereBecauseFuckYou(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After,
                i => i.MatchCall("Terraria.ModLoader.MenuLoader", "OffsetModMenu"),
                i => i.MatchLdsfld<Main>("menuMode"), // uhh
                i => i.MatchBrtrue(out _));

                // Load the sb.
            c.EmitLdarg0();
                // Load the size of the menu style switcher.
            c.EmitLdloc(5);
                // Draw text but gay
            c.EmitDelegate((SpriteBatch sb, Vector2 switchTextSize) =>
            {
                if (MenuLoader.CurrentMenu != ModContent.GetInstance<InkPondMenu>())
                    return;

                bool heavyRain = ModContent.GetInstance<VFXConfig>().TitleScreenHeavyRain;

                string text = Language.GetTextValue(heavyRain ? "Mods.WizenkleBoss.MenuStyles.InkPondHeavyRain" : "Mods.WizenkleBoss.MenuStyles.InkPondLightRain");
                Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One);
                Vector2 position = new((Main.screenWidth / 2) - textSize.X / 2f, Main.screenHeight - 4 - textSize.Y - switchTextSize.Y);
                Rectangle switchTextRect = new((int)position.X, (int)position.Y, (int)textSize.X, (int)textSize.Y);

                bool isGay = Main.mouseMiddle;

                bool hovering = switchTextRect.Contains(Main.mouseX, Main.mouseY);

                if (hovering && !Main.alreadyGrabbingSunOrMoon)
                {
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        if (isGay)
                        {
                            SoundEngine.PlaySound(AudioRegistry.Fire with { Volume = 0.4f});
                            InkSystem.PrideMonth = !InkSystem.PrideMonth;
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.MenuTick);
                            ModContent.GetInstance<VFXConfig>().TitleScreenHeavyRain = !heavyRain;
                        }
                    }
                    if (Main.mouseRightRelease && Main.mouseRight)
                    {
                        SoundEngine.PlaySound(SoundID.MenuTick);
                        ModContent.GetInstance<VFXConfig>().Open(onClose: () =>
                        {
                            Main.menuMode = 0;
                        }, scrollToOption: nameof(VFXConfig.TitleScreenHeavyRain), centerScrolledOption: true);
                    }
                }
                ChatManager.DrawColorCodedStringWithShadow(sb, FontAssets.MouseText.Value, text, position, hovering ? (isGay ? Main.DiscoColor : Main.OurFavoriteColor) : new Color(120, 120, 120, 76), 0f, Vector2.Zero, Vector2.One);
            });
        }
    }
}
