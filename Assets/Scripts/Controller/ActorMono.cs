using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Struct;
using UnityEngine.UIElements;

public class ActorMono : MonoBehaviour
{
    public enum Group
    {
        player,
        monster
    }

    public Group group;

    public enum BattleState
    {
        none,
        death
    }

    public BattleState battleState = BattleState.none;

    public bool ifActionEnd = true;


    // 属性与资源
    private int focusPoint;
    private int actionPoint;
    public float movePoint;
    public float healPoint;
    public float healPoint_max;

    public int ActionPoint
    {
        get
        {
            return actionPoint;
        }
        set
        {
            actionPoint = value;
            if (IsThisTurn)
                UIManager.instance.UpdateUIText("ActionPoint", "行动" + actionPoint);
        }
    }

    public int FocusPoint
    {
        get
        {
            return focusPoint;
        }
        set
        {
            focusPoint = value;
            if (IsThisTurn)
                UIManager.instance.UpdateUIText("FocusPoint", "专注" + focusPoint + "/" + focusPoint_max);
        }
    }


    public float atk;   // 攻击力
    public float dfd;   // 防御力
    public float amor_dfd; //护甲防御力
    public float mbt;   // 移动力
    public int advantage;   // 先攻

    // 调整值
    public int focusPoint_max;
    public int actionPoint_resume;
    public int actionPoint_max;
    public int movePoint_resume;
    public int movePoint_max;
    public float healPoint_resumeVarTurn;
    //public int cardNum_draw;
    //public int cardNum_discard;
    public int cardNum_hand_start;

    // 初始属性
    public ActorInfo info;

    // 卡池
    public CardPile handPile = new CardPile("handPile");
    public CardPile discardPile = new CardPile("discardPile");
    public CardPile focusPile = new CardPile("focusPile");
    public CardPile deckPile = new CardPile("deckPile");

    public DeckInfo deckInfo;

    // 移动
    public float move_speed;
    public Vector3 worldPos_offset;
    public Vector3 WorldPos
    {
        get
        {
            return transform.position + worldPos_offset;
        }
    }
    public Vector3Int CellPos
    {
        get
        {
            Vector3Int pos = PathFinderManager.instance.grid.WorldToCell(WorldPos);
            return pos;
        }
    }
    

    public bool IsMoving
    {
        get;
        private set;
    }

    // 标识
    private bool IsThisTurn
    {
        get
        {
            return (BattleManager.instance.actor_curTurn == this);
        }
        set
        {

        }
    }



    public Card FocusedCard
    {
        get
        {
            if (focusPile.HasCard)
                return focusPile.GetFirstCard();
            else
                return CardManager.instance.GetBlankCard();
        }
        set
        { }
    }


    public void OnBattle()
    {
        ifActionEnd = true;
        UIManager.instance.UpdateActorHpUI(gameObject, true);
    }

    // 操作
    public void Behit(float damage)
    {
        healPoint -= (damage);
        UIManager.instance.UpdateActorFloatUI(gameObject, damage + "", 0);

        if (healPoint <= 0)
        {
            healPoint = 0;
            battleState = BattleState.death;
            UIManager.instance.UpdateActorHpUI(gameObject, false);
            StartDoAction("死亡", gameObject);
            return;
        }
        else
        {
            StartDoAction("受伤", gameObject);
        }

        UIManager.instance.UpdateActorHpUI(gameObject, true);
    }

    public void ResumeHitPoint(float value)
    {
        healPoint += value;
        if (healPoint > healPoint_max)
            healPoint = healPoint_max;
        
        UIManager.instance.UpdateActorFloatUI(gameObject,value + "",0,Color.green);
        if (healPoint <= 0)
        {
            healPoint = 0;
            battleState = BattleState.death;
            UIManager.instance.UpdateActorHpUI(gameObject, false);
            StartDoAction("死亡", gameObject);
            return;
        }
        else
        {
            // 恢复行为
        }

        UIManager.instance.UpdateActorHpUI(gameObject, true);
    }

