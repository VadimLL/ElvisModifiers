namespace TestAnalyzerLib11;

[OnlyYou<Friend1>]
[OnlyYouSet<Friend2>("Test1")]
file class Me
{
    public const string Test2Alias = nameof(Test2Alias);

    public int Value1 { get; set; }

    [OnlyAliasSet<Friend2>(Test2Alias)]
    public int Value2 { get; set; }
}

file class Friend1
{
    public void Test1(in Me me)
    {
        var v = me.Value1;
        /*EA_TYPE_002*/ me.Value1 = 0;

        v = me.Value2;
        /*EA_TYPE_002*/ me.Value2 = 0;
    }

    [Alias(Me.Test2Alias)]
    public void Test2(in Me me)
    {
        var v = me.Value1;
        /*EA_TYPE_002*/ me.Value1 = 0;

        v = me.Value2;
        /*EA_TYPE_002*/ me.Value2 = 0;
    }
}

file class Friend2
{
    public void Test1(in Me me)
    {
        var v = me.Value1;
        me.Value1 = 0;

        v = me.Value2;
        /*EA_PROP_002*/ me.Value2 = 0;
    }

    [Alias(Me.Test2Alias)]
    public void Test2(in Me me)
    {
        var v = /*EA_TYPE_001*/ me.Value1;
        /*EA_TYPE_002*/ me.Value1 = 0;

        v = /*EA_TYPE_001*/ me.Value2;
        /*EA_TYPE_002*/ me.Value2 = 0;
    }
}

file class NoFriend
{
    public void Test1(in Me me)
    {
        var v = /*EA_TYPE_001*/ me.Value1;
        /*EA_TYPE_002*/ me.Value1 = 0;

        v = /*EA_TYPE_001*/ me.Value2;
        /*EA_TYPE_002*/ me.Value2 = 0;
    }
}
