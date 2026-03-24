namespace TestAnalyzerLib11;

file class Me
{
    [OnlyYou<Friend>(nameof(Friend.CreateMe))]
    public Me(decimal money) { }

    public Me CreateNew(decimal money) => new Me(money); // ok
    public static Me Create100() => new Me(100); // ok
    public static Me Instance0 => new Me(0); // ok
}

file class Friend
{
    public void CreateMe()
    {
        var me = new Me(100); // ok
        Me me2 = new (100); // ok
    }

    public void CannotCreateMe()
    {
        var me = /*EA_METH_001*/ new Me(100); // err
        Me me2 = /*EA_METH_001*/ new(100); // err
    }
}

file class NoFriend
{
    public void CreateMe()
    {
        var me = /*EA_METH_001*/ new Me(100); // err
        Me me2 = /*EA_METH_001*/ new(100); // err
    }
}
