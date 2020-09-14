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

        // 防御方反应 总是 为 无事 受伤 各类反应中的一项
        public List<CombatAction> dfd_actions_list = new List<CombatAction>();
        public List<CombatAction> atk_actions_list = new List<CombatAction>();


        // 结果缓存
        
        // 伤害相关
        public float beDamaged_atk = 0;
        public float beDamaged_dfd = 0;
        public float beDamaged1_atk = 0;
        public float beDamaged1_dfd = 0;
        public float beDamaged2_atk = 0;
        public float beDamaged2_dfd = 0;

        // 位置相关
        public Vector2Int move_atk = new Vector2Int(0, 0);
        public Vector2Int move_dfd = new Vector2Int(0, 0);
        public Vector2Int move1_atk = new Vector2Int(0, 0);
        public Vector2Int move2_atk = new Vector2Int(0, 0);
        public Vector2Int move1_dfd = new Vector2Int(0, 0);
        public Vector2Int move2_dfd = new Vector2Int(0, 0);


        // public List<string> animationInfo_list = new List<string>();
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
            // 对两个标记进行对抗，添加对应的标记效果以及事件列表
            Combat2Sign(card_atk.sign_up, card_dfd.sign_up);
            // 按优先级先后处理效果，改变combat的数据
            DoEffects();

            beDamaged1_atk = beDamaged_atk;
            beDamaged1_dfd = beDamaged_dfd;
            beDamaged_atk = beDamaged_dfd = 0;

            move1_atk = move_atk;
            move1_dfd = move_dfd;
            move_atk = move_dfd = new Vector2Int(0, 0);

            Combat2Sign(card_atk.sign_down, card_dfd.sign_down);
            DoEffects();

            beDamaged2_atk = beDamaged_atk;
            beDamaged2_dfd = beDamaged_dfd;

            move2_atk = move_atk;
            move2_dfd = move_dfd;



            // 完成一次combat的计算
            // combat 记录了战斗的双方，触发的反应，以及两次对抗分别产生的数值影响
        }

        //private void Combat2Card()
        //{
        //    Combat2Sign(card_atk.sign_up, card_dfd.sign_up);
        //    Combat2Sign(card_atk.sign_down, card_dfd.sign_down);
        //}

        // 这个方法将根据一个方位标记的对抗 填充 标记和卡牌效果列表，以及演出事件列表
        // 填充卡牌效果列表，双方列表分别增加一个CombatAction
        private void Combat2Sign(CardSign atk, CardSign dfd)
        {
            if (atk.type != CardSign.Type.atk)
            {
                // 攻击方不是攻击标记

                CombatAction action1 = new CombatAction();
                action1.AddAction("无事");
                dfd_actions_list.Add(action1);
                atk_actions_list.Add(action1);
                return;
            }

            // 攻击方是攻击标记
            atk.effect.isAtker = true;
            CombatAction action_atk = new CombatAction("攻击");

            CombatAction action_dfd = new CombatAction();
            // 添加攻击标记的效果
            effects_sign_list.Add(atk.effect);

            // 添加攻击方卡牌由 攻击 触发的效果
            for (int i = 0; i < card_atk.effects_list.Count; i++)
            {
                var effect = card_atk.effects_list[i];

                if (effect.trigger == "攻击")
                {
                    effect.isAtker = true;
                    effects_card_list.Add(effect);
                    if(effect.functionTarget == CardEffect.FunctionTarget.target)
                    {
                        action_dfd.AddAction(effect.name);
                    }
                }
            }

            // 分成两种情况
            if (atk.intensity <= dfd.intensity)
            {
                // 此时为防御情况

                // 添加防御标记的效果
                dfd.effect.isAtker = false;
                effects_sign_list.Add(dfd.effect);

                // 防御方做出反应
                action_dfd.AddAction(dfd.effect.name);

                // 添加防御方卡牌由 防御 触发的效果
                if (dfd.type == CardSign.Type.dfd)
                {
                    for (int i = 0; i < card_dfd.effects_list.Count; i++)
                    {
                        var effect = card_dfd.effects_list[i];

                        if (effect.trigger == "防御")
                        {
                            effect.isAtker = false;
                            effects_card_list.Add(effect);
                        }
                    }
                }
                // 防御方反击 添加防御方卡牌由 攻击 触发的效果
                else if(dfd.type == CardSign.Type.atk)
                {
                    for (int i = 0; i < card_dfd.effects_list.Count; i++)
                    {
                        var effect = card_dfd.effects_list[i];

                        if (effect.trigger == "攻击")
                        {
                            effect.isAtker = false;
                            effects_card_list.Add(effect);
                            if(effect.functionTarget == CardEffect.FunctionTarget.target)
                            {
                                action_atk.AddAction(effect.name);
                            }
                        }
                    }
                }

                isEnd = true;
            }
            else
            {
                // 防御方受伤
                action_dfd.AddAction("受伤");
                // dfd_actions_list.Add("受伤");
            }

            atk_actions_list.Add(action_atk);
            dfd_actions_list.Add(action_dfd);
            return;
        }

        // 这个方法不会直接影响攻击方或防御方，而是影响combat的各项数据
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
        public List<string> actions_list = new List<string>();

        public bool IfContain(string _action)
        {
            foreach(var action in actions_list)
            {
                if(action == _action)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddAction(string _action)
        {
            actions_list.Add(_action);
        }

        public CombatAction()
        {

        }

        public CombatAction(string _action)
        {
            AddAction(_action);
        }
    }

}
