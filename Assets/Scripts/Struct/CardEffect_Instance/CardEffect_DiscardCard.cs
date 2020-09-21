using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardEffect_DiscardCard : CardEffect
    {
        private int discard_num;
        public CardEffect_DiscardCard(string _trigger,int _discard_num)
        {
            trigger = _trigger;
            name = "弃卡";
            functionTarget = FunctionTarget.self;
            effectType = EffectType.afterCombat;
            discard_num = _discard_num;
            priority = 6;
        }

        public override void DoEffect(Combat combat)
        {
            
            if(isAtker)
            {
                combat.actor_atk.DiscardCard(discard_num);
            }
            else
            {
                // 防御方使用这张卡
                combat.actor_atk.DiscardCard(discard_num);
            }
        }
        public override string GetDescription()
        {
            return "弃" + discard_num + "张卡; ";
        }


    }
}
