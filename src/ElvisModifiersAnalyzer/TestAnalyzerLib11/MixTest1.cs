namespace TestAnalyzerLib11;

[OnlyYouSet<Friend>]
file class Me
{
    [OnlyYou<Friend>]
    public Me() { }

    public int Value { get; set; }

    [OnlyYouSet<Friend>(nameof(Friend.UseMe))]
    public int Value2 { get; set; }

    [OnlyYou<Friend>]
    public void Method() { }
}

file class Friend
{
    public void UseMe(Me me)
    {
        var m = new Me();

        me.Value = 0;
        me.Value2 = 0;
        me.Method();
    }

    public void Use2Me(Me me)
    {
        var m = new Me();

        me.Value = 0;
        /*EA_PROP_002*/ me.Value2 = 0;
        me.Method();
    }

}

file class NoFriend
{
    public void UseMe(Me me)
    {
        var m = /*EA_METH_001*/ new Me();
        /*EA_TYPE_002*/ me.Value = 0;
        /*EA_METH_001*/ me.Method();
    }
}