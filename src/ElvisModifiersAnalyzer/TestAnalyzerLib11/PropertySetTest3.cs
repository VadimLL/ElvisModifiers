namespace TestAnalyzerLib11;

file class Me
{
    public const string Test2Alias = nameof(Test2Alias);

    [OnlyYou<Friend1>]
    [OnlyYouSet<Friend2>("Test1")]
    public int Value1 { get; set; }

    [OnlyYou<Friend1>]
    [OnlyAliasSet<Friend2>(Test2Alias)]
    public int Value2 { get; set; }

    [OnlyYou<Friend1>("Test2")]
    [OnlyYouSet<Friend2>("Test1")]
    public int Value3 { get; set; }

}

file class Friend1
{
    public void Test1(in Me me)
    {
        var v = me.Value1;
        /*EA_PROP_002*/ me.Value1 = 0;

        v = me.Value2;
        /*EA_PROP_002*/ me.Value2 = 0;

        v = /*EA_PROP_001*/ me.Value3;
        /*EA_PROP_002*/ me.Value3 = 0;
    }

    [Alias(Me.Test2Alias)]
    public void Test2(in Me me)
    {
        var v = me.Value1;
        /*EA_PROP_002*/ me.Value1 = 0;

        v = me.Value2;
        /*EA_PROP_002*/ me.Value2 = 0;

        v = me.Value3;
        /*EA_PROP_002*/ me.Value3 = 0;
    }
}

file class Friend2
{
    public void Test1(in Me me)
    {
        var v = me.Value1;
        me.Value1 = 0;

        v = /*EA_PROP_001*/ me.Value2;
        /*EA_PROP_002*/ me.Value2 = 0;

        v = me.Value3;
        me.Value3 = 0;
    }

    [Alias(Me.Test2Alias)]
    public void Test2(in Me me)
    {
        var v = /*EA_PROP_001*/ me.Value1;
        /*EA_PROP_002*/ me.Value1 = 0;

        v = me.Value2;
        me.Value2 = 0;

        v = /*EA_PROP_001*/ me.Value3;
        /*EA_PROP_002*/ me.Value3 = 0;
    }
}

file class NoFriend
{
    public void Test1(in Me me)
    {
        var v = /*EA_PROP_001*/ me.Value1;
        /*EA_PROP_002*/ me.Value1 = 0;

        v = /*EA_PROP_001*/ me.Value2;
        /*EA_PROP_002*/ me.Value2 = 0;

        v = /*EA_PROP_001*/ me.Value3;
        /*EA_PROP_002*/ me.Value3 = 0;
    }
}
