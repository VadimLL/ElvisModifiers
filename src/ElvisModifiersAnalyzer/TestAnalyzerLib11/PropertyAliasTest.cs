namespace TestAnalyzerLib11;

file class Me
{
    public const string AcceptMul = nameof(AcceptMul);
    public const string AcceptOne = nameof(AcceptOne);
    public const string AcceptGlobalMul = nameof(AcceptGlobalMul);

    [OnlyAlias<Friend>(AcceptGlobalMul)]
    public static decimal GlobalMoney { get; set; } = 100;

    //[OnlyAlias<Friend>(AcceptOne)]
    [OnlyAlias<Friend>(AcceptMul)]
    public decimal Money { get; set; } = 100;
}

file class Friend
{
    //[Alias(Me.AcceptOne)]
    public void AcceptMoney(in Me me) => /*EA_PROP_002*/ me.Money = 0; // err

    [Alias(Me.AcceptMul)]
    public void AcceptMoney(in Me me, int mul) => me.Money = 0;  // ok

    [Alias(Me.AcceptGlobalMul)]
    public void AcceptGlobalMoney(int mul) => Me.GlobalMoney = 0;  // ok
    public void AcceptGlobalMoney() => /*EA_PROP_002*/ Me.GlobalMoney = 0;  // err
}
