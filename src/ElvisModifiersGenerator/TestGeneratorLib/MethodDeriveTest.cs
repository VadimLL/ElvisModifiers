namespace TestGeneratorLib;

file class Me
{
    public decimal Money { get; private set; } = 100;

    [OnlyYou(typeof(MyFriend), nameof(MyFriend.AcceptMoney))]
    virtual public decimal TakeMyHalfMoney()
    {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }

    [OnlyYou(typeof(MyFriend), nameof(MyFriend.AcceptMoney2))]
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
        base.TakeMyHalfMoney2(); // maybe need err? !!!
        TakeMyHalfMoney2(); // not emplemented !!! is it necessary?
        return base.TakeMyHalfMoney(); // ok
    }
}

file class MyFriend
{
    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // err

    public void AcceptMoney2(in Me2 me2) => Money += me2.TakeMyHalfMoney2(); // ok
    public void CantAcceptMoney2(in Me2 me2) => Money += me2.TakeMyHalfMoney2(); // err
    public decimal Money { get; private set; } = -40;
}
