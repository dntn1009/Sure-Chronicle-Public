using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefineHelper
{
    public enum CardUpgradeCostType
    {
        Level   = 0,
        Star
    }

    public enum MessageType
    {
        Gold    = 0,
        Jewel,
        Card
    }

    public enum BoxType
    {
        Card        = 0,
        Gold,
        Jewel,
        AD
    }
    public enum CardType
    {
        NORMAL       = 0,
        RARE,
        EPIC,
        LEGENDARY
    }
    public enum CardState
    {
        SPAWN = 0,
        LIST,
        FORMING
    }

    public enum CardList
    {
        List    = 0,
        Forming
    }
    public enum PlayerState
    {
        idle,
        run,
        attack,
        death,
        skill
    }

    public enum AttackType
    {
        Bow = 0,
        Normal,
        Magic
    }

    public enum WindowState
    {
        None        = 0,
        Hero,
        Pull,
        Enforce,
        Raid
    }

    public enum MonsterType
    {
        MeleeWeak = 0,
        MeleeStrong,
        RangedWeak,
        RangedStrong,
        StageBoss,
        RaidBoss
    }

    public enum EnforceListType
    {
        Attack  = 0,
        Defence,
        HP,
        Gold
    }
    public enum RaidType
    {
        Gold    = 0,
        Jewel
    }

}
