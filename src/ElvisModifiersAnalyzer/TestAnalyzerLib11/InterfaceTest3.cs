namespace TestAnalyzerLib11;

[OnlyYou<Friend>]
file interface IM
{
    void Method1();
}

[OnlyYou<Friend>]
file interface IMe : IM
{
    public void Method2() { }
}

file class Me1 : IMe
{    
    public void Method1() { }
    public void Method2() { }
}

file class Me2 : IMe
{
    public void Method1() { }

    [Exclude]
    public void Method2() { }
}


file interface IFriend
{
    void UseMe1(Me1 me);
}

file class Friend : IFriend
{
    public void UseMe(IMe me)
    {
        me.Method1(); // ok
        me.Method2();
    }
    public void UseMe1(Me1 me)
    {
        me.Method1(); // ok
        me.Method2();
    }
    public void UseMe2(Me2 me)
    {
        me.Method1();
        me.Method2();
    }
}

file class NoFriend
{
    public void UseMe(IMe me)
    {
        /*EA_TYPE_001*/ me.Method1();
        /*EA_TYPE_001*/ me.Method2();
    }
    public void UseMe1(Me1 me)
    {
        /*EA_TYPE_001*/ me.Method1();
        /*EA_TYPE_001*/ me.Method2();
    }
    public void UseMe2(Me2 me)
    {
        /*EA_TYPE_001*/ me.Method1();
        me.Method2();
    }
}