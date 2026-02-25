namespace TestAnalyzerLib11;

file class Me
{
    public decimal Money { get; private set; } = 100;
    public decimal UnsharedMoney { get; private set; } = 100_000;

    [OnlyYou<MyFriend>(nameof(MyFriend.AcceptMoney))]
    [OnlyYou<MyFriend2>]
    public decimal TakeMyHalfMoney() {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }

    public void SelfAcceptMoney() => Money += TakeMyHalfMoney(); // ok
}

file class MyFriend
{
    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney(in Me me)
        => Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
    public void AcceptMoneyFromNotMe(in NotMe me) => Money += me.TakeMyMoney(); // ok
    public decimal Money { get; private set; } = -40;
}

file class MyFriend2
{
    public void AcceptMoney1(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void AcceptMoney2(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public decimal Money { get; private set; } = -40;
}


file class NotMe // Loh :)
{
    public decimal TakeMyMoney() => 1000;
    //...
}
