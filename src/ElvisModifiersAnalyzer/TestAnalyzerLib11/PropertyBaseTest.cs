namespace TestAnalyzerLib11;

file class Me
{
    [OnlyYou<MyFriend>(nameof(MyFriend.SetGlobalMoney))]
    public static decimal GlobalMoney { get; set; } = 100;

    [OnlyYou<MyFriend>(nameof(MyFriend.SetMoney))]
    public decimal Money { get; set; } = 100;

    public void SelfSetMoney() => Money = 200; // ok
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
    }

    public void SetGlobalMoney() // ok
    {
        var half = Me.GlobalMoney / 2;
        Me.GlobalMoney = half;
        //... ok
    }
}