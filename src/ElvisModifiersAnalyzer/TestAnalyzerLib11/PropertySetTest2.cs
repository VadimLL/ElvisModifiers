namespace TestAnalyzerLib11;

[OnlyYouSet<MyFriend1>]
file class Me1
{
    public decimal Money { get; set; } = 100;
}

[OnlyAliasSet<MyFriend2>(SetMoney)]
file class Me2
{
    public const string SetMoney = nameof(SetMoney);
    public decimal Money { get; set; } = 100;
}


file class MyFriend1
{
    public void SetMoney(in Me1 me) // ok
    {
        var half = (me.Money) / 2;
        me.Money = half;
        me.Money += 100;
        ++me.Money;
        me.Money++;
    }

    public void SetMoney(in Me2 me)
    {
        var half = (me.Money) / 2;  // ok
        /*EA_TYPE_002*/ me.Money = half; // err
        /*EA_TYPE_002*/ me.Money += 100; // err
        /*EA_TYPE_002*/ ++me.Money; // err
        /*EA_TYPE_002*/ me.Money++; // err
    }

}

file class MyFriend2
{
    [Alias(Me2.SetMoney)]
    public void SetMoney(in Me2 me) // ok
    {
        var half = (me.Money) / 2;
        me.Money = half;
        me.Money += 100;
        ++me.Money;
        me.Money++;
    }
}



file class NotMyFriend
{
    public void SetMoney(in Me1 me)
    {
        var half = me.Money / 2;
        /*EA_TYPE_002*/ me.Money = half;
    }

    public void SetMoney(in Me2 me)
    {
        var half = me.Money / 2;
        /*EA_TYPE_002*/ me.Money = half;
    }
}
