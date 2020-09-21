using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardEffect_DrawCard:CardEffect
    {
        private int draw_num;
        public CardEffect_DrawCard(string _trigger,int _draw_num)
        {
            trigger = _trigger;
            name = "抽卡";
            functionTarget = FunctionTarget.self;
            effectType = EffectType.afterCombat;
            draw_num = _draw_num;
            priority = 6;
        }

        public override void DoEffect(Combat combat)
        {
            
            if(isAtker)
            {
                combat.actor_atk.DrawCard(draw_num);
            }
            else
            {
                // 防御方使用这张卡
                combat.actor_dfd.DrawCard(draw_num);
            }
        }
        public override string GetDescription()
        {
            return "抽" + draw_num + "张卡; ";
        }


    }
}
