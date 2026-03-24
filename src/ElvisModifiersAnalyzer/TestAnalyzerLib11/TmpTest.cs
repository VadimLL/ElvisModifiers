namespace TestAnalyzerLib11;



[OnlyYouSet<Friend3>]
[OnlyYou<Friend1>]
//[OnlyYouSet<Friend3>(nameof(Friend3.Test))]
file class Me
{
    public const string AcceptMul = nameof(AcceptMul);

    [OnlyAlias<Friend>(AcceptMul)]
    public void TakeMyHalfMoney() { }

    public int Value1 { get; set; }
}

file class Friend
{
    //[Alias(Me.AcceptOne)]
    public void AcceptMoney(in Me me)
    {
        me.TakeMyHalfMoney(); // err
    }

    [Alias(Me.AcceptMul)]
    public void AcceptMoney(in Me me, int mul)
    {
        me.TakeMyHalfMoney(); // ok
    }
}



file class Friend1 { }

file class Friend3
{
    public void Test(Me me)
    {
        me.Value1 = 0;
    }
}


/*
[OnlyYou<Friend1>]
[OnlyYouSet<Friend3>]
[OnlyYouSet<Friend3>(nameof(Friend3.Test))]
file class Me
{

    //[ExcludeMembers<Friend3>(nameof(Friend3.Test), "Qqq")]
    public void Method1() { }

    public int Value1 { get; set; }
}

file class Friend1 { }

file class Friend3
{
    public void Test(Me me)
    {
        me.Value1 = 0;
        //me.Method1();
    }
}

//file class Friend2
//{
//    void Test(Me me)
//    {
//        me.Value1 = 0;
//        me.Method1(); // err
//    }
//}


*/









//[OnlyYou<Friend1>]
//[OnlyYou<Friend2>]
//[OnlyYouSet<Friend3>]
//file class Me
//{
//    //[OnlyYouSet<Friend3>]
//    public int Value { get; set; }

//    //[OnlyYou<Friend2>]
//    public void Method1() { }

//    [Exclude]
//    public void Method10() { }

//    [Exclude<Friend2>]
//    public void Method11() { }
//}

//file class Friend1
//{
//    void Test(Me me) // all ok
//    {
//        me.Value = 0;
//        me.Method1();

//        me.Method10();
//        me.Method11();
//    }
//}

//file class Friend2
//{
//    void Test(Me me)
//    {
//        me.Value = 0; // ok
//        me.Method1(); // ok

//        me.Method10(); // ok
//        me.Method11(); // err due to [Exclude<Friend2>]
//    }
//}

//file class Friend3 { }