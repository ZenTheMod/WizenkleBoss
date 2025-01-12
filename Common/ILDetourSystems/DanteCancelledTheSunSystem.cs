using Terraria;
using Terraria.ModLoader;
using MonoMod.Cil;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.Helper;
using Microsoft.Xna.Framework.Graphics;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class DanteCancelledTheSunSystem : ModSystem
    {
        public override void OnModLoad()
        {
            On_Main.DrawSunAndMoon += IAteTheFuckingMoonWatchaGonnaDoNowBitch;
            IL_Main.DrawSunAndMoon += IAteTheFuckingSunWithILEdits;
        }

        public override void OnModUnload()
        {
            On_Main.DrawSunAndMoon -= IAteTheFuckingMoonWatchaGonnaDoNowBitch;
            IL_Main.DrawSunAndMoon -= IAteTheFuckingSunWithILEdits;
        }

            // private static SupernovaState sunState;

        private void IAteTheFuckingSunWithILEdits(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After,
                i => i.MatchLdfld<Main.SceneArea>("SceneLocalScreenPositionOffset"),
                i => i.MatchCall<Vector2>("op_Addition"),
                i => i.MatchStloc(22));

                // load the two fucking colors for some reason ????
            c.EmitLdloca(18);
            c.EmitLdloca(19);
                    // c.EmitLdloc(22);
                    // c.EmitLdloc(6);

                // purple guy sun
            c.EmitDelegate((ref Color color, ref Color color2) =>   // , Vector2 position, float num5) =>
            {
                    // index outside of bounds of the array ._.
                if (Main.gameMenu)
                    return;

                if (InkSystem.AnyActiveInk)
                {
                    var player = Main.LocalPlayer.GetModPlayer<InkPlayer>();
                    float interpolator = player.Intoxication;

                    Color purple = new(85, 25, 255, 255);
                    color = Color.Lerp(color, purple, interpolator);
                    color2 = Color.Lerp(color2, purple, interpolator);
                }
            });
        }


            // Hide the moon (it looked at me funny)
        private void IAteTheFuckingMoonWatchaGonnaDoNowBitch(On_Main.orig_DrawSunAndMoon orig, Main self, Main.SceneArea sceneArea, Color moonColor, Color sunColor, float tempMushroomInfluence)
        {
            if (Main.gameMenu)
                orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
            else if (!InkSystem.AnyActiveInk)
                orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
            else
            {
                var player = Main.LocalPlayer.GetModPlayer<InkPlayer>();
                moonColor *= 1 - player.Intoxication;

                orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
            }
        }
    }
}
