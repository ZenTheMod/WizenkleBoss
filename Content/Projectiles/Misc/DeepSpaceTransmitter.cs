using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;
using WizenkleBoss.Assets.Config;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Content.UI;
using static WizenkleBoss.Content.Projectiles.Misc.DeepSpaceTransmitterHelper;

namespace WizenkleBoss.Content.Projectiles.Misc
{
        // Super stupid in the long run.
    public enum LaserState
    {
        FadeIn,
        Open,
        FadeOut
    }
    public class DeepSpaceTransmitter : ModProjectile, IDrawLasers
    {
        public void Laser(SpriteBatch spriteBatch, GraphicsDevice device)
        {
                // Stops weird results when at really low charge values.
            if (charge <= 0.003)
                return;
            List<VertexInfo2> verticies = [];

                // Get an exponental charge value similar to how I got the center intensity value.
            float exponentialCharge = MathF.Pow(2, 10 * (charge - 1)) * 0.75f;
            for (float i = 0; i < _PointsCache.Count; i++)
            {
                float progress = i / _PointsCache.Count;

                    // Make the laser get bigger and wobblier
                float baseWidth = 480 * charge;
                float strongWidth = 830 * exponentialCharge + (MathF.Sin(i + Main.GlobalTimeWrappedHourly * -40) * 45);
                float width = MathHelper.Lerp(baseWidth, strongWidth, progress * exponentialCharge);

                    // Color color = Color.Lerp(Color.White, Color.Black, progress);

                        //                                                   :::::                                            
                        //                                                  :+@@@+:                                           
                        //                                                  :+@@@+:                                           
                        //          ____________________                    :+@@@*:                                           
                        //       /                        \                 :+@@@*:                                           
                        //      | its primitive time bitch |                :+@@@#:                                           
                        //       \                        /                 :*@@@%:                                           
                        //          ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾\ |                  :+@@@@::
                        //                              \|                :::+@@@@---:                                        
                        //  ::= *#*-::                   \                :+++======-:                                        
                        // :+@@@%@@@-:                                       ::...::
                        //:=%@+:::+@% -                                      :......::
                        // :%@*:  :+@*:                                    ::.......::
                        // ::-::  ::#@-:                                  ::.........::                                       
                        //         :-%@::                                 :.......:....:                                      
                        //          :*@@%:::                             :.... =.:::.....:                                     
                        //          -#@@@%####+-:::::::                 :...--:....:-....:                                    
                        //          ::*@@--= *#%%%%%@@@@#=----::::      ::..:....#-  ..:...:                                   
                        //            :=@#::  :::::::-=+++*%@@@%#**+-:::..:.   .%=    .:...::                                 
                        //             :*@*:               :::::::=#@@%=.:..   .+=.   .-....::                                
                        //             ::%@=:                       ::....:-.  .::   .::.....::::::::::                       
                        //              ::@% -:                      ::...... = -.    .-:::......*@@%%%%%#*=-:::::::        :%+: 
                        //               :+@*:                     ::..........+--:::...........::::::= +####%@@%%*====--:=%+: 
                        //                :#@+:                   ::.............................:       :::::::-===+#%@@@%:: 
                        //                :-@@=:                  :..................:-...........::                 ::::#@%-:
                        //                 :-@#:                 :...........+@%+-=#@@#............::                   :-%@*:
                        //                  :+@=:               :............+@@-..:+@% -............::                   :=@+:
                        //                  :-#@::             :.....................................::                    :  
                        //                   ::::             ::......................................:                       
                        //                                    ::...........:::........:==:.......:::::                        
                        //                                     :::::::::::*% +::::::::::=%% +::
                        //                                             :=@#-:          ::=%@+::                               
                        //                                            -*@+::             ::=%% +::
                        //                                            -#@::                ::+%%+::                           
                        //                                            :=%%=:                 ::+%% +::    :::                  
                        //                                             ::@% -:                  ::+@%=:::=%@-:                 
                        //                                           :====%@=:                   ::+%%%@% *-:                  
                        //  triangles are so hot am i right          :+#%%%%*:                     ::++-::                    
                        //                                              ::::                                                  

                verticies.Add(new VertexInfo2((_PointsCache[(int)i] - Main.screenPosition + new Vector2(width, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2)) / 2,
                    new Vector3(progress, 0f, 0f), // texCoord
                    Color.White)); // doesnt matter what color (itll get replaced by our shader anyway)

                    // Then copy my own homework and change - MathHelper.PiOver2 to + MathHelper.PiOver2;

                    // -- REMEMBER, verticies MUST be created CLOCKWISE, or whatever lolxd said. --
                verticies.Add(new VertexInfo2((_PointsCache[(int)i] - Main.screenPosition + new Vector2(width, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2)) / 2,
                    new Vector3(progress, 1f, 0f),
                    Color.White));
            }
            if (verticies.Count > 3)
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, verticies.ToArray(), 0, verticies.Count - 2);
        }
        public override string Texture => "WizenkleBoss/Assets/Textures/MagicPixel";
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 9000;

