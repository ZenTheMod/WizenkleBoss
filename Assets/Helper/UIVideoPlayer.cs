using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using WizenkleBoss.Assets.Config;

namespace WizenkleBoss.Assets.Helper
{
    public class UIVideoPlayerSystem : ModSystem
    {
        private bool Drawing = false;
        private static VideoPlayer VidPlayer;
        private static Texture2D VidTexture;
        public static void StartVideo(Video video)
        {
            VidPlayer = null;
            VidTexture = null;
            VidPlayer ??= new VideoPlayer();
            VidPlayer.IsLooped = false;
            VidPlayer?.Play(video);
            VidPlayer.Volume = ModContent.GetInstance<WizenkleBossConfig>().VideoVolume;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (VidPlayer != null)
            {
                MusicKiller.MuffleFactor = 0f;
                if (VidPlayer.State != MediaState.Paused && VidPlayer.State != MediaState.Stopped && Main.gamePaused)
                    VidPlayer.Pause();
                else if (VidPlayer.State == MediaState.Paused && VidPlayer.State != MediaState.Stopped && !Main.gamePaused)
                    VidPlayer.Resume();
                if (VidPlayer.Video.Duration.TotalMilliseconds <= VidPlayer.PlayPosition.TotalMilliseconds && VidPlayer.Video != null)
                {
                    Drawing = false;
                    VidPlayer.Stop();
                }
                if (VidPlayer.State != MediaState.Stopped)
                {
                    Drawing = true;
                    VidTexture = VidPlayer.GetTexture();
                    VidPlayer.Volume = ModContent.GetInstance<WizenkleBossConfig>().VideoVolume;
                }
                else
                {
                    Drawing = false;
                    VidPlayer.Stop();
                    VidPlayer = null;
                    VidTexture = null;
                    Drawing = false;
                }
            }
        }
        public override void OnWorldUnload()
        {
            if (VidPlayer != null)
            {
                Main.QueueMainThreadAction(() =>
                {
                    Drawing = false;
                    VidPlayer.Stop();
                    VidPlayer = null;
                    VidTexture = null;
                });
            }
        }
        public override void OnModUnload()
        {
            if (VidPlayer != null)
            {
                Main.QueueMainThreadAction(() =>
                {
                    Drawing = false;
                    VidPlayer.Stop();
                    VidPlayer = null;
                    VidTexture = null;
                });
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Main.dedServ)
                return;
            if (Drawing && !Main.gameMenu)
            {
                int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
                if (mouseTextIndex != -1)
                {
                    layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                        "WizenkleBoss: VideoPlayer",
                        delegate
                        {
                            if (VidTexture is null || VidPlayer.State == MediaState.Stopped || ModContent.GetInstance<WizenkleBossConfig>().VideoOpacity == 0f)
                                return true;
                            Main.spriteBatch.Draw(VidTexture, new Rectangle(Main.screenWidth / 2, Main.screenHeight / 2, Main.screenWidth, Main.screenHeight), new Rectangle(0, 0, VidTexture.Width, VidTexture.Height), Color.White * ModContent.GetInstance<WizenkleBossConfig>().VideoOpacity, 0f, new Vector2(VidTexture.Width / 2f, VidTexture.Height / 2f), SpriteEffects.None, 0f);
                            return true;
                        },
                        InterfaceScaleType.UI));
                }
            }
        }                                                                                      
        //                                                        ......                                    
        //                                                     .........:.                                  
        //                                                   .:::::::::::::.                                
        //                                         ..........:--.--:::::--::                                
        //               .....          ...:-------:-::::::::-------------=-.                               
        //            ..:::.:::..  ..:-----::::::::::::::::::::::::::----===:.                              
        //           .:---:::::-:----::::::::::::::::-:-:::::::::::::::::::--:..                            
        //           .-=---------::::--:::--------------------------::::::::::---:.                         
        //           .-==+=----------------------------------------------:-::::::---.      .......          
        //            .==------------------------------------------------------::::----. ::-::::.::..       
        //         .::-=-----------------------------------------------------------:---=-=----:::::::       
        //        .--=-----------====++***===--------------------------------------------+==-----::::.      
        //        -==-------=====+#%%%%%%%%*===*##%%###+::::-:----------------------------+***+=-----.      
        //       .-=----=======+*%%%%%%%%%%%+#%%%%%%%%%%%=-::::----------------------------++======-:       
        //      .-+---========+*%%%%%%%%%%%%%%%%%%%%%%%%%%---:------------------------==---=++++++-:::..    
        //      :+=-=========*#%%%%%%%%%%%%%%%%%%%%%%%%%%%=-------------------------======-==++=---:::::.   
        //     .============+#%%%%%%#-:+%%%%%%%%%%%%%%%%%%=----------------------============++==---:::::.  
        //     -+====++++===#%%%%%%+....+%%-:::%%%%%%%%%%%==-------==========================++====--:::::  
        //     =+====+++++++%%%%%%%:::::=%=::::#%%%%%%%%%#===================================++++=-=---:-:  
        //    .=+===+++++++*%%%%%%#:::::*%-::::#%%%%%%%%%*===================================+++++=====-:.  
        //    .++==++++++++*%%%%%%%::::-%%-:::=%%%%%%%%%%+==================================+++++++++=-..   
        //    .++==+*+++++++%%%%%%%#--*%#%=::=%%%%%%%%%%*===================================+++++=---::::   
        //    .=+++**********%%%%%%%%%%*+*%%%%%%%%%%%%%*====================================+++++==--::::.  
        //   .-====+*******++====++++==++**%%%%%%%%%%%+======================================+++*+==---::.  
        //  .======+***+-:::::::::::::::::::::-=+*#*+++++====================================+++++++=---:.  
        // .======+***-:::::::::::::::........::::::::-=+++++++++============================-=-+*++++=-.   
        //:+====++*++=------::::::::::::::::.......::::::::=+++++++=====================---::-: .::--:.     
        //+====++=:-======------:::::::::::::::::::::::::::::-++++*+==============++==-:::::--.             
        //++==++#:::-:-=++======-------:::::::::::::::::::::::-+++#*+=====++++++=:.::::::::--.              
        //=++++=. -:-=::-=++++++======---------:::::::::::::::-=+***+=====:::::-::--:::::---.               
        //..--..  .::====------===+++++++++====----------------=---+*=====-----------:----:                 
        //          .:=+====---------------====++++++++++==-----====#+=====--------------=.                 
        //           .:-+++====------------:-::::::::::::------=====*+======-==--------==.                  
        //            ..:-=+++=====---------------------------++++===*+========--------=:.                  
        //               ..:-==+++++++++=====------------====++++++++=+++==--------:----                    
        //                  ...::--==+++++++++++++++++====+===========-------------:---.                    
        //                          :::::----------------------------------===----:--:                      
        //                          .:::::----------------------------========------:                       
        //                            .:::-------=============================-----:                        
        //                              .:::----==============================---.                          
        //                                 ::-----==========================--=:                            
        //                                 :===--------===============--------:.                            
        //                                 :========--------------------==-----:                            
        //                                 .================+===========--------.                           
        //                                 ..==============++++========---------.                           
        //                                   .-++++=+=++++++==+++=====----------.                           
        //                                     ..-==+++++=-....-+++======------.                            
        //                                                      .:=+++++===+=-.                             
        //                                                          .::::::.                                 
    }
}
