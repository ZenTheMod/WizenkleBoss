using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;
using WizenkleBoss.Common.Config;
using WizenkleBoss.Common.Helper;
using WizenkleBoss.Content.Dusts;
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

                    // The little ripple that matches the sfx motai made.
                float extraWidth = MathF.Max(1 - MathF.Abs(wave - progress) * (_PointsCache.Count / 3), 0) * 90;

                float width = MathHelper.Lerp(baseWidth, strongWidth, progress * exponentialCharge) + extraWidth;

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
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 9000;

            Projectile.hide = true;
        }

            // WOO ENUMS
        private LaserState laserState = 0;

        private bool SoundPlayed = false;
        private int counter;
        private float wave = 0.95f;

        private const float BeamLength = 3500;

        private readonly List<Vector2> _PointsCache = [];

        public override bool? CanHitNPC(NPC target)
        {
                // No boss cheese for youuuu :3333333333333
            if (target.boss)
                return false;
            return base.CanHitNPC(target);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (charge <= 0.2)
                return false;
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float _ = float.NaN;
            Vector2 endPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() * BeamLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint, 530 * charge, ref _);
        }
        public override void AI()
        {
            Center = Projectile.Center;
            Projectile.rotation = (-Vector2.One).ToRotation();

                // Close enough to a runonce bool.
            if (!SoundPlayed && !Main.dedServ)
                PointSetUp();

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
            int length = 120;
            Vector2 endPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() * BeamLength;
            _PointsCache.Clear();
            Vector2 offset = Vector2.One * 30;
            for (int i = 0; i < length; i++)
                _PointsCache.Add(Vector2.Lerp(Projectile.Center + offset, endPoint, (float)i / (float)(length - 1))); // FLOAT CAST (I FORGOT THIS, DO NOT PULL A ZEN)
        }

        private void FadeInAI()
        {
                // Lustrous particles
            if (Main.rand.NextBool((int)Utils.Remap(counter, 0, 140, 30, 2)) && !Main.dedServ)
            {
                Color color = new Color(255, 196, 255) * Main.rand.NextFloat(0.3f, 1.1f);
                color.A = 0;
                Vector2 StartPosition = Projectile.Center + Main.rand.NextVector2CircularEdge(1000, 1000);
                Dust.NewDustPerfect(StartPosition, ModContent.DustType<StarSpiralDust>(), Vector2.Zero, 0, color);
            }

            if (!SoundPlayed && !Main.dedServ)
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
                    // mmmmmm ripple
                wave -= 0.0105f;
            }
            else
            {
                counter++;
            }
        }

        private void OpenAI()
        {
                // Lustrous particles
            if (Main.rand.NextBool() && !Main.dedServ)
            {
                float Magnitude = Main.rand.NextFloat(-120f, 120f);
                Color color = Color.Lerp(new Color(99, 0, 197), new Color(255, 196, 196), Math.Abs(Magnitude) / 120f) * Main.rand.NextFloat(0.3f, 1.1f);
                color.A = 0;
                Vector2 Velocity = new Vector2(Magnitude, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LerpAngleStarDust>(), Velocity, 0, color);
            }

            MusicKiller.MuffleFactor = 0f;
                // mmmmm banish ripple to shadow realm :fire:
            wave = -2;
            this.CameraShakeSimple(Projectile.Center, Vector2.Zero, 40, 40, 2, 0);
            if (++counter > 405)
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
