using System;
using System.Collections.Generic;

namespace TestAnalyzerLib11;

file class Me
{
    [OnlyYou<MyFriend>(nameof(MyFriend.SetGlobalMoney))]
    public static decimal GlobalMoney { get; set; } = 100;

    [OnlyYou<MyFriend>(nameof(MyFriend.SetMoney))]
    public decimal Money { get; set; } = 100;

    [OnlyYou<MyFriend>(nameof(MyFriend.SetMoney))]
    public decimal MoneyF = 100;

    [OnlyYou<MyFriend>(nameof(MyFriend.SetMoney))]
    public event Action<int>? Updated;

    [OnlyYou<MyFriend>(nameof(MyFriend.SetMoney))]
    public IList<decimal> Moneys = new List<decimal>();

    public void SelfSetMoney()
    {
        Money = 200; // ok
        this.MoneyF = MoneyF = 200; // ok
        Updated?.Invoke(1);
        Money =  Moneys[0];
        Moneys[0] = 0;
    }
}

file class MyFriend
{
    public void SetMoney(in Me me) // ok
    {
        var half = me.Money / 2;
        me.Money = half;
        me.Money += half;
        me.Money -= half;
        me.Money *= half;
        me.Money /= half;
        ++me.Money;
        --me.Money;
        (me.Money)++;
        me.Money--;

        half = me.MoneyF;
        ++me.MoneyF;

        me.Updated += _ => { };

        half = me.Moneys[0];
        me.Moneys[0] = 0;
    }

    public void CantSetMoney(in Me me) // err
    {
        var half = /*EA_PROP_001*/ me.Money / 2;
        /*EA_PROP_002*/ me.Money = half;
        /*EA_PROP_002*/ me.Money += half;
        /*EA_PROP_002*/ me.Money -= half;
        /*EA_PROP_002*/ me.Money *= half;
        /*EA_PROP_002*/ me.Money /= half;
        /*EA_PROP_002*/ (me.Money)++;
        /*EA_PROP_002*/ me.Money--;
        /*EA_PROP_002*/ ++me.Money;
        /*EA_PROP_002*/ --me.Money;

        half = /*EA_PROP_001*/ Me.GlobalMoney / 2;
        /*EA_PROP_002*/ Me.GlobalMoney = half;
        //... err

        half = /*EA_PROP_001*/ me.MoneyF;
        /*EA_PROP_002*/ ++me.MoneyF;

        /*EA_PROP_002*/ me.Updated += _ => { };

        half = /*EA_PROP_001*/ me.Moneys[0];
        /*EA_PROP_001*/ me.Moneys[0] = 0;
    }

    public void SetGlobalMoney() // ok
    {
        var half = Me.GlobalMoney / 2;
        Me.GlobalMoney = half;
        //... ok
    }
}