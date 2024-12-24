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
using WizenkleBoss.Assets.Config;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Assets.Helper
{
    public class HideCursorForMotaiSystem : ModSystem
    {
        public override void OnModLoad()
        {
            On_Main.DrawCursor += HideCursor;
            On_Main.DrawThickCursor += HideTHICKCursor;
        }
        public override void OnModUnload()
        {
            On_Main.DrawCursor -= HideCursor;
            On_Main.DrawThickCursor -= HideTHICKCursor;
        }

        private Vector2 HideTHICKCursor(On_Main.orig_DrawThickCursor orig, bool smart)
        {
            if (!ObservatorySatelliteDishUISystem.inUI)
                return orig(smart);

            bool hovering = ObservatorySatelliteDishUISystem.CanTargetStar();

            if (hovering)
                return Vector2.Zero;

            return orig(smart);
        }

        private void HideCursor(On_Main.orig_DrawCursor orig, Microsoft.Xna.Framework.Vector2 bonus, bool smart)
        {
            if (!ObservatorySatelliteDishUISystem.inUI)
                orig(bonus, smart);

            bool hovering = ObservatorySatelliteDishUISystem.CanTargetStar();

            if (hovering)
                return;
            orig(bonus, smart);
        }
    }
}