            Projectile.hide = true;
        }

            // WOO ENUMS
        private LaserState laserState = 0;
        private bool SoundPlayed = false;
        private int counter;

        private readonly List<Vector2> _PointsCache = [];
        public override void AI()
        {
            Center = Projectile.Center;
            Projectile.rotation = (-Vector2.One).ToRotation();

                // Close enough to a runonce bool.
            if (!SoundPlayed)
                PointSetUp();

            for (int i = 0; i < Particles.Length - 1; i++)
            {
                if (Particles[i] != Vector2.Zero)
                    Particles[i] = Projectile.timeLeft > 8998 || laserState == LaserState.FadeOut ? Vector2.Zero : Vector2.Lerp(Particles[i], Projectile.Center, 0.09f);
            }

            if (charge > 0.02)
            {
                if (TelescopeUISystem.inUI || StarMapUIHelper.inUI)
                {
                    Main.menuMode = 0;
                    IngameFancyUI.Close();
                }
            }
            switch (laserState)
            {
                default:
                    FadeInAI();
                    break;
                case LaserState.FadeIn:
                    FadeInAI();
                    break;
                case LaserState.Open:
                    OpenAI(); // omg just like the shitty ai company !!!!! yayyy i love stealing work and profiting off of it !!!!!!!!!!!!!!!!!!!!
                    break;
                case LaserState.FadeOut:
                    FadeOutAI();
                    break;
            }
        }
        public override void OnKill(int timeLeft)
        {
                // Kill whatever unfortionate star stands in my way :badasslonewolf:
            if (BarrierStarSystem.BigStar.State == SupernovaState.None && Projectile.ai[0] >= int.MaxValue / 2) // float conversion :3
                BarrierStarSystem.BigStar.State = SupernovaState.Shrinking;
            if ((int)Projectile.ai[0] != int.MaxValue && (int)Projectile.ai[0] > -1)
            {
                if (BarrierStarSystem.Stars[(int)Projectile.ai[0]].State == SupernovaState.None)
                    BarrierStarSystem.Stars[(int)Projectile.ai[0]].State = SupernovaState.Shrinking;
            }
        }
        private void PointSetUp()
        {
                // yeah they call me ms. efficient baby WOOOO.
            int length = 90;
            Vector2 endPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() * 3000;
            _PointsCache.Clear();
            Vector2 offset = Vector2.One * 20;
            for (int i = 0; i < length; i++)
                _PointsCache.Add(Vector2.Lerp(Projectile.Center + offset, endPoint, (float)i / (float)(length - 1)));
        }
        private void FadeInAI()
        {
                // Lustrous particles
            if (Main.rand.NextBool())
            {
                int n = Main.rand.Next(Particles.Length);
                if (Particles[n].Distance(Projectile.Center) < 1 || Particles[n].Length() < 2)
                {
                    Particles[n] = Projectile.Center + Main.rand.NextVector2CircularEdge(224, 224);
                }
            }

            if (!SoundPlayed)
            {
                    // Don't defean the elderly.
                if (ModContent.GetInstance<WizenkleBossConfig>().LaserLoop)
                    SoundEngine.PlaySound(AudioRegistry.SateliteDeathray);
                SoundPlayed = true;
            }
            MusicKiller.MuffleFactor = 1f - ((float)counter / 200f);
            if (counter >= 200)
            {
                if (darkness >= 0.3f)
                    this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 18, 7, 2, 0);
                if (charge >= 0.1f && darkness >= 1f)
                {
                    laserState = LaserState.Open;
                    counter = 0;
                    return;
                }
                if (SoundPlayed && charge < 0.1f)
                    charge += 0.001f;
                if (darkness < 1f)
                    darkness += 0.03f;
            }
            else
            {
                counter++;
            }
        }
        private void OpenAI()
        {
            MusicKiller.MuffleFactor = 0f;
            this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 40, 40, 2, 0);
            if (++counter > 435)
            {
                laserState = LaserState.FadeOut;
                counter = 0;
                return;
            }
            if (charge < 1f)
            {
                charge += 0.2f;
            }
        }
        private void FadeOutAI()
        {
            MusicKiller.MuffleFactor = 0f;
            if (darkness <= 0f)
            {
                counter = 0;
                Projectile.Kill();
            }
            else
            {
                darkness -= 0.0015f;
            }

            if (charge > 0.4f)
            {
                this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 12, 19, 2, 0);
                charge *= 0.98f;
            }
            else if (charge > 0f)
            {
                this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 2, 11, 2, 0);
                charge -= 0.002f;
            }
        }
    }
}
