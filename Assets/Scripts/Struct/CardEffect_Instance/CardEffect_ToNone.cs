using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardEffect_ToNone:CardEffect
    {
        public CardEffect_ToNone()
        {
            trigger = "无";
            name = "消耗";
            functionTarget = FunctionTarget.self;
            effectType = EffectType.sign;
            priority = 6;
        }

        public override void DoEffect(Combat combat)
        {
            
            if(isAtker)
            {
                
            }
            else
            {
                
            }
        }
        public override string GetDescription()
        {
            return "消耗;";
        }


    }
}
