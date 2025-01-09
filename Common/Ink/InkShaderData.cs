using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;

namespace WizenkleBoss.Common.Ink
{
    public class InkShaderData : ScreenShaderData
    {
        public const string ShaderKey = "InkScreen";
        public InkShaderData(Asset<Effect> shader, string passName)
            : base(shader, passName)
        {
        }
        public static void ToggleActivityIfNecessary()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            bool shouldBeActive = InkSystem.AnyActiveInk();

            if (shouldBeActive && !Filters.Scene[$"WizenkleBoss:{ShaderKey}"].IsActive())
                Filters.Scene.Activate($"WizenkleBoss:{ShaderKey}");

            if (!shouldBeActive && Filters.Scene[$"WizenkleBoss:{ShaderKey}"].IsActive())
                Filters.Scene.Deactivate($"WizenkleBoss:{ShaderKey}");
        }
        public override void Update(GameTime gameTime)
        {
            InkSystem.inkTargetByRequest.Request();

            if (InkSystem.inkTargetByRequest.IsReady && InkSystem.insideInkTargetByRequest.IsReady)
            {
                var player = Main.LocalPlayer.GetModPlayer<InkPlayer>();

                Shader.Parameters["embossColor"]?.SetValue(new Color(85, 25, 255, 255).ToVector4());

                Shader.Parameters["outlineColor"]?.SetValue(InkSystem.OutlineColor.ToVector4());

                Shader.Parameters["ScreenSize"]?.SetValue(new Vector2(Main.screenWidth, Main.screenHeight));

                Shader.Parameters["MaskThreshold"]?.SetValue(0.4f);

                Shader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

                Shader.Parameters["DrugStrength"]?.SetValue(player.Intoxication);

                Shader.Parameters["contrast"]?.SetValue(ModContent.GetInstance<WizenkleBossConfig>().InkContrast);

                Shader.Parameters["embossStrength"]?.SetValue(ModContent.GetInstance<WizenkleBossConfig>().EmbossStrength);
            }
        }
        public override void Apply()
        {
            var gd = Main.instance.GraphicsDevice;

            gd.SamplerStates[0] = SamplerState.LinearClamp;

            gd.Textures[1] = InkSystem.inkTargetByRequest.GetTarget();
            gd.SamplerStates[1] = SamplerState.LinearWrap;

            gd.Textures[2] = InkSystem.insideInkTargetByRequest.GetTarget();
            gd.SamplerStates[2] = SamplerState.LinearWrap;

            base.Apply();

            InkSystem.InsideInkTargetDrawnToThisFrame = false;
        }
    }
}