    public void ResumeActionPoint()
    {
        ActionPoint += actionPoint_resume;
        if (ActionPoint > actionPoint_max)
            ActionPoint = actionPoint_max;
    }
    public void ResumeMovePoint()
    {
        movePoint += movePoint_resume;
        if (movePoint > movePoint_max)
            movePoint = movePoint_max;

        if (IsThisTurn)
            UIManager.instance.UpdateUIText("MovePoint", "移动" + movePoint);
    }

    public void StartMoveByList(List<Vector3> pos_list)
    {
        if (pos_list == null)
            return;
        if((pos_list.Count - 1) * BattleManager.instance.moveCost_varCell > movePoint)
        {
            return;
        }

        UIManager.instance.HideUI("UIArea", true);
        UIManager.instance.HideUI("Hand", true);

        StartCoroutine(MoveByList(pos_list));
    }

    public bool stopMoveByList;
    IEnumerator MoveByList(List<Vector3> pos_list)
    {
        IsMoving = true;
        stopMoveByList = false;

        for (int i = 0; i < pos_list.Count; i++)
        {
            if(i!=0)
                movePoint -= BattleManager.instance.moveCost_varCell;
            if(IsThisTurn)
                UIManager.instance.UpdateUIText("MovePoint", "移动" + movePoint);

            Vector3 target_pos = pos_list[i];

            while(WorldPos != target_pos)
            {
                transform.position = Vector3.MoveTowards(transform.position, target_pos - worldPos_offset, move_speed * Time.deltaTime);
                UIManager.instance.UpdateActorHpUI(gameObject, true);
                yield return new WaitForEndOfFrame();
            }

            if (stopMoveByList)
                break;
        }

        IsMoving = false;

        UIManager.instance.HideUI("Hand", false);
        UIManager.instance.HideUI("UIArea", false);
    }

    // 卡牌相关
    public void DrawCard(int num)
    {
        for(int  i=0;i<num;i++)
            DrawOneCard();

        if (IsThisTurn)
        {
            UIManager.instance.UpdateHandUI(handPile.cards_list);
            UIManager.instance.UpdateUIText("DeckNum", "卡组:" + deckPile.Count);
        }
    }
    private void DrawOneCard()
    {
        if (!deckPile.HasCard)
        {
            discardPile.TranslateCardsTo(deckPile, discardPile.Count);
            UIManager.instance.UpdateUIText("DiscardNum", "弃牌:" + discardPile.Count);
            ShuffleDeck();
        }
        deckPile.TranslateCardsTo(handPile, 1);
    }
    /// <summary>
    /// 回合开始，抽取卡牌
    /// </summary>
    public void DrawStartCard()
    {
        int draw_num = cardNum_hand_start - handPile.Count;

        DrawCard(draw_num);

        if (IsThisTurn)
        {
            UIManager.instance.UpdateHandUI(handPile.cards_list);
            UIManager.instance.UpdateUIText("DeckNum", "卡组:" + deckPile.Count);
        }

        //Debug.Log("draw！");
    }

    /// <summary>
    /// 回合结束，丢弃卡牌
    /// </summary>
    public void DiscardEndCard()
    {
        for(int i =0; i<handPile.Count;i++)
        {
            Card card = handPile.cards_list[i];
            if (!card.isFocused)
            {
                DiscardCard(card);
                i--;
            }
        }
    }
    public void DiscardCard(Card card)
    {
        card.focusCount = 0;
        handPile.TranslateCardTo(card, discardPile);
        if(IsThisTurn)
        {
            UIManager.instance.UpdateHandUI(handPile.cards_list);
            UIManager.instance.UpdateUIText("DiscardNum", "弃牌:" + discardPile.Count);
        }
    }

    public void DiscardFocusedCard()
    {
        Card card = FocusedCard;
        if (card.memory_cost == -1)
        {
            // 专注的是空白卡牌 也就是没有进行专注
            // 恢复 休息恢复值
            ResumeHitPoint(healPoint_resumeVarTurn);
            return;
        }
        FinishFocusCard(card);
        DiscardCard(card);
    }

    public void CastCard(Card card, ActorMono target)
    {
        ActionPoint -= 1;

        UIManager.instance.UpdateActorFloatUI(gameObject, card.cardName, 2);
        CombatManager.instance.StartCombat(this, card, target);

        DiscardCard(card);

    }

