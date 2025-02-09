using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class HideCursorSystem : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawCursor += HideCursor;
            On_Main.DrawThickCursor += HideTHICKCursor;
        }

        public override void Unload()
        {
            On_Main.DrawCursor -= HideCursor;
            On_Main.DrawThickCursor -= HideTHICKCursor;
        }

        private Vector2 HideTHICKCursor(On_Main.orig_DrawThickCursor orig, bool smart)
        {
            bool cursormode = ModContent.GetInstance<UIConfig>().SatelliteUseMousePosition;

            if (!StarMapUIHelper.inUI || !cursormode)
                return orig(smart);

            bool hovering = StarMapUIHelper.CanTargetStar();

            if (hovering)
                return Vector2.Zero;

            return orig(smart);
        }

        private void HideCursor(On_Main.orig_DrawCursor orig, Vector2 bonus, bool smart)
        {
            bool cursormode = ModContent.GetInstance<UIConfig>().SatelliteUseMousePosition;

            if (!StarMapUIHelper.inUI || !cursormode)
                orig(bonus, smart);

            bool hovering = StarMapUIHelper.CanTargetStar();

            if (hovering)
                return;
            orig(bonus, smart);
        }
    }
}
