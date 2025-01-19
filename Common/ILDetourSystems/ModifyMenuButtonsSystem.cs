using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Content.Dusts;
using MonoMod.Cil;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.MenuStyles;
using Terraria.Localization;
using Terraria.ID;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class ModifyMenuButtonsSystem : ModSystem
    {
        public override void Load()
        {
            IL_Main.DrawMenu += ModifyButtons;
        }

        public override void Unload()
        {
            IL_Main.DrawMenu -= ModifyButtons;
        }

        private void ModifyButtons(ILContext il)
        {
            ILCursor c = new(il);
            ILLabel target = c.DefineLabel();

                // uhhhh lets be EXTRA safe okay ?
            c.GotoNext(MoveType.After,
                i => i.MatchLdloca(2),
                i => i.MatchLdloc1(),
                i => i.MatchLdloc1(),
                i => i.MatchLdloc1(),
                i => i.MatchLdcI4(255),
                i => i.MatchCall<Color>(".ctor"));

            c.EmitLdloca(2);
            c.EmitDelegate((ref Color color) => {
                if (MenuLoader.CurrentMenu != ModContent.GetInstance<InkPondMenu>())
                    return;
                color = InkSystem.InkColor * 1.2f;
            });

                // Make sure we're ACTUALLY targeting the credits button
            c.GotoNext(MoveType.After,
                i => i.MatchLdstr("UI.Credits"));

            c.GotoNext(MoveType.After,
                i => i.MatchLdfld<Main>("selectedMenu"),
                i => i.MatchLdloc(45),
                i => i.MatchBneUn(out target));

            c.EmitDelegate(() => {
                if (MenuLoader.CurrentMenu != ModContent.GetInstance<InkPondMenu>())
                    return false;
                SoundEngine.PlaySound(SoundID.Tink);
                for (int i = 0; i < 250; i++)
                {
                    Vector2 rain = new(Main.rand.NextFloat(0, Main.screenWidth), Main.rand.NextFloat(0, Main.screenHeight));
                    InkRippleSystem.QueueRipple(rain * Main.UIScale, 1f, Vector2.One * Main.rand.NextFloat(0.25f, 4f));
                }
                return true;
            });
            c.EmitBrtrue(target);
        }
    }
}
