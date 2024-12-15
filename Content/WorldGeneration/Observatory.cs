using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using StructureHelper;
using Terraria.IO;
using Terraria.Localization;
using Terraria.DataStructures;

namespace WizenkleBoss.Content.WorldGeneration
{
    public class ObservatoryGenSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Island Houses"));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new ObservatoryGenPass("Observatory", 100f));
            }
        }
    }
    public class ObservatoryGenPass : GenPass
    {
        public ObservatoryGenPass(string name, float loadWeight) : base(name, loadWeight) { }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetTextValue("Mods.WizenkleBoss.WorldGen.Observatory");

            int x = Main.maxTilesX;
            int xIslandGen;
            int yIslandGen;

            Rectangle potentialArea;

            do
            {
                xIslandGen = WorldGen.genRand.Next((int)(x * 0.1), (int)(x * 0.9));
                yIslandGen = WorldGen.genRand.Next(95, 126);
                yIslandGen = Math.Min(yIslandGen, (int)GenVars.worldSurfaceLow - 40);

                int checkAreaX = 70;
                int checkAreaY = 60;
                potentialArea = Utils.CenteredRectangle(new Vector2(xIslandGen, yIslandGen), new Vector2(checkAreaX, checkAreaY));
            }
            while (!ValidSkyPlacementArea(potentialArea));

            int tileXLookup = xIslandGen;
            if (xIslandGen < Main.maxTilesX / 2)
            {
                while (Main.tile[tileXLookup, yIslandGen].HasTile)
                    tileXLookup++;
            }
            else
            {
                while (Main.tile[tileXLookup, yIslandGen].HasTile)
                    tileXLookup--;
            }

            xIslandGen = tileXLookup;

            Generator.GenerateStructure("Structures/Observatory", new Point16(xIslandGen - 30, yIslandGen), ModContent.GetInstance<WizenkleBoss>());
        }
        private static bool ValidSkyPlacementArea(Rectangle area)
        {
            for (int i = area.Left; i < area.Right; i++)
            {
                for (int j = area.Top; j < area.Bottom; j++)
                {
                    if (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud || Main.tile[i, j].TileType == TileID.Sunplate)
                        return false;
                }
            }
            return true;
        }
    }
}
