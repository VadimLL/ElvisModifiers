namespace TestAnalyzerLib11;

file class Me
{
    public const string Alias1 = nameof(Alias1);
    public const string Alias2 = nameof(Alias2);

    [OnlyYou<MyFriend>("R:^Use[1-3]Me$")]
    public int Value1 { get; set; }

    [OnlyYouSet<MyFriend>("R:^Use[1-3]Me$")]
    public int Value2 { get; set; }

    [OnlyAliasSet<MyFriend>("R:^Alias[1-2]$")]
    public int Value3 { get; set; }

    [OnlyYou<MyFriend>("R:^Use[1-4]Me$")]
    public int Method1()
    {
        Value1 += 1;
        return Value1;
    }

    [OnlyYou(@$"R:^{nameof(TestAnalyzerLib11)}\.(MyFriend|Friend1)$", "R:^Use[1-2]Me$")]
    public int Method2()
    {
        Value2 += 1;
        return Value2;
    }

    [OnlyAlias<MyFriend>("R:^Alias[1-2]$")]
    public int Method3()
    {
        Value1 += 1;
        return Value1;
    }
}

[OnlyYou(@$"R:^{nameof(TestAnalyzerLib11)}\.(MyFriend|Friend1)$", "R:^Use[1-2]Me2$")]
file class Me2
{
    public void Method1() { }
    public int Value1 { get; set; }
}

file class MyFriend
{
    public void Use1Me(Me me)
    {
        me.Method1();
        me.Method2();
        me.Value1 = 0;
        me.Value2 = 0;
    }

    public void Use2Me(Me me)
    {
        me.Method1();
        me.Method2();
        me.Value1 = 0;
        me.Value2 = 0;
    }

    public void Use3Me(Me me)
    {
        me.Method1();
        /*EA_METH_001*/ me.Method2();
        me.Value1 = 0;
        me.Value2 = 0;
    }

    public void Use4Me(Me me)
    {
        me.Method1();
        var x = /*EA_PROP_001*/ me.Value1;
        /*EA_PROP_002*/ me.Value2 = 0;
    }

    public void Use5Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        var x = /*EA_PROP_001*/ me.Value1;
    }

    [Alias(Me.Alias1)]
    public void AUse1Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        me.Method3();
        var x = /*EA_PROP_001*/ me.Value1;
        /*EA_PROP_002*/ me.Value2 = 0;
        x = me.Value2;
        me.Value3 = 0;
    }

    [Alias(Me.Alias2)]
    public void AUse2Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        me.Method3();
        var x = /*EA_PROP_001*/ me.Value1;
        me.Value3 = 0;
    }

    [Alias("Alias3")]
    public void AUse3Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        /*EA_METH_001*/ me.Method3();
        /*EA_PROP_002*/ me.Value3 = 0;
        var x = me.Value3;
    }

    public void Use1Me2(Me2 me2)
    {
        me2.Method1();
        me2.Value1 = 0;
    }

    public void Use2Me2(Me2 me2)
    {
        me2.Method1();
        me2.Value1 = 0;
    }

    public void Use3Me2(Me2 me2)
    {
        /*EA_TYPE_001*/ me2.Method1();
        var x = /*EA_TYPE_001*/ me2.Value1;
    }
}

file class Friend1
{
    public void Use1Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        me.Method2();
        var x = /*EA_PROP_001*/ me.Value1;
    }

    public void Use2Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        me.Method2();
        var x = /*EA_PROP_001*/ me.Value1;
    }

    public void Use3Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        /*EA_METH_001*/ me.Method2();
        var x = /*EA_PROP_001*/ me.Value1;
    }

    [Alias(Me.Alias1)]
    public void AUse1Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        /*EA_METH_001*/ me.Method3();
        var x = /*EA_PROP_001*/ me.Value1;
    }

    public void Use1Me2(Me2 me2)
    {
        me2.Method1();
         me2.Value1 = 0;
    }

    public void Use2Me2(Me2 me2)
    {
        me2.Method1();
        me2.Value1 = 0;
    }

    public void Use3Me2(Me2 me2)
    {
        /*EA_TYPE_001*/ me2.Method1();
        var x = /*EA_TYPE_001*/ me2.Value1;
    }
}

file class NotFriend
{
    public void Use1Me(Me me)
    {
        /*EA_METH_001*/ me.Method1();
        /*EA_METH_001*/ me.Method2();
        var x = /*EA_PROP_001*/ me.Value1;
        /*EA_PROP_002*/ me.Value3 = 0;
        x = me.Value3;
    }

    public void Use1Me2(Me2 me2)
    {
        /*EA_TYPE_001*/ me2.Method1();
        var x = /*EA_TYPE_001*/ me2.Value1;
    }
}