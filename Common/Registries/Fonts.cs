using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace WizenkleBoss.Common.Registries
{
    public class Fonts : ModSystem
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
}
