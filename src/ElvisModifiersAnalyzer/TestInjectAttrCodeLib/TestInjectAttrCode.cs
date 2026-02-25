/*
For test open the ElvisModifiersAnalyzer.cs file of the ElvisModifiersAnalyzer project,
and uncomment:
//#define INJECT_ATTR
 */

namespace TestInjectAttrCodeLib;

file class Me
{
    public decimal Money { get; private set; } = 100;
    public decimal UnsharedMoney { get; private set; } = 100_000;

    [OnlyYou<MyFriend>(nameof(MyFriend.AcceptMoney))]
    //[OnlyYou(typeof(MyFriend), nameof(MyFriend.AcceptMoney))]
    public decimal TakeMyHalfMoney()
    {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }
}

file class MyFriend
{
    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // err
    public decimal Money { get; private set; } = -40;
}

