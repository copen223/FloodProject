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
        combat.StartThisCombat();
    }
}
