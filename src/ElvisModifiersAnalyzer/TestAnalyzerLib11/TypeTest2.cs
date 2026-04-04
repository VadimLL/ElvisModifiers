namespace TestAnalyzerLib11;

[OnlyAlias<Friend>(InvokeMul)]
file class Me
{
    public const string InvokeMul = nameof(InvokeMul);
    public const string InvokeOne = nameof(InvokeOne);

    [OnlyAlias<Friend>(InvokeOne)]
    public int Value { get; set; } = 100;

    public void Method1() { }
    public void Method2() { }
}

file class Friend
{
    [Alias(Me.InvokeMul)]
    public void CanInvoke1(in Me me, int mul)
    {
        me.Method1(); // ok
        var m = /*EA_PROP_001*/ me.Value;
    }
    public void CanInvoke2(in Me me) => /*EA_TYPE_001*/ me.Method2(); // err
    public void CantInvoke(in Me me) => /*EA_TYPE_001*/ me.Method1(); // err

    [Alias(Me.InvokeOne)]
    public void SomeSet3(in Me me) => /*EA_TYPE_001*/ me.Value++; // err
}

file class NoFriend
{
    public void Some1(in Me me) => /*EA_TYPE_001*/ me.Method1(); // err
    public void Some2(in Me me) => /*EA_TYPE_001*/ me.Method2(); // err
    public void SomeSet1(in Me me) => /*EA_TYPE_001*/ me.Value = 0; // err
    public void SomeSet2(in Me me) => /*EA_TYPE_001*/ ++me.Value; // err
}
