// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

namespace ConsoleAppTestIFriend;

static class Test2
{
    public static void Run()
    {
        Console.WriteLine(nameof(Test2));

        var me = new Me();
        var myPoorFriend = new MyFriend();

        log(me, myPoorFriend);
        myPoorFriend.AcceptMoney(me);
        log(me, myPoorFriend);

        Console.WriteLine();

        static void log(in Me me, MyFriend friend) =>
            Console.WriteLine($"me: {me.Money}; friend: {friend.Money}");
    }
}

file class Me
{
    public interface IFriend
    {
        static decimal TakeMyHalfMoney(Me self)
        {
            decimal half = self.Money / 2;
            self.Money -= half;
            return half;
        }
    }

    public decimal Money { get; private set; } = 100;
    public decimal UnsharedMoney { get; private set; } = 100_000;
}

file class MyFriend : Me.IFriend
{
    public void AcceptMoney(in Me me)
      => Money += Me.IFriend.TakeMyHalfMoney(me);

    public decimal Money { get; private set; } = -40;
}
