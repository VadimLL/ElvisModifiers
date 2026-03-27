namespace TestAnalyzerLib11;

[OnlyYou<Friend1>]
[OnlyYou<Friend2>]
[OnlyYou<Friend3>(nameof(Friend3.Test1))]
file class Me
{
    public int Value1 { get; set; }

    [Exclude<Friend2>]
    public int Value2 { get; set; }

    [Exclude<Friend3>]
    public int Value3 { get; set; }


    public void Method1() { }
    // ...
    public void Method9() { }

    [Exclude]
    public void Method10() { }

    [Exclude<Friend2>]
    public void Method11() { }

    [Exclude<Friend3>]
    public void Method12() { }
}

file class Friend1
{
    void Test(Me me) // all ok
    {
        me.Value1 = 0;
        me.Value2 = 0;

        me.Method1();
        // ...
        me.Method9();

        me.Method10();
        me.Method11();
        me.Method12();
    }
}

file class Friend2
{
    void Test(Me me)
    {
        me.Value1 = 0;
        /*EA_TYPE_001*/ me.Value2 = 0; // err due to [Exclude<Friend2>]

        me.Method1();
        // ...
        me.Method9();

        me.Method10();
        /*EA_TYPE_001*/ me.Method11(); // err due to [Exclude<Friend2>]
        me.Method12();
    }
}

file class Friend3
{
    public void Test1(Me me)
    {
        me.Value1 = 0;
        me.Value2 = 0;

        me.Method1();
        // ...
        me.Method9();

        me.Method10(); // ok due to [Exclude]
        me.Method11();
        /*EA_TYPE_001*/ me.Method12(); // err due to [Exclude<Friend3>]
    }
}

file class NoFriend
{
    void Test(Me me)
    {
        /*EA_TYPE_001*/ me.Value1 = 0;
        /*EA_TYPE_001*/ me.Value2 = 0;

        /*EA_TYPE_001*/ me.Method1();
        // ...
        /*EA_TYPE_001*/ me.Method9();

        me.Method10(); // ok due to [Exclude]
        /*EA_TYPE_001*/ me.Method11();
        /*EA_TYPE_001*/ me.Method12();
    }
}
