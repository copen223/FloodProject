﻿using Assets.Scripts.Struct;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private ActorMono actor;

    //// 预设的发动卡
    //public List<CardInfo> cardInfo_cast_atk_list = new List<CardInfo>();
    //public List<CardInfo> cardInfo_cast_defend_list = new List<CardInfo>();

    //public List<Card> card_cast_list = new List<Card>();
    //public List<Card> card_intention_cast_list = new List<Card>();

    //// 预设的专注卡
    //public List<CardInfo> cardInfo_focus_atk_list = new List<CardInfo>();
    //public List<CardInfo> cardInfo_focus_defend_list = new List<CardInfo>();

    // 预设的发动卡
    public CardInfo castCard_info;

    // 预设的专注卡
    public CardInfo focusCard_defend_info;
    public CardInfo focusCard_attack_info;

    private Card castCard;
    private Card focusCard_defend;
    private Card focusCard_attack;


    // 意图
    public enum ActionMode
    {
        attack,
        dfend,
        attack_defend
    }

    private ActionMode actionMode = ActionMode.attack;

    // 对象
    public GameObject target;

    public int action_count_min;
    public int action_count_max;
    private int action_count = 3;

    private void Start()
    {
        actor = gameObject.GetComponent<ActorMono>();

        //InitCastCards();
        InitCards();
    }

    private void InitCards()
    {
        focusCard_attack = CardManager.instance.GetCardByInfo(focusCard_attack_info);
        focusCard_defend = CardManager.instance.GetCardByInfo(focusCard_defend_info);
        castCard = CardManager.instance.GetCardByInfo(castCard_info);
    }

    //private void InitCastCards()
    //{
    //    foreach(var info in cardInfo_cast_atk_list)
    //    {
    //        Card card = CardManager.instance.GetCardByInfo(info);
    //        card_cast_list.Add(card);
    //    }

    //    UpdateCastCardsList();
    //}

    //private void UpdateCastCardsList()
    //{
    //    card_intention_cast_list.Clear();

    //    for(int i=0;i<3;i++)
    //    {
    //        int index = Random.Range(0, card_cast_list.Count);
    //        card_intention_cast_list.Add(card_cast_list[index]);
    //    }
    //}


    public void TurnStart()
    {
        // 决定行动次数
        action_count = Random.Range(action_count_min, action_count_max);
        // 决定目标
        target = SearchTarget();
        StartCoroutine(TurnGoOn());
    }

    IEnumerator TurnGoOn()
    {

        if (target == null)
        {
            BattleManager.instance.OnTurnEnd();
            StopAllCoroutines();
        }

        // 确定行动
        if (actor.healPoint <= actor.healPoint_max * 0.4f)
            actionMode = ActionMode.dfend;
        else
        {
            int r = Random.Range(0, 2);
            if (r == 0)
                actionMode = ActionMode.attack;
            if (r == 1)
                actionMode = ActionMode.attack_defend;
        }

        bool ifFocus = (Random.Range(0, 100)) < (15f + (100f * (actor.healPoint / actor.healPoint_max))) ? true : false;

        if (ifFocus)
        {
            if (actionMode == ActionMode.attack)
            {
                actor.FocusCard(focusCard_attack);
            }

            if (actionMode == ActionMode.attack_defend || actionMode == ActionMode.dfend)
            {
                actor.FocusCard(focusCard_defend);
            }
            actor.StartDoAction("专注", null, false);
            yield return new WaitForSeconds(0.5f);
        }


        for (int i =0;i<action_count;i++)
        {
            // 一次攻击
 

            
            // 移动
            if (actionMode == ActionMode.attack || actionMode == ActionMode.attack_defend)
            {
                // 确定要发动的卡牌
                //Card card = card_intention_cast_list[index];
                //index++;
                //if (index >= card_intention_cast_list.Count)
                //{
                //    index = 0;
                //}

                Card card = castCard;

                // 是否需要移动
                Vector2 dirWithDis = target.GetComponent<ActorMono>().WorldPos - actor.WorldPos;
                if (!card.IfCanCast(actor,target.GetComponent<ActorMono>()))
                {
                    // 确定路径
                    var path_list = PathFinderManager.instance.SearchPathLinkTo(actor.WorldPos, target.GetComponent<ActorMono>().WorldPos);
                    //path_list.RemoveAt(path_list.Count - 1);

                    actor.StartMoveByList(path_list);

                    while (actor.IsMoving)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
                // 距离不够 结束回合
                dirWithDis = target.GetComponent<ActorMono>().WorldPos - actor.WorldPos;
                if (!card.IfCanCast(actor, target.GetComponent<ActorMono>()))
                {
                    break;
                }
                // 移动完毕 

                // 释放卡牌
                UIManager.instance.UpdateActorFloatUI(gameObject, card.cardName, 2);
                CombatManager.instance.StartCombat(actor, card, target.GetComponent<ActorMono>());
            }
            if(actionMode == ActionMode.dfend)
            {
                List<Vector3> path_list = new List<Vector3>();
                List<Vector3Int> targetPos_list = new List<Vector3Int>();
                int count = 0;
                while ((path_list == null || path_list.Count == 0) && count < 10)
                {
                    count++;

                    int tileCount = (int)(actor.movePoint / BattleManager.instance.moveCost_varCell);
                    int c = Random.Range(0, tileCount + 1);
                    Vector2Int tilePos = new Vector2Int(c, tileCount - c);
                    Vector3Int pos = PathFinderManager.instance.grid.WorldToCell(actor.WorldPos);

                    int x = target.GetComponent<ActorMono>().WorldPos.x > actor.WorldPos.x ? -1 : 1;
                    int y = target.GetComponent<ActorMono>().WorldPos.y > actor.WorldPos.y ? -1 : 1;

                    pos += new Vector3Int(x * tilePos.x, y * tilePos.y, 0);

                    if (targetPos_list.Contains(pos))
                        continue;
                    else
                    {
                        targetPos_list.Add(pos);
                    }

                    Vector3 targetPos = PathFinderManager.instance.grid.GetCellCenterWorld(pos);

                    path_list = PathFinderManager.instance.SearchPathTo(actor.WorldPos,targetPos);

                    
                }

                actor.StartMoveByList(path_list);

                while (actor.IsMoving)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            // 专注卡牌
            // 确定是否要专注

            while(CombatManager.instance.isCombating)
            {
                yield return new WaitForEndOfFrame();
            }


            
        }

        while(CombatManager.instance.isCombating)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.6f);

        BattleManager.instance.OnTurnEnd();

        
    }


    // 搜索对象

    private GameObject SearchTarget()
    {
        // 获得player列表
        List<GameObject> players_list = new List<GameObject>();
        List<GameObject> actors_list = BattleManager.instance.actorsInBattle_list;
        
        for(int i =0;i< actors_list.Count;i++)
        {
            if(actors_list[i].GetComponent<ActorMono>().group == ActorMono.Group.player)
            {
                players_list.Add(actors_list[i]);
            }
        }


        // 计算成本
        float[] costs_array = new float[players_list.Count];

        for(int i =0;i<players_list.Count;i++)
        {
            float cost = Vector3.Distance(players_list[i].transform.position, transform.position);
            costs_array[i] = cost;
        }

        float min = 9999999f;
        int index_min = 9999;
            
        for(int i=0;i<costs_array.Length;i++)
        {
            if (costs_array[i] < min)
            {
                min = costs_array[i];
                index_min = i;
            }

        }

        if (index_min < 9999)
        {
            return players_list[index_min];
        }
        else
            return null;
        

    }
}
