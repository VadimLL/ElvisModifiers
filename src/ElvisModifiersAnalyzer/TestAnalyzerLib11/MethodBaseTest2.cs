namespace TestAnalyzerLib11;

file class Me
{
    public decimal Money { get; private set; } = 100;

    [OnlyYou<MyFriend1>(nameof(MyFriend1.AcceptMoney))]
    [OnlyYou<MyFriend2>(nameof(MyFriend2.AcceptMoney))]
    public decimal TakeMyHalfMoney() {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }
}

file class MyFriend1
{
    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney(in Me me)
        => Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
    public decimal Money { get; private set; } = -40;
}

file class MyFriend2
{
    Me me2 = new Me();

    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney1(in Me me)
        => Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
    public void CantAcceptMoney2()
        => Money += /*EA_METH_001*/ me2.TakeMyHalfMoney(); // err
    public decimal Money { get; private set; } = -40;
}
