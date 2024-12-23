using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Achievements;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using System.Collections;
using Terraria.ModLoader.IO;
using Terraria.UI.Gamepad;
using Terraria.Utilities;
using WizenkleBoss.Assets.Helper;
using WizenkleBoss.Assets.Textures;
using WizenkleBoss.Assets.Config;
using WizenkleBoss.Content.Projectiles.Misc;
using WizenkleBoss.Common.Packets;

namespace WizenkleBoss.Content.UI
{
    [Serializable]
    public enum SupernovaState
    {
        None,
        Shrinking,
        Expanding,
        Complete
    }
    [Serializable]
    public struct BarrierStar
    {
        public float BaseRotation;
        public float BaseSize;
        public float SupernovaSize;
        public Vector2 Position;
        public Color Color;
        public int Texture;
        public SupernovaState State;
        public string Name;
        public override bool Equals(object obj) => obj is BarrierStar star && Equals(star);
            // Bad idea probably :3
        public bool Equals(BarrierStar other)
        {
            return BaseRotation == other.BaseRotation &&
                BaseSize == other.BaseSize &&
                SupernovaSize == other.SupernovaSize &&
                Position == other.Position &&
                Color == other.Color &&
                Texture == other.Texture &&
                State == other.State &&
                Name == other.Name;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(BaseRotation, BaseSize, SupernovaSize, Position, Color, Texture, State, Name);
        }
        public static bool operator ==(BarrierStar lhs, BarrierStar rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(BarrierStar lhs, BarrierStar rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
    public class BarrierStarSystem : ModSystem
    {
        public static BarrierStar[] Stars;
        public static BarrierStar TheOneImportantThingInTheSky;
        public static Vector2[] OldMeteoritePosition = new Vector2[25];
        public override void OnWorldLoad()
        {
            if (Main.netMode == NetmodeID.SinglePlayer || Main.dedServ)
            {
                UnifiedRandom rand = new(Main.worldID);
                Stars = new BarrierStar[rand.Next(200, 400)];

                    // bad idea
                for (int i = 0; i <= Stars.Length - 1; i++)
                {
                    Stars[i] = new()
                    {
                        BaseRotation = rand.NextFloat(MathHelper.TwoPi),
                        BaseSize = rand.NextFloat(0.2f, 1f),
                        SupernovaSize = 1f,
                        Position = new(rand.NextFloat(-700, 700), rand.NextFloat(-700, 700)),
                        Color = Color.Lerp(Color.White, Color.Lerp(Color.PaleVioletRed, Color.Purple, rand.NextFloat()), rand.NextFloat()),
                        Texture = rand.Next(0, 5),
                        State = 0,
                        Name = Language.GetTextValue("Mods.WizenkleBoss.StarNames.Name" + rand.Next(235)) + " - " + rand.Next(100)
                    };
                }

                TheOneImportantThingInTheSky = new()
                {
                    BaseRotation = 0f,
                    BaseSize = 1f,
                    SupernovaSize = 1f,
                    Position = new(rand.NextFloat(-100, 100), rand.NextFloat(-100, 100)),
                    Color = Color.Black,
                    Texture = 0,
                    State = 0,
                    Name = Language.GetTextValue("Mods.WizenkleBoss.StarNames.ImportantStarName")
                };
            }
            else // EVEN WORSE IDEA (IF YOU ARE READING THIS DO NOT DO THIS)
            {
                    // Get star array for joining players.
                var requeststars = new RequestStars(Main.myPlayer);
                requeststars.Send();
            }
        }
        public override void PostUpdateEverything()
        {
                // null check :sob:
            if (Stars == null || Stars.Length == 0 || TheOneImportantThingInTheSky == default) 
                return;

            if (TheOneImportantThingInTheSky.State == SupernovaState.Expanding && BarrierTelescopeUISystem.telescopeTargetByRequest.IsReady)
            {
                for (int i = OldMeteoritePosition.Length - 2; i >= 0; i--)
                {
                    OldMeteoritePosition[i + 1] = OldMeteoritePosition[i];
                }
                Vector2 size = BarrierTelescopeUISystem.telescopeTargetByRequest.GetTarget().Size();
                OldMeteoritePosition[0] = Vector2.Lerp((size / 2f) + TheOneImportantThingInTheSky.Position, new Vector2(size.X / 2, size.Y * 3f), MathHelper.Clamp((TheOneImportantThingInTheSky.SupernovaSize * 4.7f) - 0.2f, 0, 2));
            }
            bool ImportantStarSupernova = TheOneImportantThingInTheSky.State > SupernovaState.None && TheOneImportantThingInTheSky.State != SupernovaState.Complete;
            if (!Stars.Where(s => s.State > SupernovaState.None && s.State != SupernovaState.Complete).Any() && !ImportantStarSupernova)
                return;
            for (int i = 0; i <= Stars.Length - 1; i++)
            {
                BarrierStar star = Stars[i];
                if (Stars[i].State == SupernovaState.None || Stars[i].State == SupernovaState.Complete)
                    continue;
                if (Stars[i].State == SupernovaState.Expanding && Stars[i].SupernovaSize <= 1)
                    Stars[i].SupernovaSize += 0.001f;
                else if (Stars[i].State == SupernovaState.Expanding && Stars[i].SupernovaSize > 1)
                    Stars[i].State = SupernovaState.Complete;
                if (Stars[i].State == SupernovaState.Shrinking)
                {
                    if (Stars[i].SupernovaSize >= 0.005)
                        Stars[i].SupernovaSize -= 0.005f;
                    else
                        Stars[i].State = SupernovaState.Expanding;
                }
            }
            if (ImportantStarSupernova)
            {
                if (TheOneImportantThingInTheSky.State == SupernovaState.Expanding && TheOneImportantThingInTheSky.SupernovaSize <= 1)
                    TheOneImportantThingInTheSky.SupernovaSize += 0.0006f;
                else if (TheOneImportantThingInTheSky.State == SupernovaState.Expanding && TheOneImportantThingInTheSky.SupernovaSize > 1)
                    TheOneImportantThingInTheSky.State = SupernovaState.Complete;
                if (TheOneImportantThingInTheSky.State == SupernovaState.Shrinking)
                {
                    if (TheOneImportantThingInTheSky.SupernovaSize >= 0.005)
                        TheOneImportantThingInTheSky.SupernovaSize -= 0.003f;
                    else
                        TheOneImportantThingInTheSky.State = SupernovaState.Expanding;
                }
            }
        }
    }
}
