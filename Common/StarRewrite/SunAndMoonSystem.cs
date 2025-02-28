using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Common.Config;

namespace WizenkleBoss.Common.StarRewrite
{
    public class SunAndMoonSystem : ModSystem
    {
        public static Vector2 SunMoonPosition;
        public static Color SunMoonColor;
        public static float SunMoonRotation;
        public static float SunMoonScale;

        public static Vector2 SceneAreaSize;

            // This is fine because this is ONLY changed on load.
        private static bool ShouldSkipSunMoonDrawing => ModContent.GetInstance<VFXConfig>().SunAndMoonRework;

            // public override bool IsLoadingEnabled(Mod mod) => ShouldSkipSunMoonDrawing;

        public override void Load() => IL_Main.DrawSunAndMoon += MakeSunAndMoonPretty;
        public override void Unload() => IL_Main.DrawSunAndMoon -= MakeSunAndMoonPretty;

        private void MakeSunAndMoonPretty(ILContext il)
        {
            ILCursor c = new(il);
            ILLabel sunSkipTarget = c.DefineLabel();
            ILLabel moonSkipTarget = c.DefineLabel();

            c.GotoNext(MoveType.After,
                i => i.MatchNewobj<Vector2>(),
                i => i.MatchLdarg1(),
                i => i.MatchLdfld<Main.SceneArea>("SceneLocalScreenPositionOffset"),
                i => i.MatchCall<Vector2>("op_Addition"),
                i => i.MatchStloc(22));

            c.EmitDelegate(() => ShouldSkipSunMoonDrawing);
            c.EmitBrtrue(sunSkipTarget);

            c.GotoNext(MoveType.Before,
                i => i.MatchLdsfld<Main>("dayTime"),
                i => i.MatchBrtrue(out _),
                i => i.MatchLdcR4(1f));

            c.MarkLabel(sunSkipTarget);

            c.EmitLdarg1(); // SceneArea
            c.EmitLdloc(22); // Position
            c.EmitLdloc(18); // Color
            c.EmitLdloc(7); // Rotation
            c.EmitLdloc(6); // Scale

            c.EmitDelegate(FetchInfo);

            c.GotoNext(MoveType.After,
                i => i.MatchNewobj<Vector2>(),
                i => i.MatchLdarg1(),
                i => i.MatchLdfld<Main.SceneArea>("SceneLocalScreenPositionOffset"),
                i => i.MatchCall<Vector2>("op_Addition"),
                i => i.MatchStloc(25));

            c.EmitDelegate(() => ShouldSkipSunMoonDrawing);
            c.EmitBrtrue(moonSkipTarget);

            c.GotoNext(MoveType.Before,
                i => i.MatchLdsfld<Main>("dayTime"),
                i => i.MatchBrfalse(out _),
                i => i.MatchLdloc(4));

            c.MarkLabel(moonSkipTarget);

            c.EmitLdarg1(); // SceneArea
            c.EmitLdloc(25); // Position
            c.EmitLdarg2(); // Color
            c.EmitLdloc(11); // Rotation
            c.EmitLdloc(10); // Scale

            c.EmitDelegate(FetchInfo); // (Rectangle?)new Rectangle(0, TextureAssets.Moon[num].Width() * Main.moonPhase, TextureAssets.Moon[num].Width(), TextureAssets.Moon[num].Width())
        }

        private void FetchInfo(Main.SceneArea sceneArea, Vector2 position, Color color, float rotation, float scale)
        {
            SunMoonPosition = position;
            SunMoonColor = color;
            SunMoonRotation = rotation;
            SunMoonScale = scale;

            SceneAreaSize = new(sceneArea.totalWidth, sceneArea.totalHeight);
        }
    }
}
