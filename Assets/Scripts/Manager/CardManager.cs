using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Struct;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    private void Awake()
    {
        instance = this;
    }

    // 卡牌数据
    public List<CardInfo> cardInfos_list = new List<CardInfo>();


    // UI资源
    public Sprite atk_sign_sprite;
    public Sprite dfd_sign_sprite;
    public Sprite none_sign_sprite;

    
    // 卡牌创建

    /// <summary>
    /// 通过cardInfo获取card
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public Card GetCardByInfo(CardInfo info)
    {
        Card card = new Card();
        card.cast_type = "范围指向";
        card.effect = new CardEffect();
        card.sign_up = GetCardSignByText(info.sign_up, true);
        card.sign_down = GetCardSignByText(info.sign_down, false);
        card.cast_extent_x = info.cast_extent_x;
        card.cast_extent_y = info.cast_extent_y;

        return card;
    }

    /// <summary>
    /// 获取一个空白卡牌
    /// </summary>
    /// <returns></returns>
    public Card GetBlankCard()
    {
        Card card = new Card();
        card.cast_type = "无";
        card.effect = new CardEffect();
        card.sign_up = GetCardSignByText("空白", true);
        card.sign_down = GetCardSignByText("空白", false);
        card.memory_cost = -1;

        return card;
    }

    private CardSign GetCardSignByText(string txt,bool isAtUp)
    {
        char[] words = txt.ToCharArray();

        // 名称
        List<char> name = new List<char>();
        // 强度
        List<char> value = new List<char>();
        
        for(int i=0;i<words.Length;i++)
        {
            if(words[i]>47 && words[i]<58)
            {
                value.Add(words[i]);
            }
            else
            {
                name.Add(words[i]);
            }
        }

        char[] _name = name.ToArray();
        string mname = new string(_name);

        char[] _value = value.ToArray();

        CardSign sign = new CardSign();

        switch (mname)
        {
            case ("劈砍"):
                sign.subType = CardSign.SubType.atk_chop;
                sign.type = CardSign.Type.atk;
                break;
            case ("突刺"):
                sign.subType = CardSign.SubType.atk_stab;
                sign.type = CardSign.Type.atk;
                break;
            case ("钝击"):
                sign.subType = CardSign.SubType.atk_blunt;
                sign.type = CardSign.Type.atk;
                break;
            case ("格挡"):
                sign.subType = CardSign.SubType.dfd_block;
                sign.type = CardSign.Type.dfd;
                break;
            case ("招架"):
                sign.subType = CardSign.SubType.dfd_parry;
                sign.type = CardSign.Type.dfd;
                break;
            case ("闪避"):
                sign.subType = CardSign.SubType.dfd_dodge;
                sign.type = CardSign.Type.dfd;
                break;
            case ("空白"):
                sign.subType = CardSign.SubType.none;
                sign.type = CardSign.Type.none;
                break;
        }

        if (_value.Length > 0)
        {
            int inten = 0;

            for (int i = 0; i < _value.Length; i++)
            {
                int pow = (int)Mathf.Pow(10, _value.Length - i - 1);
                inten += (_value[i] - 48) * pow;
            }

            sign.intensity = inten;
        }
        else
        {
            sign.intensity = 0;
        }

        sign.pos = isAtUp ? CardSign.Pos.up : CardSign.Pos.down;

        sign.effect = GetSignEffect(sign.type);

        return sign;
    }

    private SignEffect GetSignEffect(CardSign.Type type)
    {
        switch (type)
        {
            case CardSign.Type.atk:
                return new SignEffect_Damage();
            case CardSign.Type.dfd:
                return new SignEffect_ReduceDamage();
            case CardSign.Type.none:
                return new SignEffect_None();
            default:
                return new SignEffect_None();
        }
    }

    // 从数据库获取卡牌

    /// <summary>
    /// 在manager的cardinfos_list中搜索对应卡牌并创建
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Card GetCardByName(string name)
    {
        for(int i=0;i<cardInfos_list.Count;i++)
        {
            if(cardInfos_list[i].cardName == name)
            {
                return GetCardByInfo(cardInfos_list[i]);
            }
        }
        return null;
    }

    public CardPile GetDeckByInfo(DeckInfo info)
    {
        List<string> cards = info.cardWithNum;

        CardPile pile = new CardPile("deckPile");

        for (int i = 0; i < cards.Count; i++)
        {
            char[] words = cards[i].ToCharArray();

            // 名称
            List<char> name = new List<char>();
            // 数量
            List<char> value = new List<char>();

            for (int j = 0; j < words.Length; j++)
            {
                if (words[j] > 47 && words[j] < 58)
                {
                    value.Add(words[j]);
                }
                else
                {
                    name.Add(words[j]);
                }
            }

            char[] _name = name.ToArray();
            string mname = new string(_name);

            char[] _value = value.ToArray();

            for(int j =0;j<_value[0] -48;j++)
            {
                pile.AddCard(GetCardByName(mname));
            }
        }

        return pile;
    }
}
