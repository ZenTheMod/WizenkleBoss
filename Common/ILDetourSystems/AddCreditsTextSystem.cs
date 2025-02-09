using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Animations;
using Terraria.GameContent.Skies.CreditsRoll;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using MonoMod.Cil;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.MenuStyles;

namespace WizenkleBoss.Common.ILDetourSystems
{
        // THIS IS VERY MUCH NOT ZEN CODE

        // Huge, and I mean MASSIVE thanks to lion8cake for sharing this with me and letting me use it. prolly one of the best modders ive interacted with.
            //              ^^^^^^^ - sylvie istg if you look at this.
    public class AddCreditsTextSystem : ModSystem
    {
        public override void Load()
        {
            IL_CreditsRollComposer.FillSegments += FillCreditSegment;
            IL_Segments.LocalizedTextSegment.Draw += OurpleText;
        }

        public override void Unload()
        {
            IL_CreditsRollComposer.FillSegments -= FillCreditSegment;
            IL_Segments.LocalizedTextSegment.Draw -= OurpleText;
        }

        private SegmentInforReport PlayModdedTextRoll(CreditsRollComposer self, int startTime, string sourceCategory, Vector2 anchorOffset = default)
        {
                // We have our own text roll segment due to tmodloader using Hjson instead of json meaning that sometimes the order of names becomes backwards
                // if you want to use vanilla text for some reason i would recomend that you reflect CreditsRollComposer.PlaySegment_TextRoll
            List<IAnimationSegment> _segments = (List<IAnimationSegment>)typeof(CreditsRollComposer).GetField("_segments", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance).GetValue(self);
            anchorOffset.Y -= 40f;

            int num = 80;

            LocalizedText[] array = Language.FindAll(Lang.CreateDialogFilter(sourceCategory + ".", null));

            for (int i = 0; i < array.Length; i++)
            {
                _segments.Add(new Segments.LocalizedTextSegment(startTime + i * num, Language.GetText(sourceCategory + "." + (i + 1)), anchorOffset));
            }

            SegmentInforReport result = default;
            result.totalTime = array.Length * num + num * -1;
            return result;
        }

        private void FillCreditSegment(ILContext il)
        {
                // why does lion leave more comments then movie critics.
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before, 
                i => i.MatchLdloc0(), 
                i => i.MatchLdarg0(), 
                i => i.MatchLdloc0(), 
                i => i.MatchLdstr("CreditsRollCategory_Creator"), 
                i => i.MatchLdloc3());

                // make sure all instructions match, movetype will place our code before the first instruction once all instructions match
            c.EmitLdarg(0); //Emit ldarg_0 (self)
            c.EmitLdloca(0); //Emit ldloc_0 (num)
            c.EmitLdloca(2); //Emit ldloc_2 (num3)
            c.EmitLdloca(3); //Emit ldloc_3 (vector2 or val2)

            c.EmitDelegate((CreditsRollComposer self, ref int num, ref int num3, ref Vector2 vector2) => { //Get the needed variables and instance
                                                                                                           //Edit inside here for more text and animations, shown here is just how to add 1 text and 1 animation
                num += PlayModdedTextRoll(self, num, "Mods.WizenkleBoss.CreditsRoll", vector2).totalTime; //Play our credit text
                num += num3; // wait
            });
        }

        private void OurpleText(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After,
                i => i.MatchLdcI4(255),
                i => i.MatchCall<Main>("hslToRgb"),
                i => i.MatchStloc(8));

                // Load the ref Color used for the outline color.
            c.EmitLdloca(8);

                // shutup I know I can write better ils
            c.EmitDelegate((ref Color col) =>
            {
                if (!InkPondMenu.InMenu)
                    return;
                col = InkSystem.InkColor * 2f;
            });
        }
    }
}
