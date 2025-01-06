using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WizenkleBoss.Content.Tiles;

namespace WizenkleBoss.Content.TileEntities
{
    public abstract class PowerSourceTileEntity : ModTileEntity
    {
        internal virtual int Width => 1;
        internal virtual int Height => 1;
        internal virtual int TileType => 0;

        public int Charge = 0;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            return tile.HasTile && tile.TileType == TileType;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, Width, Height);

                NetMessage.SendData(MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
                return -1;
            }

            int placedEntity = Place(i, j);
            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.TileEntitySharing, number: ID, number2: Position.X, number3: Position.Y);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(Charge);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Charge = reader.Read7BitEncodedInt();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["charge"] = Charge;
        }

        public override void LoadData(TagCompound tag)
        {
            Charge = tag.GetInt("charge");
        }
    }
}
