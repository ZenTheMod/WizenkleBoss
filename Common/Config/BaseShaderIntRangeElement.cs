using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using WizenkleBoss.Common.Ink;

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
                List[Index] = value;
            else if (MemberInfo.CanWrite)
                MemberInfo.SetValue(Item, value);
        }

        protected override object GetObject()
        {
            object value = base.GetObject();

            modifying = (int)value;

            return value;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

                // FieldInfo rightLockInfo = typeof(RangeElement).GetField("rightLock", BindingFlags.NonPublic | BindingFlags.Static);

                // RangeElement rightLock = (RangeElement)rightLockInfo.GetValue(null);
            bool inBar = rightLock == this;
            InkSystem.ConfigInk |= inBar;
        }
    }
}
