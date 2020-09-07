using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        Explore,
        Battle
    }

    public State gameState;

    public enum InputMode
    {
        none,
        play,
        animation,
        selectarget
    }

    public InputMode gameInputMode;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        OnExplore();
    }

    /// <summary>
    /// 进入战斗状态
    /// </summary>
    public void OnBattle()
    {
        gameState = State.Battle;
        BattleManager.instance.OnBattleStart();
    }

    /// <summary>
    /// 进入探索状态
    /// </summary>
    public void OnExplore()
    {
        gameState = State.Explore;
        BattleManager.instance.OnBattleEnd();
    }

}
