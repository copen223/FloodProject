using Assets.Scripts.Struct;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private ActorMono actor;

    // 预设的发动卡
    public List<CardInfo> cardInfo_cast_atk_list = new List<CardInfo>();
    public List<CardInfo> cardInfo_cast_defend_list = new List<CardInfo>();

    public List<Card> card_cast_list = new List<Card>();
    public List<Card> card_intention_cast_list = new List<Card>();

    // 预设的专注卡
    public List<CardInfo> cardInfo_focus_atk_list = new List<CardInfo>();
    public List<CardInfo> cardInfo_focus_defend_list = new List<CardInfo>();


    // 意图
    public enum Intention
    {
        attack,
        dfend,
        attack_defend
    }

    private Intention intention = Intention.attack;

    // 对象
    public GameObject target;

    public int action_count_min;
    public int action_count_max;
    private int action_count = 3;

    private void Start()
    {
        actor = gameObject.GetComponent<ActorMono>();
        InitCastCards();
    }

    private void InitCastCards()
    {
        foreach(var info in cardInfo_cast_atk_list)
        {
            Card card = CardManager.instance.GetCardByInfo(info);
            card_cast_list.Add(card);
        }

        UpdateCastCardsList();
    }

    private void UpdateCastCardsList()
    {
        card_intention_cast_list.Clear();

        for(int i=0;i<3;i++)
        {
            int index = Random.Range(0, card_cast_list.Count);
            card_intention_cast_list.Add(card_cast_list[index]);
        }
    }


    public void TurnStart()
    {
        // 决定行动次数
        action_count = Random.Range(action_count_min, action_count_max);
        UpdateCastCardsList();
        StartCoroutine(TurnGoOn());
    }

    IEnumerator TurnGoOn()
    {
        
        // 确定行动
        int index = 0;

        for (int i =0;i<action_count;i++)
        {
            // 一次攻击
            // 确定要发动的卡牌

            Card card = card_intention_cast_list[index];
            index++;
            if(index >= card_intention_cast_list.Count)
            {
                index = 0;
            }

            // 是否需要移动
            Vector2 dirWithDis = target.GetComponent<ActorMono>().WorldPos - actor.WorldPos;
            if(Mathf.Abs(dirWithDis.x) > card.cast_extent_x || Mathf.Abs(dirWithDis.y) > card.cast_extent_y)
            {
                // 确定路径
                var path_list = PathFinderManager.instance.SearchPathTo(actor.WorldPos, target.GetComponent<ActorMono>().WorldPos);
                path_list.RemoveAt(path_list.Count - 1);

                actor.StartMoveByList(path_list);

                while(actor.IsMoving)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            // 距离不够 结束回合
            dirWithDis = target.GetComponent<ActorMono>().WorldPos - actor.WorldPos;
            if (Mathf.Abs(dirWithDis.x) > card.cast_extent_x || Mathf.Abs(dirWithDis.y) > card.cast_extent_y)
            {
                break;
            }

            // 移动完毕 施放卡牌

            Timer timer = new Timer();

            // 动画
            Animator animator = GetComponent<Animator>();
            float toRight = target.transform.position.x > transform.position.x ? 1 : -1;
            animator.SetInteger("ToAttack", 1);
            animator.SetFloat("Blend", toRight);
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            float aniTime = info.length;
            float aniTimer = 0f;

            while (aniTimer < aniTime)
            {
                aniTimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("ToAttack", -1);

            CombatManager.instance.StartCombat(actor, card, target.GetComponent<ActorMono>());

            yield return new WaitForSeconds(0.5f);
        }

        BattleManager.instance.OnTurnEnd();

        
    }
}
