using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "卡组", menuName = "序列化卡组")]
public class DeckInfo : ScriptableObject
{
    public List<string> cardWithNum = new List<string>();
}
