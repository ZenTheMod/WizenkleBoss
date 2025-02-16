using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI.Chat;
using WizenkleBoss.Common.Ink;
using WizenkleBoss.Content.UI;

namespace WizenkleBoss.Common.Config
{
    public abstract class BaseShaderIntRangeElement : PrimitiveRangeElement<int>
    {
        public abstract ref int modifying { get; }

        public override int NumberTicks => (Max - Min) / Increment + 1;

        public override float TickIncrement => Increment / (float)(Max - Min);

        protected override float Proportion
        {
            get
            {
                return (GetValue() - Min) / (float)(Max - Min);
            }
            set
            {
                SetValue((int)Math.Round((value * (Max - Min) + Min) * (1f / Increment)) * Increment);
            }
        }

        public BaseShaderIntRangeElement()
        {
            Min = 0;
            Max = 100;
            Increment = 5;
        }

        protected override void SetObject(object value)
        {
            modifying = (int)value;

            if (List != null)
            {
                List[Index] = value;
                return;
            }

            if (!MemberInfo.CanWrite)
                return;

            MemberInfo.SetValue(Item, value);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

                // FieldInfo rightLockInfo = typeof(RangeElement).GetField("rightLock", BindingFlags.NonPublic | BindingFlags.Static);

                // RangeElement rightLock = (RangeElement)rightLockInfo.GetValue(null);
            bool inBar = rightLock == this;
            InkSystem.ConfigInk |= inBar;
        }
           
            // I do hate that I have to do this in a draw hook, rip highfpssupport players, you were never loved or missed <3
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
                    // Bassically just autosave. ( I was too lazy to make it revert the one in memory. )
                // FieldInfo rightLockInfo = typeof(RangeElement).GetField("rightLock", BindingFlags.NonPublic | BindingFlags.Static);

            RangeElement oldRightLock = rightLock;
            base.DrawSelf(spriteBatch);
            RangeElement newRightLock = rightLock;

            if (oldRightLock != newRightLock && newRightLock == null && oldRightLock is BaseShaderIntRangeElement)
            {
                Interface.modConfig.SaveConfig(null, null);
                    // var Interface = typeof(Mod).Assembly.GetType("Terraria.ModLoader.UI.Interface");
                    // object UIModConfigInstance = Interface.GetField("modConfig", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null); // SaveConfig

                    // var UIModConfig = typeof(Mod).Assembly.GetType("Terraria.ModLoader.Config.UI.UIModConfig");

                    // MethodInfo SaveConfig = UIModConfig.GetMethod("SaveConfig", BindingFlags.NonPublic | BindingFlags.Instance);
                    // SaveConfig.Invoke(UIModConfigInstance, [null, null]);
            }
        }
    }
}
