using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Struct;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

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
        GameManager.instance.gameInputMode = GameManager.InputMode.animation;

        float timer = 0;
        bool ifDfdAttack = false;

        // 攻击方攻击0.6f
        combat.actor_atk.StartDoAction("攻击",combat.actor_dfd.gameObject);
        
        while(timer <= 0.6f)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
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
                combat.actor_dfd.StartDoAction("受伤", combat.actor_atk.gameObject);
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
        }


        // 等待0.4s
        while (timer <= 0.4f)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timer = 0;

        // 防御方做出反应2
        if (dfd_actions_list.Count >= 2)
        {
            string action = dfd_actions_list[1];
            if (action == "受伤")
            {
                combat.actor_dfd.Behit(combat.beDamaged2_dfd);
                combat.actor_dfd.StartDoAction("受伤",combat.actor_atk.gameObject);
            }
            if (action == "闪避")
            {
                combat.actor_dfd.StartDoAction("闪避", combat.actor_atk.gameObject);
            }
            if (action == "攻击")
            {
                combat.actor_dfd.Behit(combat.beDamaged2_dfd);
                combat.actor_dfd.StartDoAction("受伤", combat.actor_atk.gameObject);
                ifDfdAttack = true;
            }
        }

        // 等待0.4s
        while (timer <= 0.4f)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timer = 0;

        if(ifDfdAttack)
        {
            combat.actor_dfd.StartDoAction("攻击", combat.actor_atk.gameObject);
            combat.actor_atk.StartDoAction("受伤", combat.actor_dfd.gameObject);
            combat.actor_atk.Behit(combat.beDamaged1_atk + combat.beDamaged2_atk);
        }

        while (timer <= 0.4f && ifDfdAttack)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        GameManager.instance.gameInputMode = GameManager.InputMode.play;
    }
}
