using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using MonoMod.Cil;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Common.MenuStyles;
using WizenkleBoss.Common.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class ForceDrawInkSystem : ModSystem
    {
        public override void Load()
        {
            IL_FilterManager.EndCapture += ApplyInkEffect;
            On_FilterManager.Update += UpdateInkEffect;
        }

        public override void Unload()
        {
            IL_FilterManager.EndCapture -= ApplyInkEffect;
            On_FilterManager.Update -= UpdateInkEffect;
        }

        private void UpdateInkEffect(On_FilterManager.orig_Update orig, FilterManager self, GameTime gameTime)
        {
            orig(self, gameTime);
            if (Helper.InkShader == null)
                return;
            if (Helper.InkShader.Active)
                Helper.InkShader.Update(gameTime);
        }

        private void ApplyInkEffect(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.After,
                i => i.MatchStloc1(),
                i => i.MatchStloc0(),
                i => i.MatchLdloc0(),
                i => i.MatchBrtrue(out _));

            c.EmitLdloc(4);
            c.EmitLdloca(2);
            c.EmitLdloca(3);
            c.EmitLdarg2();
            c.EmitLdarg3();
            c.EmitLdarg(4);
            c.EmitDelegate((GraphicsDevice graphicsDevice, ref RenderTarget2D renderTarget2D, ref RenderTarget2D renderTarget2D2, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor) => {
                if (Helper.InkShader == null)
                    return;
                if (!Helper.InkShader.Active)
                    return;

                    // idk if i need to even do this...
                RenderTarget2D screenTarget = (renderTarget2D2 != screenTarget1) ? screenTarget1 : screenTarget2;

                renderTarget2D = screenTarget;

                graphicsDevice.SetRenderTarget(renderTarget2D);
                graphicsDevice.Clear(clearColor);

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                Helper.InkShader.Apply();
                Main.spriteBatch.Draw(renderTarget2D2, Vector2.Zero, Main.ColorOfTheSkies);

                Main.spriteBatch.End();

                renderTarget2D2 = screenTarget;
            });
        }
    }
}
