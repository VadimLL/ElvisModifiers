namespace TestAnalyzerLib11;

[OnlyYou<Friend>]
file class Me
{
    public int Value { get; set; }

    public void Method1() { }

    [OnlyYou<Friend>(nameof(Friend.CanInvoke1))]
    public void Method2() { }
    
    [OnlyYou<NoFriend>(nameof(Friend.CanInvoke1))]
    [OnlyYou<NoFriend>(nameof(NoFriend.Some1))] // no effect !!! prohibit the attribute with a rule? 
    public void Method3() { }
}

[OnlyYou<Friend>(nameof(Friend.UseMe2))]
file class Me2
{
    public int Value { get; set; }
    public void Method1() { }
}


file class Friend
{
    public void CanInvoke1(in Me me) => me.Method2(); // ok
    public void CanInvoke2(in Me me) => me.Method1(); // ok
    public void CanSet(in Me me) => me.Value = 0; // ok
    public void CantInvoke(in Me me) => /*EA_METH_001*/ me.Method2(); // err
    public void CantInvoke3(in Me me) => /*EA_METH_001*/ me.Method3(); // err??? !!!

    public void UseMe2(in Me2 me2)
    {
        me2.Method1(); // ok
        me2.Value = 0; // ok
    }
    public void CantUseMe2(in Me2 me2)
    {
        /*EA_TYPE_001*/ me2.Method1(); // err
        /*EA_TYPE_001*/ me2.Value = 0; // err
    }
}

file class NoFriend
{
    public void Some1(in Me me) => /*EA_TYPE_001*/ me.Method1(); // err
    public void Some2(in Me me) => /*EA_TYPE_001*/ me.Method2(); // err
    public void Some3(in Me me) => /*EA_TYPE_001*/ me.Method3(); // err
    public void SomeSet1(in Me me) => /*EA_TYPE_001*/ me.Value = 0; // err
    public void SomeSet2(in Me me) => /*EA_TYPE_001*/ ++me.Value; // err
    public void SomeSet3(in Me me) => /*EA_TYPE_001*/ me.Value++; // err
}
