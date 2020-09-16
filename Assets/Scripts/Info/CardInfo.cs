using Assets.Scripts.Struct;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "卡片", menuName = "序列化卡片")]
public class CardInfo : ScriptableObject
{
    public string sign_up;
    public string sign_down;
    public string effect;
    public Card.CastType cast_type;
    public int cast_extent_x;
    public int cast_extent_y;
    public int memory_cost;
    public float damage_multiply;

    // 描述
    public string cardName;
}
