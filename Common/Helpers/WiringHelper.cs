using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using WizenkleBoss.Content.TileEntities;
using ReLogic.Content;
using System.ComponentModel;
using Terraria.ID;

namespace WizenkleBoss.Common.Helpers
{
    public static partial class Helper
    {

            // ALL CREDIT OF THIS GOES TO JUNE, IM SO ASS AT GOOD CODE ITS NOT EVEN FUNNY.

            // go checkout techaria if you havent already.

        private static Point16[] Directions => [new(-1, 0), new(1, 0), new(0, -1), new(0, 1)];
        public static bool HasWire(byte wire, Tile tile) => wire switch
        {
            0 => tile.RedWire,
            1 => tile.BlueWire,
            2 => tile.GreenWire,
            3 => tile.YellowWire,
            _ => throw new ArgumentOutOfRangeException(nameof(wire), wire, null)
        };

        public static void TravelToPoint(byte wire, Point16 point, Queue<Point16> frontier, HashSet<Point16> visited)
        {
            if (visited.Contains(point) || !HasWire(wire, Main.tile[point])) return;
            frontier.Enqueue(point);
            visited.Add(point);
        }

        public static bool WireScanForTileType(int x, int y, int width, int height, int type, out Point16? tile)
        {
            bool found = WireScanForTileType(0, x, y, width, height, type, out tile) ||
                WireScanForTileType(1, x, y, width, height, type, out tile) ||
                WireScanForTileType(2, x, y, width, height, type, out tile) ||
                WireScanForTileType(3, x, y, width, height, type, out tile);
            return found;
        }

        public static bool WireScanForTileType(byte wire, int x, int y, int width, int height, int type, out Point16? tile)
        {
                // MAD GENIUS BULLSHIT
            Queue<Point16> frontier = new();
            HashSet<Point16> visited = new();

            for (int j = y; j < y + height; j++)
            {
                for (int i = x; i < x + width; i++)
                {
                    TravelToPoint(wire, new Point16(i, j), frontier, visited);
                }
            }

            while (frontier.Count > 0)
            {
                var point = frontier.Dequeue();
                for (int i = 0; i < 4; i++)
                {
                    var next = point + Directions[i];
                    TravelToPoint(wire, next, frontier, visited);
                }
            }

            for (int j = y; j < y + height; j++)
            {
                for (int i = x; i < x + width; i++)
                {
                    visited.Remove(new Point16(i, j));
                }
            }

            bool found = false;
            tile = null;

            foreach (var point in visited)
            {
                Tile tl = Main.tile[point.X, point.Y];
                if (tl != null && tl.TileType == type)
                {
                    tile = point;
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static int WireScanAndConsumePower(int x, int y, int width, int height, int amountToConsume)
        {
            var set = WireScanForPower(0, x, y, width, height);
                set.UnionWith(WireScanForPower(1, x, y, width, height));
                set.UnionWith(WireScanForPower(2, x, y, width, height));
                set.UnionWith(WireScanForPower(3, x, y, width, height));

            int _amountToConsume = amountToConsume;
            foreach (var tile in set)
            {
                if (_amountToConsume <= 0)
                    break;
                if (tile.Charge > 0)
                {
                    int consume = Math.Min(_amountToConsume, tile.Charge);
                    tile.Charge -= consume;
                    _amountToConsume -= consume;

                        // lag
                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendData(MessageID.TileEntitySharing, number: tile.ID, number2: tile.Position.X, number3: tile.Position.Y);
                }
            }

            return amountToConsume - _amountToConsume;
        }

        public static HashSet<PowerSourceTileEntity> WireScanForPower(byte wire, int x, int y, int width, int height)
        {
                // MAD GENIUS BULLSHIT
            Queue<Point16> frontier = new();
            HashSet<Point16> visited = new();

            for (int j = y; j < y + height; j++)
            {
                for (int i = x; i < x + width; i++)
                {
                    TravelToPoint(wire, new Point16(i, j), frontier, visited);
                }
            }

            while (frontier.Count > 0)
            {
                var point = frontier.Dequeue();
                for (int i = 0; i < 4; i++)
                {
                    var next = point + Directions[i];
                    TravelToPoint(wire, next, frontier, visited);
                }
            }

            for (int j = y; j < y + height; j++)
            {
                for (int i = x; i < x + width; i++)
                {
                    visited.Remove(new Point16(i, j));
                }
            }

            HashSet<PowerSourceTileEntity> tileEntity = new();

            foreach (var point in visited)
            {
                Point16 tl = Helper.GetTopLeftTileInMultitile(point.X, point.Y);
                TileEntity.ByPosition.TryGetValue(tl, out TileEntity te);

                if (te is PowerSourceTileEntity powerSource)
                {
                    tileEntity.Add(powerSource);
                }
            }
            return tileEntity;
        }
    }
}
