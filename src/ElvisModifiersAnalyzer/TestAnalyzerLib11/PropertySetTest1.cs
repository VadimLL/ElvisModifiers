namespace TestAnalyzerLib11;

file class Me
{
    [OnlyYouSet<MyFriend>(nameof(MyFriend.SetMoney1))]
    public static decimal GlobalMoney { get; set; } = 100;

    public const string SetMoney2 = nameof(SetMoney2);

    [OnlyYouSet<MyFriend>(nameof(MyFriend.SetMoney1))]
    public decimal Money1 { get; set; } = 100;
    
    [OnlyAliasSet<MyFriend>(SetMoney2)]
    public decimal Money2 { get; set; } = 100;

    [OnlyYouSet<MyFriend>(nameof(MyFriend.SetMoney1))]
    public decimal MoneyF = 100;

}

file class MyFriend
{
    public void SetMoney1(in Me me) // ok
    {
        var half = (me.Money1) / 2;
        me.Money1 = half;
        me.Money1 += 100;
        ++me.Money1;
        me.Money1++;

        Me.GlobalMoney = 100;

        half = me.MoneyF;
        ++me.MoneyF;
    }

    [Alias(Me.SetMoney2)]
    public void SetMoney2(in Me me) // ok
    {
        var half = (me.Money1) / 2;
        half = (me.Money2) / 2;
        me.Money2 = half;
        me.Money2 += 100;
        ++me.Money2;
        me.Money2++;

        /*EA_PROP_002*/ me.Money1 = half; // err
        /*EA_PROP_002*/ (me.Money1) = 100; // err
        /*EA_PROP_002*/ me.Money1 += 100; // err
        /*EA_PROP_002*/ ++((me.Money1)); // err
        //...

        half = Me.GlobalMoney / 2; // ok
        /*EA_PROP_002*/ Me.GlobalMoney = 100; // err

        half = me.MoneyF; // ok
        /*EA_PROP_002*/ ++me.MoneyF;
    }

    public void CantSetMoney(in Me me)
    {
        var half = me.Money1 / 2; // ok
        /*EA_PROP_002*/ me.Money1 = half; // err
        /*EA_PROP_002*/ me.Money1 += 100; // err
        /*EA_PROP_002*/ ++((me.Money1)); // err
        //...

        half = (me.Money2) / 2; // ok
        /*EA_PROP_002*/ me.Money2 = half; // err
        /*EA_PROP_002*/ me.Money2 += 100; // err
        /*EA_PROP_002*/ ++((me.Money2)); // err
        //...

        half = Me.GlobalMoney / 2; // ok
        /*EA_PROP_002*/ Me.GlobalMoney = half; // err
    }
}

file class NotMyFriend
{
    [Alias(Me.SetMoney2)]
    public void SetMoney(in Me me)
    {
        var half = me.Money1 / 2; // ok
        /*EA_PROP_002*/ me.Money1 = half; // err
        //...

        half = (me.Money2) / 2; // ok
        /*EA_PROP_002*/ me.Money2 = half; // err

        half = Me.GlobalMoney / 2; // ok
        /*EA_PROP_002*/ Me.GlobalMoney = half; // err
    }
}
