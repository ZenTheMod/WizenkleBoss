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
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Content.Buffs;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class GaslightSystem : ModSystem
    {
        public override void OnModLoad()
        {
            On_PlayerHeadDrawRenderTargetContent.UsePlayer += HideGhosts;
            IL_NewMultiplayerClosePlayersOverlay.Draw += HideGhosts;
        }
        public override void OnModUnload()
        {
            On_PlayerHeadDrawRenderTargetContent.UsePlayer -= HideGhosts;
            IL_NewMultiplayerClosePlayersOverlay.Draw -= HideGhosts;
        }
        private void HideGhosts(On_PlayerHeadDrawRenderTargetContent.orig_UsePlayer orig, PlayerHeadDrawRenderTargetContent self, Player player)
        {
            if (player.HasBuff<InkDrugBuff>() || player.HasBuff<InkDrugStatBuff>())
                orig(self, null);
            else
                orig(self, player);
        }
            // Huge thanks to lion8cake/Kittenbun.
        private void HideGhosts(ILContext il)
        {
            ILCursor c = new(il);

                // Get the target to go to if the player has the buff.
                // In this case we're looking for the line
                    // beq IL_0232
                // This is the line that tells the cursor to goto the next loop, so increment i and restart the loop.
            ILLabel target = c.DefineLabel();

                // We need the output target from the beq command so we match for the following:
                    // IL_009b: ldloc.s 13
                    // IL_009d: ldloc.s 7
                    // IL_009f: beq IL_0232 -- what we're after.
            c.GotoNext(MoveType.After,
                i => i.MatchLdloc(13),
                i => i.MatchLdloc(7),
                i => i.MatchBeq(out target));

                // Then we need to insert our if statement after the lines:
                    // Player player3 = player[i]; -- What the IL instructions represent.
                    // IL_00a4: ldloc.s 6
                    // IL_00a6: ldloc.s 13
                    // IL_00a8: ldelem.ref
                    // IL_00a9: stloc.s 14 // Store local "player3".
            c.GotoNext(MoveType.After,
                i => i.MatchLdloc(13),
                i => i.MatchLdelemRef(),
                i => i.MatchStloc(14));

                // Load player3 to the stack.
            c.EmitLdloc(14);

                // Emit a bool and if its true then coniture the loop.
            c.EmitDelegate((Player player3) => player3.GetModPlayer<InkPlayer>().InkBuffActive);
            c.EmitBrtrue(target);

                // You can think of this as inserting:
                    // if (player3.HasBuff<InkDrugBuff>() && player3.HasBuff<InkDrugStatBuff>())
                    // {
                    //     continue;
                    // }
                // In the middle of:
                    //for (int i = 0; i < 255; i++)
                    //{
                    //    if (i == myPlayer)
                    //    {
                    //        continue;
                    //    }
                    //    Player player3 = player[i];

                // Insert the if here.

                    //    if (!player3.active || player3.dead || player3.team != player2.team)
                    //    {
                    //        continue;
                    //    }

                // Again huge thanks to lion8cake/Kittenbun as without their help I would've had to learn the dark arts by trial and error.
        }
    }
}
