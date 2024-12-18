using Microsoft.CodeAnalysis.CSharp.Syntax;
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
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Map;
using Terraria.ModLoader;
using WizenkleBoss.Content.Buffs;
using WizenkleBoss.Content.Tiles.Clouds;
using Terraria.ID;

namespace WizenkleBoss.Assets.Helper
{
    public class BanishRetroLightingSystem : ModSystem
    {
        public override void OnModLoad()
        {
                //IL_Main.DoDraw += MakeTheGameBetter;
            IL_CaptureCamera.DrawTick += FixCaptureCam;
        }
        public override void OnModUnload()
        {
                //IL_Main.DoDraw -= MakeTheGameBetter;
            IL_CaptureCamera.DrawTick -= FixCaptureCam;
        }

            // retro lighting is a god damn pain in my ass,
            // if ANYONE is reading this and would like to try fixing this shitty part of the game dm me pleaseeee !





            //private void MakeTheGameBetter(ILContext il)
            //{
            //    ILCursor c = new(il);

            //    ILLabel target = c.DefineLabel();

            //    c.GotoNext(MoveType.After,
            //        i => i.MatchBr(out _),
            //        i => i.MatchLdcI4(0),
            //        i => i.MatchStsfld<Main>("drawToScreen"));

            //    c.EmitBr(target);

            //    c.GotoNext(MoveType.After,
            //        i => i.MatchLdarg0(),
            //        i => i.MatchCall<Main>("ReleaseTargets"),
            //        i => i.MatchLdsfld<Main>("drawToScreen"),
            //        i => i.MatchBrtrue(out _));

            //    c.MarkLabel(target);



            //    c.GotoNext(MoveType.After, 
            //        i => i.MatchCall<Lighting>("get_NotRetro"),
            //        i => i.MatchBrfalse(out _),
            //        i => i.MatchLdsfld("Terraria.Graphics.Effects.Filters", "Scene"),
            //        i => i.MatchCallvirt<FilterManager>("CanCapture"),
            //        i => i.MatchBr(out _));

            //    c.EmitLdloc(10);
            //    c.EmitLdloca(11); // oooohhh look mom i made it editable :3

            //    c.EmitDelegate((bool flag, ref bool flag2) => flag2 = !(Main.netMode == NetmodeID.Server || flag) && !Main.mapFullscreen && Filters.Scene.CanCapture());

            //    //ILLabel target2 = c.DefineLabel();

            //    //c.GotoNext(MoveType.After, 
            //    //    i => i.MatchLdsfld<Main>("spriteBatch"),
            //    //    i => i.MatchLdcI4(9), 
            //    //    i => i.MatchLdcI4(1), 
            //    //    i => i.MatchCallvirt<OverlayManager>("Draw"));

            //    //c.EmitLdloc(11);
            //    //c.EmitBrtrue(target2);

            //    //c.GotoNext(MoveType.After,
            //    //    i => i.MatchLdloc(11),
            //    //    i => i.MatchBrfalse(out _));

            //    // c.MarkLabel(target2);
            //}
        private void FixCaptureCam(ILContext il)
        {
            ILCursor c = new(il);

            ILLabel target = c.DefineLabel();

            c.GotoNext(MoveType.After, 
                i => i.MatchLdarg0(),
                i => i.MatchLdfld("Terraria.Graphics.Capture.CaptureCamera", "_graphics"),
                i => i.MatchCall<Color>("get_Transparent"),
                i => i.MatchCallvirt<GraphicsDevice>("Clear"));

            c.EmitBr(target);

            c.GotoNext(MoveType.After, i => i.MatchLdloc0(),
                i => i.MatchBrfalse(out _));

            c.MarkLabel(target);
        }
    }
}
