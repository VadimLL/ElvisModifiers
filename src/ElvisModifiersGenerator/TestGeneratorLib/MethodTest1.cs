namespace TestGeneratorLib;

file class Me
{
    public interface IFriend
    {
        [OnlyYou(typeof(MyFriend), nameof(MyFriend.AcceptMoney))]
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

    public void CantAcceptMoney(in Me me)
      => Money += Me.IFriend.TakeMyHalfMoney(me); // err

    public decimal Money { get; private set; } = -40;
}

