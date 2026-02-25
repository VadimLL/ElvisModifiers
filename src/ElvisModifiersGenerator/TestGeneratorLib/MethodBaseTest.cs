//namespace TestGeneratorLib;

file class Me
{
    public decimal Money { get; private set; } = 100;
    public decimal UnsharedMoney { get; private set; } = 100_000;

    //[OnlyYou(typeof(MyFriend), "AcceptMoney")]

    [OnlyYou(typeof(MyFriend), nameof(MyFriend.AcceptMoney))]
    public decimal TakeMyHalfMoney() {
        decimal half = Money / 2;
        Money -= half;
        return half;
    }
}

file class MyFriend
{
    public void AcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // ok
    public void CantAcceptMoney(in Me me) => Money += me.TakeMyHalfMoney(); // err
    public void AcceptMoneyFromNotMe(in NotMe me) => Money += me.TakeMyHalfMoney(); // ok
    public decimal Money { get; private set; } = -40;
}


file class NotMe
{
    public decimal TakeMyHalfMoney() => 0;
}
