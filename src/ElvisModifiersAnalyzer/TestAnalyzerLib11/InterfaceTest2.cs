namespace TestAnalyzerLib11;

file interface IMe
{
    //[OnlyYou<IFriend>]
    //[OnlyYou<Friend>]
    //[OnlyYou<Friend>(nameof(IFriend.AcceptMoney))]
    [OnlyYou<IFriend>(nameof(IFriend.AcceptMoney))]
    void TakeMoney();
}

file class Me1 : IMe
{
    //[OnlyYou<IFriend>]
    //[OnlyYou<Friend>]
    public void TakeMoney() { }
}

file class Me2 : IMe
{
    //[OnlyYou<IFriend>]
    //[OnlyYou<Friend>]
    public void TakeMoney() { }
}


file interface IFriend
{
    void AcceptMoney(Me1 me);
}

file class Friend : IFriend
{
    public void AcceptMoney(Me1 me)
    {
        me.TakeMoney(); // ok
    }

    public void AcceptMoney(Me2 me2)
    {
        me2.TakeMoney(); // ok
    }

    public void AcceptMoney1(Me2 me2)
    {
        /*EA_METH_001*/ me2.TakeMoney(); // err
    }
}

file class NoFriend
{
    public void AcceptMoney(Me1 me)
    {
        /*EA_METH_001*/ me.TakeMoney(); // err
    }

    public void AcceptMoney(Me2 me2)
    {
        /*EA_METH_001*/ me2.TakeMoney(); // err
    }
}