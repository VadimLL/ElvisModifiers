// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

namespace ConsoleAppTestIFriend;

static class Test1
{
    public static void Run()
    {
        Console.WriteLine(nameof(Test1));

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
        // Setter
        static protected void setMoney(in Me self, in decimal value)
          => self.Money = value;
    }

    public decimal Money { get; private set; } = 100;
    public decimal UnsharedMoney { get; private set; } = 100_000;
}

file class MyFriend : Me.IFriend
{
    public void AcceptMoney(in Me me)
    {
        decimal half = me.Money / 2;
        Me.IFriend.setMoney(me, half);
        Money += half;
        //me.UnsharedMoney = me.Money = 0; // obviously 2 compile errors
    }

    public decimal Money { get; private set; } = -40;
}
