using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{

    public class Combat
    {
        public ActorMono actor_atk;
        public ActorMono actor_dfd;
        public Card card_atk;
        public Card card_dfd;

        // 标记效果
        public List<SignEffect> effects_sign_list = new List<SignEffect>();
        // 卡牌效果
        public List<CardEffect> effects_card_list = new List<CardEffect>();
        // 总效果
        public List<Effect> effects_list = new List<Effect>();

        // 防御方情况
        public List<string> dfd_actions_list = new List<string>();

        // 结果缓存
        public float beDamaged_atk = 0;
        public float beDamaged_dfd = 0;
        public float beDamaged1_atk = 0;
        public float beDamaged1_dfd = 0;
        public float beDamaged2_atk = 0;
        public float beDamaged2_dfd = 0;
        public List<string> animationInfo_list = new List<string>();
        public bool isEnd = false;

        public Combat(ActorMono user, Card atkCard, ActorMono target)
        {
            card_atk = atkCard;
            actor_dfd = target;
            card_dfd = target.FocusedCard;
            actor_atk = user;
        }

        public void StartThisCombat()
        {
            // 对两个标记进行对抗，添加对应的标记效果
            Combat2Sign(card_atk.sign_up, card_dfd.sign_up);
            // 按优先级先后处理标记效果
            DoEffects();
            beDamaged1_atk = beDamaged_atk;
            beDamaged1_dfd = beDamaged_dfd;
            beDamaged_atk = beDamaged_dfd = 0;

            Combat2Sign(card_atk.sign_down, card_dfd.sign_down);
            DoEffects();

            beDamaged2_atk = beDamaged_atk;
            beDamaged2_dfd = beDamaged_dfd;
            
            // 完成一次combat的计算
            // combat 记录了战斗的双方，触发的反应，以及两次对抗分别产生的数值影响
        }

        //private void Combat2Card()
        //{
        //    Combat2Sign(card_atk.sign_up, card_dfd.sign_up);
        //    Combat2Sign(card_atk.sign_down, card_dfd.sign_down);
        //}

        private void Combat2Sign(CardSign atk, CardSign dfd)
        {
            if (atk.type != CardSign.Type.atk)
            {
                dfd_actions_list.Add("无事");
                return;
            }
            // 此时为攻击情况


            // 添加攻击标记效果
            atk.effect.isAtker = true;
            effects_sign_list.Add(atk.effect);

            // 添加攻击方卡牌由 攻击 触发的效果
            for (int i = 0; i < card_atk.effects_list.Count; i++)
            {
                var effect = card_atk.effects_list[i];

                if (effect.trigger == "攻击")
                {
                    effect.isAtker = true;
                    effects_card_list.Add(effect);
                }
            }

            if (atk.intensity <= dfd.intensity)
            {
                // 此时为防御情况

                // 添加防御标记的效果
                dfd.effect.isAtker = false;
                effects_sign_list.Add(dfd.effect);

                // 防御方做出反应
                dfd_actions_list.Add(dfd.effect.name);

                // 添加防御方卡牌由 防御 触发的效果
                for (int i = 0; i < card_dfd.effects_list.Count; i++)
                {
                    var effect = card_dfd.effects_list[i];

                    if (effect.trigger == "防御")
                    {
                        effect.isAtker = false;
                        effects_card_list.Add(effect);
                    }
                }

                isEnd = true;
            }
            else
            {
                // 防御方受伤
                dfd_actions_list.Add("受伤");
            }
        }

        private void DoEffects()
        {
            EffectSortByPriority sort = new EffectSortByPriority();

            //effects_sign_list.Sort(sort);
            //effects_card_list.Sort(sort);
            effects_list.AddRange(effects_sign_list);
            effects_list.AddRange(effects_card_list);
            effects_list.Sort(sort);

            for (int i = 0; i < effects_list.Count; i++)
            {
                effects_list[i].DoEffect(this);
            }

            effects_list.Clear();
            effects_sign_list.Clear();
            effects_card_list.Clear();
        }

        private void DoResult()
        {
            Debug.LogWarning("things");
            foreach (var thing in dfd_actions_list)
            {
                Debug.Log(thing);
            }

            // 伤害结算
            if (beDamaged_atk < 0)
                beDamaged_atk = 0;
            if (beDamaged_dfd < 0)
                beDamaged_dfd = 0;

            actor_atk.Behit(beDamaged_atk);
            actor_dfd.Behit(beDamaged_dfd);
        }

        private class EffectSortByPriority : IComparer<Effect>
        {
            public int Compare(Effect x, Effect y)
            {
                // return 1 就代表交换两个元素的位置，所以要想从大到小，则左小于大就返回1
                if (x.priority > y.priority)
                    return -1;
                else if (x.priority == y.priority)
                    return 0;
                else
                    return 1;
            }
        }
    }

    public class CombatAction
    {
        public enum ActionType
        {
            atk,
            dodge,
            block,
            parry
        }

        public ActionType action;
        public ActorMono actor;

        public CombatAction(ActionType _action, ActorMono _actor)
        {
            action = _action;
            actor = _actor;
        }
    }

}
