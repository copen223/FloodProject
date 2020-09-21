using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardEffect_GetCard:CardEffect
    {
        private string card_willGet;
        private CardPile.PileType pile;
        public CardEffect_GetCard(string _trigger,string _card_willGet,CardPile.PileType _pile)
        {
            trigger = _trigger;
            name = "获卡";
            functionTarget = FunctionTarget.self;
            effectType = EffectType.afterCombat;
            card_willGet = _card_willGet;
            pile = _pile;
            priority = 6;
        }

        public override void DoEffect(Combat combat)
        {
            
            if(isAtker)
            {
                // 攻击方使用这张卡
                Card card = CardManager.instance.GetCardByName(card_willGet);
                card.holder = combat.actor_atk;
                combat.actor_atk.AddNewCardTo(pile,card);
            }
            else
            {
                // 防御方使用这张卡
                Card card = CardManager.instance.GetCardByName(card_willGet);
                card.holder = combat.actor_dfd;
                combat.actor_dfd.AddNewCardTo(pile, card);
            }
        }
        public override string GetDescription()
        {
            return "获得" + card_willGet + "; " +"（在什么地方太麻烦了不写了）";
        }


    }
}
