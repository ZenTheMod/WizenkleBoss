using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace WizenkleBoss.Common.Registries
{
    public class Sounds : ModSystem
    {
            // Completely unrelated but...
            // Ear destruction count: 8
            // To the many, many people who have worked on most popular 'freesound' websites (e.g. freesound, pixabay, or soundcloud)...
            // I would like to wish a fate worse than death upon each and every one of you, my reasons include:

                // 1. Let me tell you a story, t'was november 23rd 2024, a young trans girl wanted to find the perfect sound for her star destroying laser cannon;
                // She googled "free cosmic deathray sound effects" and clicked the first result, scrolling down she wanted to listen to the first one that caught her eye;
                // But then miliseconds after clicking, she realised the horrible mistake she'd made, as her ears were permanently deafened by the resulting sound that filled her headphones she screamed...
                // "FUCK YOU FREESOUND.ORG !!!!!!!!!!!!!!!!!"
                // But you see, *she* wasn't the one who made that horrible mistake, it was the dumbass mother fucking website developers, who forgot a key feature, a feature so simple.
                // Legends say that this one feature could cure cancer, and solve world hunger as well as cure aids and stop suicide around the globe.
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

            // For these reasons I'm suing every company that has committed these acts for: battery causing great bodily injury, sexual assult, giving me aids, and other reasons.
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
