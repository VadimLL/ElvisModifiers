namespace TestAnalyzerLib11;

file interface IMe
{    
    [OnlyYou<Friend>(nameof(Friend.AcceptMoney))]
    decimal TakeMyHalfMoney();

    [OnlyYou<Friend>(nameof(Friend.AcceptMoney))]
    static void StaticMethod() { }
}

file class Me : IMe
{
    public decimal Money { get; private set; } = 100;
    public decimal TakeMyHalfMoney()
    {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }
}

file class Friend
{
    public void AcceptMoney(in Me me)
    {
        Money += me.TakeMyHalfMoney(); // ok
        IMe.StaticMethod(); // ok
    }
    public void CantAcceptMoney(in Me me)
    {
        Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
        /*EA_METH_001*/ IMe.StaticMethod(); // err
    }
    public decimal Money { get; private set; } = -40;
}
