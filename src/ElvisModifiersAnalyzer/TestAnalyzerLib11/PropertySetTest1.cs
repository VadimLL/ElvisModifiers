namespace TestAnalyzerLib11;

file class Me
{
    [OnlyYouSet<Friend>(nameof(Friend.SetValue1))]
    public static int GlobalMoney { get; set; } = 100;

    public const string SetValue2Alias = nameof(SetValue2Alias);

    [OnlyYouSet<Friend>(nameof(Friend.SetValue1))]
    public int Value1 { get; set; } = 100;
    
    [OnlyAliasSet<Friend>(SetValue2Alias)]
    public int Value2 { get; set; } = 100;
}

[OnlyYouSet<Friend>]
file class Me2
{
    public int Value { get; set; } = 100;
    public void Method() { }
}

file class Friend
{
    public void SetValue1(in Me me) // ok
    {
        var half = (me.Value1) / 2;
        me.Value1 = half;
        me.Value1 += 100;
        ++me.Value1;
        me.Value1++;

        Me.GlobalMoney = 100;
    }

    [Alias(Me.SetValue2Alias)]
    public void SetValue2(in Me me) // ok
    {
        var half = (me.Value1) / 2;
        half = (me.Value2) / 2;
        me.Value2 = half;
        me.Value2 += 100;
        ++me.Value2;
        me.Value2++;

        /*EA_PROP_002*/ me.Value1 = half; // err
        /*EA_PROP_002*/ (me.Value1) = 100; // err
        /*EA_PROP_002*/ me.Value1 += 100; // err
        /*EA_PROP_002*/ ++((me.Value1)); // err
        //...

        half = Me.GlobalMoney / 2; // ok
        /*EA_PROP_002*/ Me.GlobalMoney = 100; // err
    }

    public void CantSetValue(in Me me)
    {
        var half = me.Value1 / 2; // ok
        /*EA_PROP_002*/ me.Value1 = half; // err
        /*EA_PROP_002*/ me.Value1 += 100; // err
        /*EA_PROP_002*/ ++((me.Value1)); // err
        //...

        half = (me.Value2) / 2; // ok
        /*EA_PROP_002*/ me.Value2 = half; // err
        /*EA_PROP_002*/ me.Value2 += 100; // err
        /*EA_PROP_002*/ ++((me.Value2)); // err
        //...

        half = Me.GlobalMoney / 2; // ok
        /*EA_PROP_002*/ Me.GlobalMoney = half; // err
    }

    public void UseMe2(Me2 me2)
    {
        me2.Value = 0;
        me2.Method();
    }
}

file class NoFriend
{
    [Alias(Me.SetValue2Alias)]
    public void SetValue(in Me me)
    {
        var half = me.Value1 / 2; // ok
        /*EA_PROP_002*/ me.Value1 = half; // err
        //...

        half = (me.Value2) / 2; // ok
        /*EA_PROP_002*/ me.Value2 = half; // err

        half = Me.GlobalMoney / 2; // ok
        /*EA_PROP_002*/ Me.GlobalMoney = half; // err
    }

    public void UseMe2(Me2 me2)
    {
        /*EA_TYPE_002*/ me2.Value = 0;
        me2.Method(); // ok
    }
}
