using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Struct;
using TMPro.EditorUtilities;
using System.Runtime.InteropServices.WindowsRuntime;
using System;

public class CardMono : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    private void Start()
    {
        
    }


    // UI控制
    public GameObject cast_icon;
    public GameObject focus_icon;
    public GameObject focus_frame;
    public Text signInten_up;
    public Text signInten_down;
    public Text foucsCount;
    public Image signIcon_up;
    public Image signIcon_down;

    public Color focus_frame_color;
    public Color focus_one_frame_color;

    public Vector2 scale_ori;
    public Vector2 scale_big;
    public Vector2 offset;

    #region 状态机
    public enum State
    {
        none,
        selected,   // 被选中
        clicked,    // 被点击
        casted,     // 被发动
        focused     // 被专注
    }

    public State state;

    private void Update()
    {
        switch (state)
        {
            case State.none:None();break;
            case State.selected:Selected();break;
            case State.clicked:Clicked();break;
            case State.casted:Casted();break;
            case State.focused:Focused();break;
        }
        
    }
    public void OnNone()
    {
        ActiveActionUI(false);
        UIManager.instance.ActiveUI("TargetSelect", false);
        UIManager.instance.HideUI("Hand", false);

        transform.localScale = scale_ori;
        transform.position = (Vector2) transform.position - offset;

        
    }
    private void None()
    {
        
    }
    private void OnSelected()
    {
        transform.localScale = scale_big;
        transform.position = (Vector2) transform.position + offset;
    }
    private void Selected()
    {

    }
    private void OnClicked()
    {
        ActiveActionUI(true);
    }
    private void Clicked()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            ActiveActionUI(false);
            state = State.selected;
        }

    }
    public void OnCasted()
    {
        if (holder.ActionPoint <= 0)
            return;

        UIManager.instance.ActiveUI("TargetSelect", true);
        // 1-3 2-5 3-7
        UIManager.instance.SetUIScale("TargetSelect", new Vector3(card.cast_extent_x *2 + 1,card.cast_extent_y * 2 + 1));
        UIManager.instance.TranslateUIPos("TargetSelect",  HolderPos);
        UIManager.instance.HideUI("Hand", true);

        state = State.casted;

    }
    private void Casted()
    {
        UIManager.instance.TranslateUIPos("TargetSelect", HolderPos);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            state = State.none;
            OnNone();
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            // 确定选中网格
            Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int mouse_cell = PathFinderManager.instance.grid.WorldToCell(mouse_pos);
            Vector3 mouse_cell_pos = PathFinderManager.instance.grid.GetCellCenterWorld(mouse_cell);
            // 检测距离
            float dis_x = Mathf.Abs(mouse_cell_pos.x - holder.WorldPos.x);
            float dis_y = Mathf.Abs(mouse_cell_pos.y - holder.WorldPos.y);

            if(dis_x > card.cast_extent_x || dis_y > card.cast_extent_y)
            {
                return;
            }

            // 进行cast
            RaycastHit2D hit = Physics2D.Raycast(mouse_cell_pos, Vector2.zero);
            if(hit.collider!=null)
            {
                holder.ActionPoint -= 1;
                DoCasted(hit.collider.gameObject.GetComponent<ActorMono>());
            }
        }
    }
    public void OnFocused()
    {
        //if (holder.ActionPoint <= 0)
        //    return;

        if (holder.focusPoint_max <= holder.FocusPoint)
            return;

        state = State.focused;
        //holder.ActionPoint -= 1;
        holder.FocusCard(CardModel);

        ActiveActionUI(false);
        transform.localScale = scale_ori;
        transform.position = (Vector2)transform.position - offset;
    }
    private void Focused()
    {

    }

    #endregion 

    // 数据与链接

    public CardInfo cardInfo;

    private Card card;

    /// <summary>
    /// 卡牌的抽象对象
    /// </summary>
    public Card CardModel
    {
        get
        {
            return card;
        }
        set
        {
            // 改变卡牌数据的同时更新卡牌view
            card = value;
            UpdateCardView();

        }
    }

    /// <summary>
    /// 卡牌的持有者
    /// </summary>
    public ActorMono holder;
    public Vector2 HolderPos
    {
        get { return Camera.main.WorldToScreenPoint(holder.WorldPos); }
        set { }
    }
    
    // UI操作
    private void ActiveActionUI(bool isActive)
    {
        cast_icon.SetActive(isActive);
        focus_icon.SetActive(isActive);
    }

    // 更新卡牌view
    private void UpdateCardView()
    {
        //Debug.Log(gameObject.name + " " + card.isFocused);

        signInten_up.text = "" + card.sign_up.intensity;
        signInten_down.text = "" + card.sign_down.intensity;

        switch(card.sign_up.type)
        {
            case CardSign.Type.atk:
                signIcon_up.sprite = CardManager.instance.atk_sign_sprite;
                break;
            case CardSign.Type.dfd:
                signIcon_up.sprite = CardManager.instance.dfd_sign_sprite;
                break;
            case CardSign.Type.none:
                signIcon_up.sprite = CardManager.instance.none_sign_sprite;
                signInten_up.text = "";
                break;
        }

        switch (card.sign_down.type)
        {
            case CardSign.Type.atk:
                signIcon_down.sprite = CardManager.instance.atk_sign_sprite;
                break;
            case CardSign.Type.dfd:
                signIcon_down.sprite = CardManager.instance.dfd_sign_sprite;
                break;
            case CardSign.Type.none:
                signIcon_down.sprite = CardManager.instance.none_sign_sprite;
                signInten_down.text = "";
                break;
        }

        if(card.focusCount != 0)
        {
            foucsCount.text = "" + card.focusCount;
        }
        else
        {
            foucsCount.text = "";
        }

        if(card.isFocused)
        {
            focus_frame.SetActive(true);
            if(holder.FocusedCard == CardModel)
            {
                focus_frame.GetComponent<Image>().color = focus_one_frame_color;
            }
            else
            {
                focus_frame.GetComponent<Image>().color = focus_frame_color;
            }
            state = State.focused;
        }
        else
        {
            focus_frame.SetActive(false);
            state = State.none;
        }

    }

    // 其他操作
    /// <summary>
    /// 设定卡片
    /// </summary>
    public void SetCard(Card card)
    {
        CardModel = card;
        holder = CardModel.holder;
    }
    /// <summary>
    /// 发动卡片
    /// </summary>
    /// <param name="target"></param>
    private void DoCasted(ActorMono target)
    {
        CombatManager.instance.StartCombat(holder, card, target);

        state = State.none;
        OnNone();

        holder.DiscardCard(CardModel);
    }


    // 事件
    public void OnPointerClick(PointerEventData eventData)
    {
        // 点击->clicked
        if(state == State.selected && eventData.button == PointerEventData.InputButton.Left)
        {
            state = State.clicked;
            OnClicked();
            return;
        }

        if (state == State.selected && eventData.button == PointerEventData.InputButton.Right)
        {
            OnFocused();
            return;
        }

        if(state == State.focused && eventData.button == PointerEventData.InputButton.Right)
        {
            holder.FinishFocusCard(CardModel);
            state = State.none;
            OnNone();
            return;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 放大卡牌
        if(state == State.none)
        {
            state = State.selected;
            OnSelected();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 还原卡牌状态
        if(state == State.clicked || state == State.selected)
        {
            state = State.none;
            OnNone();
        }
    }
}
