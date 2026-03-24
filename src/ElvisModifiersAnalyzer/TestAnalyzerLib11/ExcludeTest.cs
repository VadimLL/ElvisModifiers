namespace TestAnalyzerLib11;

[OnlyYou<Friend1>]
[OnlyYou<Friend2>]
[OnlyYouSet<Friend3>(nameof(Friend3.Test))]
//[OnlyYouSet<Friend3>]
file class Me
{
    //[OnlyYouSet<Friend3>]
    public int Value1 { get; set; }

    [Exclude<Friend2>]
    public int Value2 { get; set; }


    public void Method1() { }
    // ...
    public void Method9() { }

    [Exclude]
    public void Method10() { }

    [Exclude<Friend2>]
    public void Method11() { }
}

file class Friend1
{
    void Test(Me me) // all ok
    {
        me.Value1 = 0;
        me.Value2 = 0;

        me.Method1();
        // ...
        me.Method9();

        me.Method10();
        me.Method11();
    }
}

file class Friend2
{
    void Test(Me me)
    {
        me.Value1 = 0;
        /*EA_TYPE_002*/ me.Value2 = 0; // err due to [Exclude<Friend2>]

        me.Method1();
        // ...
        me.Method9();

        me.Method10();
        /*EA_TYPE_001*/ me.Method11(); // err due to [Exclude<Friend2>]
    }
}

file class Friend3
{
    public void Test(Me me)
    {
        me.Value1 = 0;
        me.Value2 = 0;

        /*EA_TYPE_001*/ me.Method1();
        // ...
        /*EA_TYPE_001*/ me.Method9();

        me.Method10(); // ok due to [Exclude]
        /*EA_TYPE_001*/ me.Method11();
    }
}

file class NoFriend
{
    void Test(Me me)
    {
        /*EA_TYPE_002*/ me.Value1 = 0;
        /*EA_TYPE_002*/ me.Value2 = 0;

        /*EA_TYPE_001*/ me.Method1();
        // ...
        /*EA_TYPE_001*/ me.Method9();

        me.Method10(); // ok due to [Exclude]
        /*EA_TYPE_001*/ me.Method11();
    }
}

////[OnlyYou<Fiend1>]
////[OnlyYou<Fiend2>]
//[OnlyYou("R:Fiend2$")]
//file class Me
//{
//    //[Exclude]
//    public Me() { }

//    //[Exclude<Fiend1>]
//    //[Exclude(typeof(Fiend2))]
//    [Exclude("TestAnalyzerLib11.Fiend2")]
//    //[Exclude("R:Fiend2$")]
//    //[Exclude]
//    public void Method1() { }
//    public void Method2() { }
//}

//file class Fiend1
//{
//    void UseMe(Me me)
//    {
//        me.Method1();
//        me.Method2();
//    }
//}

//file class Fiend2
//{
//    void UseMe(Me me)
//    {
//        var m = new Me();
//        me.Method1();
//        me.Method2();
//    }
//}


//file class NotMyFiend
//{
//    void UseMe(Me me)
//    {
//        //var m = new Me();
//        me.Method1();
//        me.Method2();
//    }
//}
