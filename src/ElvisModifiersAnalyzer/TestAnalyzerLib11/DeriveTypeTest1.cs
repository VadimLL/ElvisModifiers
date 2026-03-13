namespace TestAnalyzerLib11;

file class Me
{
    public decimal Money { get; private set; } = 100;

    [OnlyYou<MyFriend>(nameof(MyFriend.AcceptMoney))]
    public decimal Money2 { get; set; } = 100;

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

    void selfMethod() // all ok
    {
        var m2 = Money2 = this.Money2;
        Money2 = this.Money2 = 0;
        ++Money2;
        ++this.Money2;

        TakeMyHalfMoney2();
        TakeMyHalfMoney();
    }
}

file class Me2 : Me
{
    // all ok, but maybe need err? !!! see #define STRICT_OU in the ElvisModifiersAnalyzer.cs
    public override decimal TakeMyHalfMoney()
    {
        var m2 = Money2 = this.Money2;
        Money2 = 0;
        ++Money2;
        ++this.Money2;

        base.TakeMyHalfMoney2();
        TakeMyHalfMoney2();
        return base.TakeMyHalfMoney();
    }

    // all ok, but maybe need err? !!! see #define STRICT_OU in the ElvisModifiersAnalyzer.cs
    void otherMethod()
    {
        var m2 = Money2 = this.Money2;
        Money2 = 0;

        base.TakeMyHalfMoney2();
        TakeMyHalfMoney2();
        base.TakeMyHalfMoney();
    }
}

file class MyFriend
{
    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void AcceptMoney(in Me2 me2) => Money += me2.TakeMyHalfMoney(); // ok

    public void AcceptMoney2(in Me me) => Money += me.TakeMyHalfMoney2(); // ok
    public void AcceptMoney2(in Me2 me2) => Money += me2.TakeMyHalfMoney2(); // ok

    public void CantAcceptMoney(in Me me)
        => Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
    public void CantAcceptMoney2(in Me2 me2)
        => Money += /*EA_METH_001*/ me2.TakeMyHalfMoney2(); // err
    public decimal Money { get; private set; } = -40;
}
