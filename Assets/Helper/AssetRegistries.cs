using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Audio;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace WizenkleBoss.Assets.Textures
{
    public class TextureRegistry : ModSystem
    {
        public static Texture2D Invis {  get; private set; }
        public static Texture2D Pixel { get; private set; }
        public static Texture2D[] Cosmos { get; private set; }
        public static Texture2D Bloat { get; private set; }
        public static Texture2D Roar { get; private set; }
        public static Texture2D Shockwave { get; private set; }
        public static Texture2D[] Space { get; private set; }
        public static Texture2D Star { get; private set; }
        public static Texture2D[] Stars { get; private set; }
        public static Texture2D Bloom { get; private set; }
        public static Texture2D Circle { get; private set; }
        public static Texture2D Bracket { get; private set; }
        public static Texture2D Smoke { get; private set; }
        public static Texture2D Tech { get; private set; }
        public static Texture2D WavyNoise { get; private set; }
        public static Texture2D Lichen { get; private set; }
        public static Texture2D Dither { get; private set; }
        public static Texture2D Wood { get; private set; }
        public static Texture2D Blink { get; private set; }
        public static Texture2D BlinkOuter { get; private set; }
        public static Texture2D InkDash { get; private set; }
        public static Texture2D TextBoxStars { get; private set; }
        public static Texture2D TelescopeMap { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Invis = LoadTexture2D("MagicPixel");

            Pixel = LoadTexture2D("NotSoMagicPixel");

            Cosmos = new Texture2D[2];
            Cosmos[0] = LoadTexture2D("Cosmos");
            Cosmos[1] = LoadTexture2D("Cosmos2");

            Bloat = LoadTexture2D("Noise/Bloat");
            Roar = LoadTexture2D("Roar");
            Shockwave = LoadTexture2D("Shockwave");

            Space = new Texture2D[3];
            Space[0] = LoadTexture2D("Space");
            Space[1] = LoadTexture2D("Space2");
            Space[2] = LoadTexture2D("Space3");

            Star = LoadTexture2D("Stars/Star");

            Stars = new Texture2D[5];
            Stars[0] = LoadTexture2D("Stars/Star_0");
            Stars[1] = LoadTexture2D("Stars/Star_1");
            Stars[2] = LoadTexture2D("Stars/Star_2");
            Stars[3] = LoadTexture2D("Stars/Star_3");
            Stars[4] = LoadTexture2D("Stars/Star_4");

            Bloom = LoadTexture2D("Bloom");

            Circle = LoadTexture2D("Circle");
            Bracket = LoadTexture2D("Bracket");

            Smoke = LoadTexture2D("Telescope/Smoke");

            Tech = LoadTexture2D("Noise/Tech");
            WavyNoise = LoadTexture2D("Noise/WavyNoise");
            Lichen = LoadTexture2D("Noise/Lichen");
            Dither = LoadTexture2D("Noise/Dither");

            Wood = LoadTexture2D("Noise/Wood");

            Blink = LoadTexture2D("Telescope/Blink");
            BlinkOuter = LoadTexture2D("Telescope/BlinkOuter");

            InkDash = LoadTexture2D("InkDash");

            TextBoxStars = LoadTexture2D("Telescope/TextBoxStars");

            TelescopeMap = LoadTexture2D("Telescope/Map");
        }
        private static Texture2D LoadTexture2D(string TexturePath)
        {
                //if (Main.netMode == NetmodeID.Server)
                //    return default;
            return ModContent.Request<Texture2D>("WizenkleBoss/Assets/Textures/" + TexturePath, AssetRequestMode.ImmediateLoad).Value;
        }
        public override void Unload()
        {
            Invis = null;
            Pixel = null;
            Cosmos = null;
            Bloat = null;
            Roar = null;
            Shockwave = null;
            Space = null;
            Star = null;
            Stars = null;
            Bloom = null;
            Circle = null;
            Bracket = null;
            Smoke = null;
            Tech = null;
            WavyNoise = null;
            Lichen = null;
            Dither = null;
            Wood = null;
            Blink = null;
            BlinkOuter = null;
            InkDash = null;
            TextBoxStars = null;
            TelescopeMap = null;
        }
    }
    public class FontRegistry : ModSystem
    {
        public static DynamicSpriteFont Starlight { get; internal set; }
        public static DynamicSpriteFont SpaceMono { get; internal set; }
        public static DynamicSpriteFont Papyrus { get; internal set; }
        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Starlight = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Starlight", AssetRequestMode.ImmediateLoad).Value;
                SpaceMono = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/SpaceMono", AssetRequestMode.ImmediateLoad).Value;
                Papyrus = Mod.Assets.Request<DynamicSpriteFont>("Assets/Fonts/Papyrus", AssetRequestMode.ImmediateLoad).Value;
            }
            else
            {
                Starlight = FontAssets.MouseText.Value;
                SpaceMono = FontAssets.DeathText.Value;
                Papyrus = FontAssets.DeathText.Value;
            }
        }
    }
    public class AudioRegistry : ModSystem
    {
            // Completely unrelated but...
                // Ear destruction count: 8
            // To the many, many people who have worked on most popular 'freesound' websites (e.g. freesound, pixabay, or soundcloud)...
            // I would like to wish a fate worse than death opon each and every one of you, my reasons include:

                // 1. Let me tell you a story, t'was november 23rd 2024, a young trans girl wanted to find the perfect sound for her star destroying laser cannon;
                // She googled "free cosmic deathray sound effects" and clicked the first result, scrolling down she wanted to listen to the first one that caught her eye;
                // But then miliseconds after clicking, she realised the horrible mistake she'd made, as her ears were permanently deafened by the resulting sound that filled her headphones she screamed...
                // "FUCK YOU FREESOUND.ORG !!!!!!!!!!!!!!!!!"
                // But you see, *she* wasnt the one who made that horrible mistake, it was the dumbass mother fucking website developers, who forgot a key feature, a feature so simple.
                // Legends say that this one feature could cure cancer, and solve world hunger as well as cure aids and stop suicide aroung the globe.
                // The feature they spoke of was...
                // A MOTHER FUCKING VOLUME SLIDER
                // HOW CAN THIS BE THAT GOD DAMN DIFFICULT??!?!?

                    // also the trans girl is me.



                // 2. Making people sign in to download sound effects;
                // I know for a damn good fact that all you're gonna do with my email after I verify it is spam me with articles and ads.



                // 3. As an extension of point 1, I would love to mention websites that autoplay sounds on hover.
                // Do you like having ears?
                // When you listen to shitty kpop do you install a volume slider chrome extension and then pay the developers money so you can crack the volume from 600% to 800% as well as bass boost it?
                    // btw this is a real thing.. the exact quote is
                    // '''
                    // Volume Master is a "pay what you like" software. You can use it for free, forever. 😊
                    // However, if you really like it, you can pay for it to support its development.
                    // As a thank-you you'll get 800 % volume boost.
                    // It's up to you and it's ok if you don't want to. Thank you and have a nice day! ☀️
                    // '''
                    // - https://chromewebstore.google.com/detail/volume-master/jghecgabfgfdldnmbfkhmffcabddioke

                        // If you "need" a 800% volume boost you're lying, and like, why do you need a bass boost option???

                // Do you purposefully adopt dogs so that you can have them bark in your ear when you light fireworks?
                // Are you my upstairs neighbor?
                // When you wake up do you turn up your alarm to fall back asleep?
                // Do you listen to EAS alarms while studying?
                // When you play music at an old folks home do the deaf cover their ears?

            // For these reasons I'm sueing every company that has committed these acts for: battery causing great bodily injury, sexual assult, giving me aids, and other reasons.
            // I am requesting at least: $467,589,489,634,896,754,986,756,758,936,793,465,975 in compensation.
            // You have until tommorow to comply.
        public static SoundStyle SateliteDeathray { get; internal set; }
        public static SoundStyle TelescopeOpen { get; internal set; }
        public static SoundStyle TelescopeClose { get; internal set; }
        public static SoundStyle TelescopePan { get; internal set; }
        public static SoundStyle NoteOpen { get; internal set; }
        public static SoundStyle HumStart { get; internal set; }
        public static SoundStyle Select { get; internal set; }
        public static SoundStyle Deselect { get; internal set; }
        public static SoundStyle Fire { get; internal set; }

        public static SoundStyle InkEffectDrinkStart { get; internal set; }
        public static SoundStyle InkEffectEnd { get; internal set; }
        public static SoundStyle InkDash { get; internal set; }
        public static SoundStyle InkDashEnd { get; internal set; }
        public static SoundStyle InkEnterTile { get; internal set; }
        public static SoundStyle InkExitTile { get; internal set; }
        public static SoundStyle InkBurrowing { get; internal set; }
        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            InkEffectDrinkStart = new SoundStyle("WizenkleBoss/Sounds/Ink/drink") with { Volume = 0.7f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = true };
            InkEffectEnd = new SoundStyle("WizenkleBoss/Sounds/Ink/effectend") with { Volume = 0.6f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = true };
            InkDash = new SoundStyle("WizenkleBoss/Sounds/Ink/dash") with { Volume = 0.5f, PitchVariance = 0.07f, MaxInstances = 5, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = true };
            InkDashEnd = new SoundStyle("WizenkleBoss/Sounds/Ink/dashend") with { Volume = 0.4f, PitchVariance = 0.07f, MaxInstances = 5, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = true };
            InkBurrowing = new SoundStyle("WizenkleBoss/Sounds/Ink/dig") with { Volume = 0.4f, PitchVariance = 0.5f, MaxInstances = 9, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = true };
            InkEnterTile = new SoundStyle("WizenkleBoss/Sounds/Ink/burrow") with { Volume = 0.55f, PitchVariance = 0.13f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = true };
            InkExitTile = new SoundStyle("WizenkleBoss/Sounds/Ink/unburrow") with { Volume = 0.55f, PitchVariance = 0.13f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = true };



            SateliteDeathray = new SoundStyle("WizenkleBoss/Sounds/SoundIMakeWhenIBlowUpTheSun") with { Volume = 0.7f, MaxInstances = 2, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = false };

            TelescopeOpen = new SoundStyle("WizenkleBoss/Sounds/TelescopeOpen") with { Volume = 1f, MaxInstances = 2, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.15f, PlayOnlyIfFocused = false };
            TelescopeClose = new SoundStyle("WizenkleBoss/Sounds/TelescopeClose") with { Volume = 1f, MaxInstances = 2, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.15f, PlayOnlyIfFocused = false };
            TelescopePan = new SoundStyle("WizenkleBoss/Sounds/TelescopePan") with { Volume = 0.15f, MaxInstances = 7, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.2f, Pitch = -0.4f, PlayOnlyIfFocused = false, Variants = new ReadOnlySpan<int>([1, 2, 3, 4]) };

            NoteOpen = new SoundStyle("WizenkleBoss/Sounds/NoteOpen") with { Volume = 0.7f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.1f, Pitch = 0f, PlayOnlyIfFocused = false };

            HumStart = new SoundStyle("WizenkleBoss/Sounds/ComputerHumStart") with { Volume = 0.1f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = false };

            Select = new SoundStyle("WizenkleBoss/Sounds/Select1") with { Volume = 0.65f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.13f, Pitch = -0.1f, PlayOnlyIfFocused = true};
            Deselect = new SoundStyle("WizenkleBoss/Sounds/Deselect") with { Volume = 0.65f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.13f, Pitch = -0.1f, PlayOnlyIfFocused = true };

            Fire = new SoundStyle("WizenkleBoss/Sounds/Select2") with { Volume = 0.9f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.05f, PlayOnlyIfFocused = false };
        }
    }
}
