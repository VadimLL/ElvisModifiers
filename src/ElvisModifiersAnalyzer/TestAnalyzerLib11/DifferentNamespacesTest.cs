namespace TestAnalyzerLib11
{
    file class Me
    {
        public decimal Money { get; private set; } = 100;

        [OnlyYou<NS1.MyFriend>(nameof(NS1.MyFriend.AcceptMoney))]
        public decimal TakeMyHalfMoney1()
        {
            decimal half = Money / 2;
            Money -= half;
            return half;
        }

        [OnlyYou<NS2.MyFriend>(nameof(NS2.MyFriend.AcceptMoney))]
        public decimal TakeMyHalfMoney2()
        {
            decimal half = Money / 2;
            Money -= half;
            return half;
        }
    }
}

namespace NS1
{
    using TestAnalyzerLib11;
    file class MyFriend
    {
        public void AcceptMoney(in Me me)
        {
            Money += me.TakeMyHalfMoney1(); // ok
            Money += /*EA_METH_001*/ me.TakeMyHalfMoney2(); // err
        }
        public void CantAcceptMoney(in Me me)
            => Money += /*EA_METH_001*/ me.TakeMyHalfMoney1(); // err
        public decimal Money { get; private set; } = -40;
    }
}

namespace NS2
{
    using TestAnalyzerLib11;
    file class MyFriend
    {
        public void AcceptMoney(in Me me)
        {
            Money += /*EA_METH_001*/ me.TakeMyHalfMoney1(); // err
            Money += me.TakeMyHalfMoney2(); // ok
        }
        public void CantAcceptMoney(in Me me)
            => Money += /*EA_METH_001*/ me.TakeMyHalfMoney2(); // err
        public decimal Money { get; private set; } = -40;
    }
}