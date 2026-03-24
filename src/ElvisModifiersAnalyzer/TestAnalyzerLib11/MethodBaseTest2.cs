namespace TestAnalyzerLib11;

file class Me
{
    public decimal Money { get; private set; } = 100;

    [OnlyYou<Friend1>(nameof(Friend1.AcceptMoney)
                    , nameof(Friend1.Property)
                    , nameof(Friend1.Field))]
    [OnlyYou<Friend2>(nameof(Friend2.AcceptMoney))]
    public decimal TakeMyHalfMoney() {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }
}

file class Friend1
{
    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney(in Me me)
        => Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
    public decimal Money { get; private set; } = -40;


    private Me _me = null!;

    static Me _Me = null!;

    public Friend1()
    {
        /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
        /*EA_METH_001*/ _Me.TakeMyHalfMoney(); // err
    }

    public void CantAcceptMoney1()
    {
        var m = /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
        /*EA_METH_001*/ _Me.TakeMyHalfMoney(); // err
    }

    private decimal _field = /*EA_METH_001*/ _Me.TakeMyHalfMoney(); // err

    public decimal Field = _Me.TakeMyHalfMoney(); // ok

    public decimal Property
    {
        get {
            var m = _me.TakeMyHalfMoney(); // ok
            m = _Me.TakeMyHalfMoney(); // ok
            return 0;
        }            
        set {
            var m = _me.TakeMyHalfMoney(); // ok
            m = _Me.TakeMyHalfMoney(); // ok
        }
    }

    public decimal Property1
    {
        get {
            var m = /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
            m = /*EA_METH_001*/ _Me.TakeMyHalfMoney(); // err
            return 0;
        }
        set {
            var m = /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
            m = /*EA_METH_001*/ _Me.TakeMyHalfMoney(); // err
        }
    }

    public decimal this[int index]
    {
        get {
            var m = /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
            m = /*EA_METH_001*/ _Me.TakeMyHalfMoney(); // err
            return 0;
        }
        set {
            var m = /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
            m = /*EA_METH_001*/ _Me.TakeMyHalfMoney(); // err
        }
    }
}

file class Friend2
{
    Me me2 = new Me();

    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney1(in Me me)
        => Money += /*EA_METH_001*/ me.TakeMyHalfMoney(); // err
    public void CantAcceptMoney2()
        => Money += /*EA_METH_001*/ me2.TakeMyHalfMoney(); // err
    public decimal Money { get; private set; } = -40;


    private Me _me = null!;

    public void CantAcceptMoney1()
    {
        var m = /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
    }

    private decimal _Property;
    public decimal Property
    {
        get {
            var m = /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
            return _Property;
        }
        set {
            var m = /*EA_METH_001*/ _me.TakeMyHalfMoney(); // err
            _Property = value;
        }
    }
}
