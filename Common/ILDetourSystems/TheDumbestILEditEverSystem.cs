using Terraria;
using Terraria.ModLoader;
using MonoMod.Cil;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.Helpers;
using Microsoft.Xna.Framework.Graphics;
using WizenkleBoss.Content.UI;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria.UI;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class TheDumbestILEditEverSystem : ModSystem
    {
        public static ILHook NoImNotLettingYouConsumeMyItemAndHaveTheAUDACITYToPlayASFXOnTopOfIt;

        public override void Load()
        {
            MethodInfo RightClick = typeof(ItemLoader).GetMethod("RightClick", BindingFlags.Static | BindingFlags.Public);
            NoImNotLettingYouConsumeMyItemAndHaveTheAUDACITYToPlayASFXOnTopOfIt = new(RightClick, NOSOUNDBAD);
            NoImNotLettingYouConsumeMyItemAndHaveTheAUDACITYToPlayASFXOnTopOfIt?.Apply();

                // dont even ask
            IL_ItemSlot.TryOpenContainer += NOSOUNDBAD;
        }

        public override void Unload()
        {
            NoImNotLettingYouConsumeMyItemAndHaveTheAUDACITYToPlayASFXOnTopOfIt?.Dispose();

            IL_ItemSlot.TryOpenContainer -= NOSOUNDBAD;
        }

        private static void NOSOUNDBAD(ILContext il)
        {
            ILCursor c = new(il);
            ILLabel target = c.DefineLabel();

            c.GotoNext(MoveType.After,
                i => i.MatchLdarg0(),
                i => i.MatchLdarg1(),
                i => i.MatchCall("Terraria.ModLoader.ItemLoader", "RightClickCallHooks"));

            c.EmitLdarg0();

            c.EmitDelegate((Item item) => item.ModItem is IDontGetRightClicked);
            c.EmitBrfalse(target);
            c.EmitRet();

            c.MarkLabel(target);
        }
    }
}
