using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardEffect_PowerUp : CardEffect
    {
        private int powerUp_num;

        public CardEffect_PowerUp(string _trigger,int _up_num)
        {
            trigger = _trigger;
            name = "增强";
            functionTarget = FunctionTarget.self;
            effectType = EffectType.afterCombat;
            powerUp_num = _up_num;
            priority = 6;
        }

        public override void DoEffect(Combat combat)
        {
            ActorMono target;
            if(isAtker)
            {
                target = combat.actor_atk;
            }
            else
            {
                // 防御方使用这张卡
                target = combat.actor_dfd;
            }

            
            target.atk_addValue += powerUp_num;
            UIManager.instance.UpdateHandUI(BattleManager.instance.actor_curTurn.handPile.cards_list);
        }
        public override string GetDescription()
        {
            return "增强"  + powerUp_num + "点; ";
        }


    }
}
