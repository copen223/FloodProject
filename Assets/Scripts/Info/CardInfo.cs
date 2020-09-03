using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "卡片", menuName = "序列化对象")]
public class CardInfo : ScriptableObject
{
    public string sign_up;
    public string sign_down;
    public string effect;
    public string cast_type;
    public int cast_extent_x;
    public int cast_extent_y;
    public int memory_cost;

    // 描述
    public string cardName;
}
