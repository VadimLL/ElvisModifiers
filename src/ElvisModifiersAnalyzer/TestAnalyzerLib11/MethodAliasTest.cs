namespace TestAnalyzerLib11;

file class Me
{
    public const string AcceptMul = nameof(AcceptMul);
    public const string AcceptOne = nameof(AcceptOne);
    public decimal Money { get; private set; } = 100;

    [OnlyAlias<MyFriend>(AcceptMul, AcceptOne)]
    //[OnlyYou<MyFriend>(nameof(MyFriend.AcceptMoney))]
    public decimal TakeMyHalfMoney() {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }
}

file class MyFriend
{
    //[Alias(Me.AcceptOne)]
    public void AcceptMoney(in Me me)
        => Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err

    [Alias(Me.AcceptMul)]
    public void AcceptMoney(in Me me, int mul)
        => Money += mul * me.TakeMyHalfMoney(); // ok

    [Alias(Me.AcceptMul)]
    public void AcceptMoney(in Me me, decimal mul)
        => Money += mul * me.TakeMyHalfMoney(); // ok

    public decimal Money { get; private set; } = -40;
}
