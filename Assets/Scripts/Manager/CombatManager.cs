using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Struct;

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

    public IEnumerator ActionShow(Combat combat)
    {
        isCombating = true;
        bool isDead = false;

        GameManager.instance.gameInputMode = GameManager.InputMode.animation;

        float timer = 0;
        bool ifDfdAttack = false;

        // 攻击方攻击
        combat.actor_atk.StartDoAction("攻击",combat.actor_dfd.gameObject);
        
        while(timer < 0.3f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        timer = 0;

        // 防御方做出反应1
        List<string> dfd_actions_list = combat.dfd_actions_list;
        if (dfd_actions_list.Count >= 1)
        {
            string action = dfd_actions_list[0];
            if(action == "受伤")
            {
                combat.actor_dfd.Behit(combat.beDamaged1_dfd);
            }
            if(action == "格挡")
            {
                combat.actor_dfd.Behit(combat.beDamaged1_dfd);
                combat.actor_dfd.StartDoAction("格挡", combat.actor_atk.gameObject);
            }
            if(action == "闪避")
            {
                combat.actor_dfd.StartDoAction("闪避", combat.actor_atk.gameObject);
            }
            if(action == "攻击")
            {
                combat.actor_dfd.Behit(combat.beDamaged1_dfd);
                combat.actor_dfd.StartDoAction("受伤", combat.actor_atk.gameObject);
                ifDfdAttack = true;
            }

            if(combat.actor_dfd.battleState == ActorMono.BattleState.death)
            {
                isDead = true;
            }
        }


        // 等待
        while (timer < 0.3f && !isDead)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        timer = 0;

        // 防御方做出反应2
        if (dfd_actions_list.Count >= 2 && !isDead)
        {
            string action = dfd_actions_list[1];
            if (action == "受伤")
            {
                combat.actor_dfd.Behit(combat.beDamaged2_dfd);
            }
            if (action == "格挡")
            {
                combat.actor_dfd.Behit(combat.beDamaged2_dfd);
                combat.actor_dfd.StartDoAction("格挡", combat.actor_atk.gameObject);
            }
            if (action == "闪避")
            {
                combat.actor_dfd.StartDoAction("闪避", combat.actor_atk.gameObject);
            }
            if (action == "攻击")
            {
                combat.actor_dfd.Behit(combat.beDamaged2_dfd);
                ifDfdAttack = true;
            }
        }

        // 等待
        while (!combat.actor_atk.ifActionEnd || !combat.actor_dfd.ifActionEnd)
        {
            yield return new WaitForEndOfFrame();
        }

        // 防御方反击
        if(ifDfdAttack && !isDead)
        {
            combat.actor_dfd.StartDoAction("反击", combat.actor_atk.gameObject);

            // 攻击时间
            while (timer<0.3f)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            combat.actor_atk.Behit(combat.beDamaged1_atk + combat.beDamaged2_atk);
        }

        while (!combat.actor_atk.ifActionEnd || !combat.actor_dfd.ifActionEnd)
        {
            yield return new WaitForEndOfFrame();
        }


        isCombating = false;

        GameManager.instance.gameInputMode = GameManager.InputMode.play;
    }
}