    public void FocusCard(Card card)
    {
        if (focusPoint >= focusPoint_max)
            return;

        UIManager.instance.UpdateActorFloatUI(gameObject, card.cardName, 2);
        handPile.TranslateCardTo(card, focusPile);
        FocusPoint++;

        if(IsThisTurn)
        {
            UIManager.instance.UpdateHandUI(handPile.cards_list);
        }
    }

    public void FinishFocusCard(Card card)
    {
        focusPile.RemoveCard(card);
        FocusPoint--;
        card.isFocused = false;

        if (IsThisTurn)
        {
            UIManager.instance.UpdateHandUI(handPile.cards_list);
        }
    }

    public void FinishFocus()
    {
        foreach(var card in handPile.cards_list)
        {
            if(card.isFocused)
            {
                focusPile.RemoveCard(card);
                FocusPoint--;
                card.isFocused = false;
                card.focusCount+=1;
            }
        }

        if (IsThisTurn)
        {
            UIManager.instance.UpdateHandUI(handPile.cards_list);
        }
    }

    public void FocusUp()
    {
        foreach (var card in handPile.cards_list)
        {
            if (card.isFocused)
            {
                card.focusCount += 1;
            }
        }

        if (IsThisTurn)
        {
            UIManager.instance.UpdateHandUI(handPile.cards_list);
        }
    }

    // 开始演出
    public void StartDoAction(string action,GameObject target)
    {
        ifActionEnd = false;
        StopAllCoroutines();
        StartCoroutine(DoAction(action, target));
    }

    // 演出协程
    private IEnumerator DoAction(string action,GameObject target)
    {
        float timer = 0f;

        if(action == "受伤")
        {
            //UIManager.instance.UpdateActorFloatUI(gameObject, "被击中", 1);


            while(timer<0.7f)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        if (action == "攻击" || action == "反击")
        {
            UIManager.instance.UpdateActorFloatUI(gameObject, action, 1);

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
        }
        if (action == "闪避")
        {
            UIManager.instance.UpdateActorFloatUI(gameObject, "闪避", 1);
            while (timer < 0.7f)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        if (action == "格挡")
        {
            UIManager.instance.UpdateActorFloatUI(gameObject, "格挡", 1);
            while (timer < 0.7f)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        if(action == "死亡")
        {
            UIManager.instance.UpdateActorFloatUI(gameObject, "死亡", 1);
            // 死亡时间
            while(timer<0.7f)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            // 死亡结果
            UIManager.instance.UpdateActorFloatUI(gameObject, "", 0, false);
            UIManager.instance.UpdateActorFloatUI(gameObject, "", 1, false);
            BattleManager.instance.RemoveFromBattle(gameObject);
            if (IsThisTurn)
            {
                ifActionEnd = true;
                BattleManager.instance.OnTurnEnd();
            }
            gameObject.SetActive(false);
        }
        if(action == "专注")
        {
            UIManager.instance.UpdateActorFloatUI(gameObject, "专注", 1);
        }
        ifActionEnd = true;

    }

    // 初始化
    public void InitDeck()
    {
        deckPile = CardManager.instance.GetDeckByInfo(deckInfo);
        deckPile.SetHolder(this);
    }

    public void ShuffleDeck()
    {
        deckPile.Shuffle();
    }



    // 鼠标指向事件

    private void OnMouseEnter()
    {
        if (GameManager.instance.gameState == GameManager.State.Explore)
            return; 

        switch (GameManager.instance.gameInputMode)
        {
            case GameManager.InputMode.animation:
                UIManager.instance.UpdateActorDirSignUI(gameObject, false);
                break;
            case GameManager.InputMode.selectarget:
                UIManager.instance.UpdateActorDirSignUI(gameObject, true);
                break;
            case GameManager.InputMode.play:
                UIManager.instance.UpdateActorDirSignUI(gameObject, true);
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            default:
                break;
        }
    }

    private void OnMouseExit()
    {
        if (GameManager.instance.gameState == GameManager.State.Explore)
            return;
        UIManager.instance.UpdateActorDirSignUI(gameObject, false);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void OnMouseSelect(bool isSelected)
    {
        if(GameManager.instance.gameInputMode == GameManager.InputMode.selectarget)
        {
            if(isSelected)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
