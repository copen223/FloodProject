using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardEffect_Repulse:CardEffect
    {
        private int repulse_count;
        public CardEffect_Repulse(int _repulseCount)
        {
            trigger = "攻击";
            name = "击退";
            functionTarget = FunctionTarget.target;
            repulse_count = _repulseCount;
            effectTags_list.Add(EffectTag.接触);
            priority = 2;
        }

        public override void DoEffect(Combat combat)
        {
            
            if(isAtker)
            {
                // 攻击方使用这张卡
                bool toRight = combat.actor_dfd.WorldPos.x >= combat.actor_atk.WorldPos.x ? true : false;
                if(toRight)
                {
                    combat.move_dfd += new UnityEngine.Vector2Int(1, 0) * repulse_count;
                }
                else
                {
                    combat.move_dfd += new UnityEngine.Vector2Int(-1, 0) * repulse_count;
                }
            }
            else
            {
                // 防御方使用这张卡
                bool toRight = combat.actor_atk.WorldPos.x >= combat.actor_dfd.WorldPos.x ? true : false;
                if (toRight)
                {
                    combat.move_atk += new UnityEngine.Vector2Int(1, 0) * repulse_count;
                }
                else
                {
                    combat.move_atk += new UnityEngine.Vector2Int(-1, 0) * repulse_count;
                }
            }
        }
        public override string GetDescription()
        {
            return "击退" + repulse_count + "; ";
        }


    }
}
