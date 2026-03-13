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

    [OnlyYou<MyFriend>(nameof(MyFriend.AcceptMoney))]
    public decimal this[int index]
    {
        get => Money + index;
        set => Money = value - index;
    }

    public void SelfAcceptMoney() => Money += TakeMyHalfMoney(); // ok
}

file class MyFriend
{
    public void AcceptMoney(in Me me)
    {
        Money += me.TakeMyHalfMoney(); // ok
        var m0 = me[0];
    }
    public void CantAcceptMoney(in Me me)
    { 
        //Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
        var m0 = /*EA_METH_001*/ me[0];
        /*EA_METH_001*/ me[0] = 0;
    }
    public void AcceptMoneyFromNotMe(in NotMe me)
    {
        Money += me.TakeMyMoney(); // ok
    }
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
