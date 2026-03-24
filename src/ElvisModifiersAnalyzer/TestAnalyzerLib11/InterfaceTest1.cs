namespace TestAnalyzerLib11;

file class Me
{
    public const string AcceptOne = nameof(AcceptOne);
    public decimal Money { get; private set; } = 100;

    [OnlyAlias<IFriend>(AcceptOne)]
    //[OnlyYou<IFriend>]
    //[OnlyYou<IFriend>(nameof(IFriend.AcceptMoney))]
    //[OnlyYou<BestFriend>]
    public decimal TakeMyHalfMoney()
    {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }

    public void SelfAcceptMoney() => Money += TakeMyHalfMoney(); // ok


    [OnlyYouSet<IFriend>]
    //[OnlyAliasSet<IFriend>]
    //[OnlyYouSet<BestFriend>]
    public decimal Money2 { get; set; } = 100;

}

file interface IFriend
{
    [Alias(Me.AcceptOne)]
    void AcceptMoney(Me me);
    //{
    //    me.TakeMyHalfMoney();
    //}
}

file class BestFriend : IFriend
{
    public void AcceptMoney(Me me)
    {
        me.TakeMyHalfMoney();
        me.Money2 = 0;
        var money2 = me.Money2;
    }
}


file class BestFriend2 : IFriend
{
    public void AcceptMoney(Me me)
    {
        me.TakeMyHalfMoney();
        me.Money2 = 0;
        var money2 = me.Money2;
    }
}

file class NoFriend
{
    public void AcceptMoney(Me me)
    {
        /*EA_METH_001*/ me.TakeMyHalfMoney();
        /*EA_PROP_002*/ me.Money2 = 0;
        var money2 = me.Money2;
    }
}
