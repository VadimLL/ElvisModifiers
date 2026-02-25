namespace TestAnalyzerLib11;

[OnlyYou<MyFriend>]
file class Me
{
    public decimal Money { get; set; } = 100;

    public void Method1() { }

    [OnlyYou<MyFriend>(nameof(MyFriend.CanInvoke1))]
    public void Method2() { }
    
    [OnlyYou<NotMyFriend>(nameof(MyFriend.CanInvoke1))]
    [OnlyYou<NotMyFriend>(nameof(NotMyFriend.Some1))] // no effect !!! prohibit the attribute with a rule? 
    public void Method3() { }
}

file class MyFriend
{
    public void CanInvoke1(in Me me) => me.Method2(); // ok
    public void CanInvoke2(in Me me) => me.Method1(); // ok
    public void CanSet(in Me me) => me.Money = 0; // ok
    public void CantInvoke(in Me me) => /*EA_METH_001*/ me.Method2(); // err
    public void CantInvoke3(in Me me) => /*EA_METH_001*/ me.Method3(); // err??? !!!
}

file class NotMyFriend
{
    public void Some1(in Me me) => /*EA_TYPE_001*/ me.Method1(); // err
    public void Some2(in Me me) => /*EA_TYPE_001*/ me.Method2(); // err
    public void Some3(in Me me) => /*EA_TYPE_001*/ me.Method3(); // err
    public void SomeSet1(in Me me) => /*EA_TYPE_001*/ me.Money = 0; // err
    public void SomeSet2(in Me me) => /*EA_TYPE_001*/ ++me.Money; // err
    public void SomeSet3(in Me me) => /*EA_TYPE_001*/ me.Money++; // err
}
