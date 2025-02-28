using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using WizenkleBoss.Common.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria;
using WizenkleBoss.Content.NPCs.InkCreature;
using ReLogic.Content;
using WizenkleBoss.Common.Ink;
using Terraria.ModLoader.UI;
using WizenkleBoss.Common.Registries;

namespace WizenkleBoss.Common.ILDetourSystems
{
    public class AnimatedModIconSystem : ModSystem
    {
        public static readonly Asset<Texture2D> BaseIcon = LoadTexture2D("icon");
        public static readonly Asset<Texture2D> InkIcon = LoadTexture2D("icon_ink");

        private static Asset<Texture2D> LoadTexture2D(string TexturePath)
        {
            if (Main.dedServ)
                return null; // uuuuuuuuuhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
            return ModContent.Request<Texture2D>("WizenkleBoss/" + TexturePath);
        }

        public class ModIconTargetContent : ARenderTargetContentByRequest
        {
            protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
            {
                PrepareARenderTarget_AndListenToEvents(ref _target, device, 80, 80, (RenderTargetUsage)1);

                device.SetRenderTarget(_target);
                device.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

                spriteBatch.Draw(BaseIcon.Value, Vector2.Zero, Color.White);
                spriteBatch.Draw(InkIcon.Value, Vector2.Zero, Color.White);

                var barrierShader = Shaders.ObjectInkShader;

                barrierShader.Value.Parameters["embossColor"]?.SetValue(InkSystem.InkColor.ToVector4());
                barrierShader.Value.Parameters["Size"]?.SetValue(new Vector2(80));
                barrierShader.Value.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

                barrierShader.Value.CurrentTechnique.Passes[0].Apply();

                spriteBatch.Draw(InkIcon.Value, Vector2.Zero, Color.White);

                spriteBatch.End();
                device.SetRenderTarget(null);
                _wasPrepared = true;
            }
        }

            // Common rt stuff i do, 
            // TODO: make a proper rtcontentbyrequest loader.
        public static ModIconTargetContent modIconTargetByRequest;

            // shut ip i know i can do this with a detour i just dont care <3
        public static Hook ModIconDrawDetour;
        public delegate void orig_Draw(object self, SpriteBatch spriteBatch);

        public override void Load()
        {
            MethodInfo Draw = typeof(UIModItem).GetMethod("Draw", BindingFlags.Instance | BindingFlags.Public);

            ModIconDrawDetour = new(Draw, UpdateModIcon);
            ModIconDrawDetour?.Apply();

            modIconTargetByRequest = new();
            Main.ContentThatNeedsRenderTargets.Add(modIconTargetByRequest);
        }

        public override void Unload()
        {
            ModIconDrawDetour?.Dispose();
            Main.ContentThatNeedsRenderTargets.Remove(modIconTargetByRequest);
        }

        private static void UpdateModIcon(orig_Draw orig, UIModItem self, SpriteBatch spriteBatch)
        {
            if (self._mod.Name != ModContent.GetInstance<WizenkleBoss>().Name)
            {
                orig(self, spriteBatch);
                return;
            }

            modIconTargetByRequest.Request();
            if (modIconTargetByRequest.IsReady)
                self._modIcon.SetImage(modIconTargetByRequest.GetTarget());
            orig(self, spriteBatch);
        }
    }
}
