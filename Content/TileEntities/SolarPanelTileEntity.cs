using Microsoft.Xna.Framework;
using SteelSeries.GameSense;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WizenkleBoss.Content.Tiles;

namespace WizenkleBoss.Content.TileEntities
{
    public class SolarPanelTileEntity : PowerSourceTileEntity
    {
        internal override int Width => 4;
        internal override int Height => 3;
        internal override int TileType => ModContent.TileType<SolarPanelTile>();

        public override void Update()
        {
            if (Position.Y >= Main.worldSurface)
                return;
            if ((int)(Main.GlobalTimeWrappedHourly * 60) % 520 != 0)
                return;

            float sunBrightness;

            if (Main.dayTime)
            {
                if (Main.time < 6700f)
                {
                    float interpolator = (float)Main.time / 6700f;
                    sunBrightness = interpolator;
                }
                else
                {
                    sunBrightness = 1;
                    if (Main.time > 48000f)
                    {
                        float interpolator = ((float)Main.time - 48000f) / (54000f - 48000f);
                        sunBrightness = 1 - interpolator;
                    }
                }
            }
            else
                sunBrightness = 0;

            int Amount = (int)(sunBrightness * 5);

            Charge = (int)MathHelper.Clamp(Charge + Amount, 0, 500);

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }
    }
}
