using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.GameContent.Shaders;
using WizenkleBoss.Common.Helpers;

namespace WizenkleBoss.Common.Ink
{
    public readonly struct Ripple(Vector2 position, float intensity, float bloomIntensity, Vector2 size)
    {
        public readonly Vector2 Position = position;

        public readonly float Intensity = intensity;

        public readonly float BloomIntensity = bloomIntensity;

        public readonly Vector2 Size = size;
    }
    public class InkRippleSystem : ModSystem
    {
        private static Vector2 targetSize => new Vector2(Main.screenWidth, Main.screenHeight) * scale;
        
        public static RenderTarget2D rippleTarget;
        public static RenderTarget2D _rippleTarget;

        private const float scale = 0.25f;

        private static Vector2 lastDistortionDrawOffset = Vector2.Zero;

        private static Ripple[] ripples = new Ripple[50];

        private static int rippleCount;

        private static bool clearNextFrame = true;

        public static bool requestedThisFrame = false;

        public static bool isReady = false;

        public override void Load()
        {
            Main.OnPreDraw += CheckRippleTarget;
        }

        public override void Unload()
        {
            Main.OnPreDraw -= CheckRippleTarget;
        }

        private void CheckRippleTarget(GameTime obj)
        {
            if (requestedThisFrame)
            {
                requestedThisFrame = false;
                    // Keeping the typo
                HandleUseReqest(Main.instance.GraphicsDevice, Main.spriteBatch);
            }
        }

        private void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
        {
            PrepareARenderTargetButGoodBecauseGodForbidWomenDoAnything(ref rippleTarget, device, (int)targetSize.X, (int)targetSize.Y);
            PrepareARenderTargetButGoodBecauseGodForbidWomenDoAnything(ref _rippleTarget, device, (int)targetSize.X, (int)targetSize.Y);

            var bindings = device.GetRenderTargets();
            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            device.SetRenderTarget(_rippleTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

            ApplyShader();

            spriteBatch.Draw(rippleTarget, new Rectangle(0, 0, (int)targetSize.X, (int)targetSize.Y), Color.White);

            if (clearNextFrame)
            {
                device.Clear(new Color(0.5f, 0.5f, 0f, 1f));
                clearNextFrame = false;
            }
            spriteBatch.End();
            device.SetRenderTarget(rippleTarget);
            device.Clear(new Color(0.5f, 0.5f, 0f, 1f));
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.Identity);

            Vector2 position = GetPosition();
            Vector2 vector3 = position - lastDistortionDrawOffset;
            lastDistortionDrawOffset = position;
            spriteBatch.Draw(_rippleTarget, new Rectangle((int)vector3.X, (int)vector3.Y, (int)targetSize.X, (int)targetSize.Y), Color.White);

            DrawWaves(spriteBatch);

            spriteBatch.End();
            device.SetRenderTargets(bindings);
            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
            isReady = true;
        }

        private static Vector2 GetPosition()
        {
            if (Main.gameMenu)
                return Vector2.Zero;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 position = zero - Main.screenPosition;
            position *= scale;
            return position;
        }

        private static void ApplyShader()
        {
                // Simply put this shader (similarly to vanilla) uses colors to encode the simulation data:
                    // RED: Current state
                    // GREEN: Previous state
                    // BLUE: New ripples
                    // ALPHA: Unused
            var Processor = Helper.WaterProcessor;

            Processor.Value.Parameters["ScreenSize"]?.SetValue(targetSize);
            Processor.Value.Parameters["Decay"]?.SetValue(0.94f);    // DO NOT SET ABOVE 1.f
            Processor.Value.Parameters["RippleStrength"]?.SetValue(9f);

            Processor.Value.CurrentTechnique.Passes[0].Apply();
        }

        private static void DrawWaves(SpriteBatch spriteBatch)
        {
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 offset = zero - (lastDistortionDrawOffset / scale);
            if (Main.gameMenu)
                offset = Vector2.Zero;
            for (int l = 0; l < rippleCount; l++)
            {
                Ripple ripple = ripples[l];

                Vector2 position = ripple.Position - offset;
                Vector2 size = ripple.Size;

                Texture2D value = TextureRegistry.Bloom.Value;
                spriteBatch.Draw(value, position * scale, null, new Color(0f, 0f, ripple.Intensity * ripple.BloomIntensity) with { A = 0 }, 0f, value.Size() / 2, size * scale * 1.2f, SpriteEffects.None, 0f);

                Texture2D circle = TextureRegistry.Circle.Value;
                spriteBatch.Draw(circle, position * scale, null, new Color(0f, 0f, ripple.Intensity), 0f, circle.Size() / 2, size * scale, SpriteEffects.None, 0f);
            }
            rippleCount = 0;
        }

        /// <summary>
        /// If you're looking for a genuine summary you can go fuck yourself. <br /><br />
        /// I alone, spent THREE DAYS, THREE FUCKING DAYS ON THIS ONE ISSUE. <br />
        /// 'What issue?' you may ask, like the moron you are, well I'll tell you; <br />
        /// When I initially tried to use the fabled, lolxd approved <see cref="ARenderTargetContentByRequest"/> class, my fate was sealed from the moment I copy-pasted:
        /// <code>
        /// PrepareARenderTarget_AndListenToEvents(ref _target, device, Main.screenWidth, Main.screenHeight, (RenderTargetUsage)1);
        /// </code>
        /// You see <c>(RenderTargetUsage)1</c> translates to <see cref="RenderTargetUsage.PreserveContents"/>, my fatal mistake; <br />
        /// When you make a 'back buffer' system why would you EVER use <see cref="RenderTargetUsage.PreserveContents"/>??? you fucking moron, you ape, you orphaned cow. <br />
        /// You OBVIOUSLY use <see cref="RenderTargetUsage.DiscardContents"/> and manually set <see cref="Main.instance.GraphicsDevice.PresentationParameters.RenderTargetUsage"/> to <see cref="RenderTargetUsage.PreserveContents"/> like the good tmodlet you fucking are. <br />
        /// In conclusion: <br />
        /// I wasted 3 FUCKING DAYS on a INCORRECT CTOR, fuck you, have a merry january &lt;3
        /// </summary>
        /// <param name="target">The <see cref="RenderTarget2D"/> your stupid bitchass wants to set.</param>
        /// <param name="device">God forbid women have hobbies.</param>
        /// <param name="neededWidth">I WANNA BE SKINNY</param>
        /// <param name="neededHeight">I WANNA BE SHORT</param>
        private static void PrepareARenderTargetButGoodBecauseGodForbidWomenDoAnything(ref RenderTarget2D target, GraphicsDevice device, int neededWidth, int neededHeight)
        {
            if (target == null || target.IsDisposed || target.Width != neededWidth || target.Height != neededHeight)
            {
                clearNextFrame = true;
                target = new(device, neededWidth, neededHeight);
            }
        }

        public static void QueueRipple(Vector2 position, float intensity, Vector2 size, float bloom = 0.4f)
        {
            if (Main.drawToScreen)
                rippleCount = 0;
            else if (rippleCount < ripples.Length)
                ripples[rippleCount++] = new Ripple(position, intensity, bloom, size);
        }

        public override void OnWorldLoad()
        {
            clearNextFrame = true;
        }
    }
}
