using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Content.Dusts;
using MonoMod.Cil;
using WizenkleBoss.Content.UI;
using WizenkleBoss.Common.Ink;

namespace WizenkleBoss.Content.NPCs.InkCreature
{
    public class InkCreatureHelper : ModSystem, IDrawInInk
    {
        public class BeastTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Black);

                    // Deferred because of the lack of shaders.
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                foreach (var p in Main.ActiveNPCs)
                {
                    if (p.ModNPC is not IDrawWiggly drawer)
                        continue;
                    drawer.Shape();
                }

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }

            // Common rt stuff i do, 
            // TODO: make a proper rtcontentbyrequest loader.
        public static BeastTargetContent beastTargetByRequest;
        public override void Load()
        {
            beastTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(beastTargetByRequest);
        }

        public override void Unload()
        {
            Main.ContentThatNeedsRenderTargets.Remove(beastTargetByRequest);
        }

        public override void PostDrawTiles()
        {
            
        }

            // Draw it ONLY draw in ink.
        public void Shape()
        {
            if (!Main.npc.Where(npc => npc.active && npc.ModNPC is IDrawWiggly).Any())
                return;
            beastTargetByRequest.Request();
            if (beastTargetByRequest.IsReady)
            {
                var snapshit = Main.spriteBatch.CaptureSnapshot();

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                var Coronaries = Helper.CoronariesShader;
                var device = Main.instance.GraphicsDevice;

                Coronaries.Value.Parameters["globalTime"]?.SetValue(Main.GlobalTimeWrappedHourly * 2f);
                Coronaries.Value.Parameters["baseColor"]?.SetValue(new Color(85, 25, 255, 255).ToVector4());
                Coronaries.Value.Parameters["min"]?.SetValue(-0.8f);
                Coronaries.Value.Parameters["max"]?.SetValue(1.1f);

                device.Textures[1] = TextureRegistry.Space[1].Value;
                device.SamplerStates[1] = SamplerState.LinearWrap;

                Coronaries.Value.CurrentTechnique.Passes[0].Apply();

                Main.spriteBatch.Draw(beastTargetByRequest.GetTarget(), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(in snapshit);
            }
        }
    }
}
