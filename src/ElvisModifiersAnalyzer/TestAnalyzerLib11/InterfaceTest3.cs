namespace TestAnalyzerLib11;

[OnlyYou<Friend1>]
file interface IM
{
    void Method1();
}

file interface IMe : IM
{
    [OnlyYou<IFriend1>]
    public void Method2();

    [OnlyYou<IFriend2>]
    public void Method3();
}

file interface IMe2 : IM
{
    [OnlyYou<IFriend2>]
    public void Method4();
}


file class Me1 : IMe
{    
    public void Method1() { }
    public void Method2() { }
    public void Method3() { }
}

file class Me2 : IMe
{
    public void Method1() { }

    [Exclude]
    public void Method2() { }
    public void Method3() { }
}


file interface IFriend1
{
    void UseMe1(Me1 me);
}

file interface IFriend2
{
    void Use2Me1(Me1 me);
}

file class Friend1 : IFriend1
{
    public void UseMe(IMe me)
    {
        me.Method1();
        me.Method2();
        /*EA_METH_001*/ me.Method3();
    }
    public void UseMe1(Me1 me)
    {
        me.Method1();
        me.Method2();
        /*EA_METH_001*/ me.Method3();
    }
    public void UseMe2(Me2 me)
    {
        me.Method1();
        me.Method2();
        /*EA_METH_001*/ me.Method3();
    }
}

file class Friend2 : IFriend1, IFriend2
{
    public void UseMe1(Me1 me)
    {
        /*EA_TYPE_001*/ me.Method1();
        me.Method2();
        me.Method3();
    }
    public void Use2Me1(Me1 me)
    {
        /*EA_TYPE_001*/ me.Method1();
        me.Method2();
        me.Method3();
    }

    public void UseMe2(Me2 me)
    {
        /*EA_TYPE_001*/ me.Method1();
        me.Method2();
        me.Method3();
    }
}

file class NoFriend
{
    public void UseMe(IMe me)
    {
        /*EA_TYPE_001*/ me.Method1();
        /*EA_METH_001*/ me.Method2();
        /*EA_METH_001*/ me.Method3();
    }
    public void UseMe1(Me1 me)
    {
        /*EA_TYPE_001*/ me.Method1();
        /*EA_METH_001*/ me.Method2();
        /*EA_METH_001*/ me.Method3();
    }
    public void UseMe2(Me2 me)
    {
        /*EA_TYPE_001*/ me.Method1();
        me.Method2();
        /*EA_METH_001*/ me.Method3();
    }
}