﻿using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using Terraria.GameContent;
using Terraria.Audio;
using Steamworks;

namespace WizenkleBoss.Common.Helpers
{
    public class TextureRegistry : ModSystem
    {
        public static readonly Asset<Texture2D> Invis = LoadTexture2D("MagicPixel");
        public static readonly Asset<Texture2D> Pixel = LoadTexture2D("NotSoMagicPixel");
        public static readonly Asset<Texture2D>[] Cosmos = [LoadTexture2D("Cosmos"), LoadTexture2D("Cosmos2")];
        public static readonly Asset<Texture2D> Bloat = LoadTexture2D("Noise/Bloat");
        public static readonly Asset<Texture2D> Roar = LoadTexture2D("Roar");
        public static readonly Asset<Texture2D> Shockwave = LoadTexture2D("Shockwave");
        public static readonly Asset<Texture2D>[] Space = [LoadTexture2D("Space"), LoadTexture2D("Space2"), LoadTexture2D("Space3")];
        public static readonly Asset<Texture2D> Star = LoadTexture2D("Stars/Star");
        public static readonly Asset<Texture2D> Bloom = LoadTexture2D("Bloom");
        public static readonly Asset<Texture2D> Ball = LoadTexture2D("Ball");
        public static readonly Asset<Texture2D> Circle = LoadTexture2D("Circle");
        public static readonly Asset<Texture2D> Bracket = LoadTexture2D("Bracket");
        public static readonly Asset<Texture2D> Tech = LoadTexture2D("Noise/Tech");
        public static readonly Asset<Texture2D> WavyNoise = LoadTexture2D("Noise/WavyNoise");
        public static readonly Asset<Texture2D> Lichen = LoadTexture2D("Noise/Lichen");
        public static readonly Asset<Texture2D> Dither = LoadTexture2D("Noise/Dither");
        public static readonly Asset<Texture2D> Wood = LoadTexture2D("Noise/Wood");
        public static readonly Asset<Texture2D> Rainbow = LoadTexture2D("Noise/PRIDEMONTH");
        public static readonly Asset<Texture2D> Blink = LoadTexture2D("Telescope/Blink");
        public static readonly Asset<Texture2D> BlinkOuter = LoadTexture2D("Telescope/BlinkOuter");
        public static readonly Asset<Texture2D> InkDash = LoadTexture2D("InkDash");
        public static readonly Asset<Texture2D> TextBoxStars = LoadTexture2D("Telescope/TextBoxStars");
        public static readonly Asset<Texture2D> TelescopeMap = LoadTexture2D("Telescope/Map");
        public static readonly Asset<Texture2D> Cursor = LoadTexture2D("Cursor");
        public static readonly Asset<Texture2D>[] ConsoleError = [LoadTexture2D("Icons/NotConnectedToPower"), LoadTexture2D("Icons/NotConnectedToSatelliteDish")];
        public static readonly Asset<Texture2D> ConfigIcon = LoadTexture2D("Icons/Settings");
        public static readonly Asset<Texture2D> MagnifyIcon = LoadTexture2D("Icons/Magnify");
        public static readonly Asset<Texture2D> EnergyBar = LoadTexture2D("Icons/PowerBar");
        private static Asset<Texture2D> LoadTexture2D(string TexturePath)
        {
            if (Main.dedServ)
                return null; // uuuuuuuuuhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh
            return ModContent.Request<Texture2D>("WizenkleBoss/Assets/Textures/" + TexturePath);
        }
    }
    public class FontRegistry : ModSystem
    {
            // Now you may be asking, ZEN (zoe)!, WHAT THE FUCK IS A 'SpriteFont' ?!??!?!/111!!?!?1/?one!!!1!1
            // Well I decided that I LOVE fonts (my note system looked stupid on mac) so; I decided to be INFINITLY better than those low life SCUMS (calamity mod) who use DynamicFontGenerator.exe.
            // I found this guide (https://stackoverflow.com/questions/55045066/how-do-i-convert-a-ttf-or-other-font-to-a-xnb-xna-game-studio-font);
            // Which gives DIRECT AND OBVIOUS steps on generating these files; (AND THEY WORK ON ALL PLATFORMS (prolly))
            // My interpretation of said steps are: 

                // Step 1: Know what font you're using. (dumbass tModlet)
                // Step 2: Install MonoGame for Visual Studio Community. (easy part)
                // Step 3: Create a new test project under the multiplatform game project preset.
                // Step 4: 'Open the MonoGame Pipeline Tool by double-clicking on the Content.mgcb file.'
                // Step 5: Press Edit > Add > New Item > SpriteFont Description.
                // Step 6: Edit it in notepad (or open it in vs if you're a based giga-pilled CHAD BRAD GIRLIEPOP) (It's like the same thing that DynamicFontGenerator.exe uses)
                // Step 7: In the 'MonoGame Pipeline Tool' Right Click > Rebuild, then you can find in in your 'Project/Content/bin/DesktopGL/' folder. (prolly)
                // Step 8: Load it asynchronously you won't.
        public static Asset<SpriteFont> Starlight { get; internal set; }

        public static Asset<SpriteFont> SpaceMono { get; internal set; }

        public static Asset<SpriteFont> Microserif { get; internal set; }

        public override void OnModLoad()
        {
            if (Main.dedServ)
                return;

            Starlight = ModContent.Request<SpriteFont>("WizenkleBoss/Assets/Fonts/Starlight");
            SpaceMono = ModContent.Request<SpriteFont>("WizenkleBoss/Assets/Fonts/SpaceMono");
            Microserif = ModContent.Request<SpriteFont>("WizenkleBoss/Assets/Fonts/Microserif");
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
        public static SoundStyle HumLoop { get; internal set; }
        public static SoundStyle HumEnd { get; internal set; }
        public static SoundStyle Beep { get; internal set; }
        public static SoundStyle BeepError { get; internal set; }
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

            HumStart = new SoundStyle("WizenkleBoss/Sounds/SatelliteAmbienceStart") with { Volume = 0.25f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = false };
            HumLoop = new SoundStyle("WizenkleBoss/Sounds/SatelliteAmbienceLoop") with { Volume = 0.15f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = false, IsLooped = true };
            HumEnd = new SoundStyle("WizenkleBoss/Sounds/SatelliteAmbienceEnd") with { Volume = 0.15f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PlayOnlyIfFocused = false };

            Select = new SoundStyle("WizenkleBoss/Sounds/Select1") with { Volume = 0.65f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.13f, Pitch = -0.1f, PlayOnlyIfFocused = true };
            Deselect = new SoundStyle("WizenkleBoss/Sounds/Deselect") with { Volume = 0.65f, MaxInstances = 3, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.13f, Pitch = -0.1f, PlayOnlyIfFocused = true };

            Beep = new SoundStyle("WizenkleBoss/Sounds/SatelliteBeep") with { Volume = 0.55f, MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.13f, PlayOnlyIfFocused = true };
            BeepError = new SoundStyle("WizenkleBoss/Sounds/SatelliteErrorBeep") with { Volume = 0.55f, MaxInstances = 10, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.13f, PlayOnlyIfFocused = true };

            Fire = new SoundStyle("WizenkleBoss/Sounds/SatelliteCharge") with { Volume = 0.9f, MaxInstances = 1, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, PitchVariance = 0.05f, PlayOnlyIfFocused = false };
        }
    }
}
