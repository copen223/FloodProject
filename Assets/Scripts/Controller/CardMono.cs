using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Struct;
using System;
using UnityEngine.XR.WSA.Input;

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
    public Text cardName_text;
    public Text cardDescription_text;

    public Color focus_frame_color;
    public Color focus_one_frame_color;

    public Vector2 scale_ori;
    public Vector2 scale_big;
    public Vector2 offset;

    private bool isWatting = false;
    private float waitTime = 0;
    private float timer = 0;

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
        if (GameManager.instance.gameInputMode == GameManager.InputMode.animation)
            return;

        if(timer > waitTime)
        {
            isWatting = false;
        }
        else
        {
            timer += Time.deltaTime;
        }

        if(isWatting)
        {
            return;
        }

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

        //Debug.Log("none  " + gameObject.name);

        transform.localScale = scale_ori;
        transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);

        
    }
    private void None()
    {
        
    }
    private void OnSelected()
    {
        //Debug.Log("selected  " + gameObject.name);
        transform.localScale = scale_big;
        transform.localPosition = new Vector3(transform.localPosition.x, offset.y, 0);

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
        {
            state = State.selected;
            OnSelected();
            return;
        }

        UIManager.instance.ActiveUI("TargetSelect", true);
        // 1-3 2-5 3-7
        UIManager.instance.SetUIScale("TargetSelect", new Vector3(card.cast_extent_x *2 + 1,card.cast_extent_y * 2 + 1));
        UIManager.instance.TranslateUIPos("TargetSelect",  HolderPos);
        UIManager.instance.HideUI("Hand", true);

        GameManager.instance.gameInputMode = GameManager.InputMode.selectarget;

        state = State.casted;

    }
    private void Casted()
    {
        UIManager.instance.TranslateUIPos("TargetSelect", HolderPos);

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10;
        Vector3 mouse_pos = Camera.main.ScreenToWorldPoint(mousePosition);

        

        // 检测距离
        bool CanSelect = true;

        float dis_x = Mathf.Abs(mouse_pos.x - holder.WorldPos.x);
        float dis_y = Mathf.Abs(mouse_pos.y - holder.WorldPos.y);

        if (dis_x > card.cast_extent_x + 0.5f || dis_y > card.cast_extent_y +0.5f)
        {
            CanSelect = false;
        }

        Collider2D collider = new Collider2D();

        if (CardModel.cast_type ==  Card.CastType.射线单体)
        {

            if (!CanSelect)
            {
                UIManager.instance.UpdateLineUI(HolderPos, mouse_pos, false);
                return;
            }
            RaycastHit2D[] hits = Physics2D.RaycastAll(holder.WorldPos, (mouse_pos - holder.WorldPos).normalized, Vector3.Distance(holder.WorldPos, mouse_pos));
            List<GameObject> hitObjects = new List<GameObject>();

            RaycastHit2D hit = new RaycastHit2D();
            bool isHitTarget = false;

            for (int i = 0; i < hits.Length; i++)
            {
                hit = hits[i];
                if (hit.collider.gameObject == holder.gameObject)    // 射到自己 跳过
                {
                    isHitTarget = false;
                    continue;
                }
                if (hit.collider.tag == "Ladder") // 射到梯子 跳过
                {
                    isHitTarget = false;
                    continue;
                }

                if (hit.collider.tag == "Obstacle")  // 射到墙壁 中断
                {
                    isHitTarget = true;
                    break;
                }

                if (hit.collider.tag == "Actor")
                {
                    isHitTarget = true;
                    hitObjects.Add(hit.collider.gameObject);
                }
            }

            if (!isHitTarget)
            {
                UIManager.instance.UpdateLineUI(holder.WorldPos, mouse_pos, true);
            }
            else if (hit)
            {
                Vector3 target_pos = hit.point;
                UIManager.instance.UpdateLineUI(holder.WorldPos, target_pos, true);
            }

            if (hit.collider.tag == "Actor" && isHitTarget)
            {
                collider = hit.collider;
                ActorMono targetMono = hit.collider.gameObject.GetComponent<ActorMono>();
                if (targetMono != null)
                {
                    CanSelect = true;
                    targetMono.OnMouseSelect(CanSelect);
                }
                else
                    CanSelect = false;
            }
        }
        else if(CardModel.cast_type ==  Card.CastType.指向单体)
        {
            collider = Physics2D.OverlapPoint(mouse_pos);

            if (collider != null)
            {
                ActorMono targetMono = collider.gameObject.GetComponent<ActorMono>();

                if (targetMono != null)
                {
                    targetMono.OnMouseSelect(CanSelect);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            UIManager.instance.UpdateLineUI(HolderPos, mouse_pos, false);
            GameManager.instance.gameInputMode = GameManager.InputMode.play;
            state = State.none;
            OnNone();
        }

        if (CanSelect)
        {
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                // 进行cast
                if (collider != null)
                {
                    ActorMono targetMono = collider.gameObject.GetComponent<ActorMono>();
                    if (targetMono != null)
                    {
                        targetMono.OnMouseSelect(false);
                        UIManager.instance.UpdateLineUI(HolderPos, mouse_pos, false);
                        DoCasted(collider.gameObject.GetComponent<ActorMono>());
                        //GameManager.instance.gameInputMode = GameManager.InputMode.play;
                    }
                }
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
        holder.StartDoAction("专注", null,false);

        ActiveActionUI(false);
        transform.localScale = scale_ori;
        transform.localPosition = new Vector3(transform.localPosition.x,offset.y, 0);
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
            holder = value.holder;
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

        cardName_text.text = card.cardName;

        signInten_up.text = "" + card.sign_up.intensity;
        signInten_down.text = "" + card.sign_down.intensity;

        UpdateSignView(card.sign_up.subType,true);
        UpdateSignView(card.sign_down.subType, false);

        // 描述文本
        string des = "";
        des += GetDescriptionOfSign(card.sign_up);
        des += GetDescriptionOfSign(card.sign_down);
        cardDescription_text.text = des;

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
    private string GetDescriptionOfSign(CardSign sign)
    {
        string description = "";
        if (sign.type == CardSign.Type.atk)
        {
            description += "造成" + card.damage_multiply * holder.atk + "点伤害;" + " ";
        }
        if (sign.type == CardSign.Type.dfd)
        {
            if (sign.type == CardSign.Type.dfd)
            {
                if (sign.subType == CardSign.SubType.dfd_block)
                {
                    description += "格挡" + holder.dfd + "点伤害;" + " ";
                }
                if (sign.subType == CardSign.SubType.dfd_dodge)
                {
                    description += "闪避;" + " ";
                }
                if (sign.subType == CardSign.SubType.dfd_parry)
                {
                    description += "招架;" + " ";
                }
            }
        }
        return description;
    }


    private void UpdateSignView(CardSign.SubType sub,bool isAtUp)
    {
        Image image = null;

        if (isAtUp)
        {
            image = signIcon_up;
        }
        else
            image = signIcon_down;

        switch (sub)
        {
            case CardSign.SubType.atk_blunt:
                image.sprite = CardManager.instance.atk_blunt_sign_sprite;
                break;
            case CardSign.SubType.atk_chop:
                image.sprite = CardManager.instance.atk_chop_sign_sprite;
                break;
            case CardSign.SubType.atk_stab:
                image.sprite = CardManager.instance.atk_stab_sign_sprite;
                break;
            case CardSign.SubType.dfd_block:
                image.sprite = CardManager.instance.dfd_block_sign_sprite;
                break;
            case CardSign.SubType.dfd_dodge:
                image.sprite = CardManager.instance.dfd_dodge_sign_sprite;
                break;
            case CardSign.SubType.dfd_parry:
                image.sprite = CardManager.instance.dfd_parry_sign_sprite;
                break;
            case CardSign.SubType.none:
                image.sprite = CardManager.instance.none_sign_sprite;
                break;


        }
    }
    // 其他操作
    /// <summary>
    /// 设定卡片
    /// </summary>
    public void SetCard(Card card)
    {
        CardModel = card;
    }
    /// <summary>
    /// 发动卡片
    /// </summary>
    /// <param name="target"></param>
    private void DoCasted(ActorMono target)
    {
        //holder.GetComponent<ActorMono>().StartCoroutine(holder.GetComponent<ActorMono>().CastCard(card, target));
        holder.GetComponent<ActorMono>().CastCard(card, target);
        state = State.none;
        OnNone();
    }


    // 事件
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.gameInputMode == GameManager.InputMode.animation)
            return;
        if (isWatting)
            return;


        // 点击->clicked
        if (state == State.selected && eventData.button == PointerEventData.InputButton.Left)
        {
            if (card.cast_type == Card.CastType.无)
                return;

            state = State.casted;
            OnCasted();
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
        if (isWatting)
            return;
        if (GameManager.instance.gameInputMode == GameManager.InputMode.animation)
            return;
        // 放大卡牌
        if (state == State.none)
        {
            state = State.selected;
            OnSelected();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isWatting)
            return;
        // 还原卡牌状态
        if (state == State.clicked || state == State.selected)
        {
            state = State.none;
            OnNone();
        }
    }

    public void WaitForSeconds(float time)
    {
        isWatting = true;
        waitTime = time;
    }

}
