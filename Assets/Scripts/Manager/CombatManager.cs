using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Struct;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;
    public bool isCombating = false;


    private void Awake()
    {
        instance = this;
    }

    public void StartCombat(ActorMono user,Card usedCard,ActorMono target)
    {
        Combat combat = new Combat(user, usedCard, target);
        // 计算combat
        combat.StartThisCombat();
        // 应用计算结果并演出
        StartCoroutine(ActionShow(combat));
    }

    //public IEnumerator ActionShow1(Combat combat)
    //{
    //    isCombating = true;
    //    bool isDead = false;
    //    bool ifAtk = false;
    //    bool ifDfdAttack = false;

    //    float timer = 0;

    //    GameManager.instance.gameInputMode = GameManager.InputMode.animation;

    //    List<CombatAction> dfd_combatActions_list = combat.dfd_actions_list;
    //    List<CombatAction> atk_combatActions_list = combat.atk_actions_list;
    //    int actionIndex = 0;

    //    // 攻击方攻击
    //    foreach(var action in atk_combatActions_list)
    //    {
    //        if (action.IfContain("攻击"))
    //            ifAtk = true;
    //    }
    //    if(!ifAtk)
    //    {
    //        isCombating = false;
    //        StopCoroutine(ActionShow(combat));
    //    }

    //    combat.actor_atk.StartDoAction("攻击",combat.actor_dfd.gameObject);
        
    //    while(timer < 0.3f)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        timer += Time.deltaTime;
    //    }
    //    timer = 0;

    //    // 防御方做出反应1
    //    List<string> dfd_actions_list = combat.dfd_actions_list;
    //    if (dfd_actions_list.Count >= 1)
    //    {
    //        string action = dfd_actions_list[0];
    //        if(action == "受伤")
    //        {
    //            combat.actor_dfd.Behit(combat.beDamaged1_dfd);
    //        }
    //        if(action == "格挡")
    //        {
    //            combat.actor_dfd.Behit(combat.beDamaged1_dfd);
    //            combat.actor_dfd.StartDoAction("格挡", combat.actor_atk.gameObject);
    //        }
    //        if(action == "闪避")
    //        {
    //            combat.actor_dfd.StartDoAction("闪避", combat.actor_atk.gameObject);
    //        }
    //        if(action == "攻击")
    //        {
    //            combat.actor_dfd.Behit(combat.beDamaged1_dfd);
    //            combat.actor_dfd.StartDoAction("受伤", combat.actor_atk.gameObject);
    //            ifDfdAttack = true;
    //        }

    //        if(combat.actor_dfd.battleState == ActorMono.BattleState.death)
    //        {
    //            isDead = true;
    //        }
    //    }


    //    // 等待
    //    while (timer < 0.3f && !isDead)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        timer += Time.deltaTime;
    //    }
    //    timer = 0;

    //    // 防御方做出反应2
    //    if (dfd_actions_list.Count >= 2 && !isDead)
    //    {
    //        string action = dfd_actions_list[1];
    //        if (action == "受伤")
    //        {
    //            combat.actor_dfd.Behit(combat.beDamaged2_dfd);
    //        }
    //        if (action == "格挡")
    //        {
    //            combat.actor_dfd.Behit(combat.beDamaged2_dfd);
    //            combat.actor_dfd.StartDoAction("格挡", combat.actor_atk.gameObject);
    //        }
    //        if (action == "闪避")
    //        {
    //            combat.actor_dfd.StartDoAction("闪避", combat.actor_atk.gameObject);
    //        }
    //        if (action == "攻击")
    //        {
    //            combat.actor_dfd.Behit(combat.beDamaged2_dfd);
    //            ifDfdAttack = true;
    //        }
    //    }

    //    // 等待
    //    while (!combat.actor_atk.ifActionEnd || !combat.actor_dfd.ifActionEnd)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }

    //    // 防御方反击
    //    if(ifDfdAttack && !isDead)
    //    {
    //        combat.actor_dfd.StartDoAction("反击", combat.actor_atk.gameObject);

    //        // 攻击时间
    //        while (timer<0.3f)
    //        {
    //            timer += Time.deltaTime;
    //            yield return new WaitForEndOfFrame();
    //        }

    //        combat.actor_atk.Behit(combat.beDamaged1_atk + combat.beDamaged2_atk);
    //    }

    //    while (!combat.actor_atk.ifActionEnd || !combat.actor_dfd.ifActionEnd)
    //    {
    //        yield return new WaitForEndOfFrame();
    //    }


    //    isCombating = false;

    //    GameManager.instance.gameInputMode = GameManager.InputMode.play;
    //}

    public IEnumerator ActionShow(Combat combat)
    {
        isCombating = true;
        bool isDead = false;
        bool ifAtk = false;
        bool ifDfdAttack = false;

        List<CombatAction> dfd_combatActions_list = combat.dfd_actions_list;
        List<CombatAction> atk_combatActions_list = combat.atk_actions_list;

        #region 攻击演出

        foreach (var action in atk_combatActions_list)
        {
            if (action.IfContain("攻击"))
                ifAtk = true;
        }
        if (!ifAtk)
        {
            isCombating = false;
            StopCoroutine(ActionShow(combat));
        }
        combat.actor_atk.StartDoAction("攻击", combat,true);
        yield return new WaitForSeconds(0.3f);

        #endregion

        int i = 0;

        for (;i<2;i++)
        {
            CombatAction dfd_combatAction = dfd_combatActions_list[i];
            CombatAction atk_combatAction = atk_combatActions_list[i];

            #region 防御方标记反应 应用和演出
            string action = dfd_combatAction.actions_list[dfd_combatAction.actions_list.Count - 1];

            float damage = i == 0 ? combat.beDamaged1_dfd : combat.beDamaged2_dfd;

            if (action == "受伤")
            {
                combat.actor_dfd.Behit(damage);
            }
            if (action == "格挡")
            {
                combat.actor_dfd.Behit(damage);
                combat.actor_dfd.StartDoAction("格挡", combat,false);
            }
            if (action == "闪避")
            {
                combat.actor_dfd.StartDoAction("闪避", combat,false);
            }
            if (action == "攻击")
            {
                combat.actor_dfd.Behit(damage);
                combat.actor_dfd.StartDoAction("受伤", combat,false);
                ifDfdAttack = true;

                if(atk_combatAction.IfContain("击退"))
                {
                    Vector2Int move = i == 0 ? combat.move1_atk : combat.move2_atk;
                    combat.actor_atk.StartForceMoveByDirWithDis(move);
                    
                    if (combat.actor_atk.IsMoving)
                        yield return new WaitForEndOfFrame();

                    isCombating = false;
                    StopCoroutine(ActionShow(combat));
                }
            }


            if (combat.actor_dfd.battleState == ActorMono.BattleState.death)
            {
                isDead = true;
            }
            #endregion

            #region 特殊效果应用与演出

            if(dfd_combatAction.IfContain("击退"))
            {
                if (!combat.actor_dfd.ifActionEnd)
                {
                    yield return new WaitForEndOfFrame();
                }

                Vector2Int move = i == 0 ? combat.move1_dfd : combat.move2_dfd;
                combat.actor_dfd.StartForceMoveByDirWithDis(move);

                if (combat.actor_atk.IsMoving)
                    yield return new WaitForEndOfFrame();
            }

            #endregion

            yield return new WaitForSeconds(0.3f);
        }

        if(!combat.actor_atk.ifActionEnd || !combat.actor_dfd.ifActionEnd)
        {
            yield return new WaitForEndOfFrame();
        }

        #region 反击演出

        if (ifDfdAttack && !isDead && combat.card_dfd.IfCanCast(combat.actor_atk))
        {
            combat.actor_dfd.StartDoAction("反击", combat, false);

            yield return new WaitForSeconds(0.3f);

            combat.actor_atk.Behit(combat.beDamaged1_atk + combat.beDamaged2_atk);

            if (!combat.actor_atk.ifActionEnd || !combat.actor_dfd.ifActionEnd)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(0.5f);
        }

        #endregion

        isCombating = false;
    }


}
