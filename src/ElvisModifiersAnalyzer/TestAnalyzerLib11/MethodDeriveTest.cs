namespace TestAnalyzerLib11;

file class Me
{
    public decimal Money { get; private set; } = 100;

    [OnlyYou<MyFriend>(nameof(MyFriend.AcceptMoney))]
    virtual public decimal TakeMyHalfMoney()
    {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }

    [OnlyYou<MyFriend>(nameof(MyFriend.AcceptMoney2))]
    public decimal TakeMyHalfMoney2()
    {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }
}

file class Me2 : Me
{
    public override decimal TakeMyHalfMoney()
    {
        base.TakeMyHalfMoney2(); // ok, but maybe need err? !!!
        /*EA_METH_001*/ TakeMyHalfMoney2(); // err? not emplemented !!! is it necessary?
        return base.TakeMyHalfMoney(); // ok
    }
}

file class MyFriend
{
    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney(in Me me)
        => Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
    public void AcceptMoney2(in Me2 me2) => Money += me2.TakeMyHalfMoney2(); // ok
    public void CantAcceptMoney2(in Me2 me2)
        => Money += /*EA_METH_001*/ me2.TakeMyHalfMoney2(); // err
    public decimal Money { get; private set; } = -40;
}
