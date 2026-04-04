namespace TestAnalyzerLib11
{
    file class Me
    {
        public int Value { get; private set; } = 100;

        [OnlyYou<NS1.Friend>(nameof(NS1.Friend.AcceptValue))]
        public int TakeHalfValue1()
        {
            int half = Value / 2;
            Value -= half;
            return half;
        }

        [OnlyYou<NS2.Friend>(nameof(NS2.Friend.AcceptValue))]
        public int TakeHalfValue2()
        {
            int half = Value / 2;
            Value -= half;
            return half;
        }
    }
}

namespace NS1
{
    using TestAnalyzerLib11;
    file class Friend
    {
        public void AcceptValue(in Me me)
        {
            Value += me.TakeHalfValue1(); // ok
            Value += /*EA_METH_001*/ me.TakeHalfValue2(); // err
        }
        public void CantAcceptValue(in Me me)
            => Value += /*EA_METH_001*/ me.TakeHalfValue1(); // err
        public int Value { get; private set; } = -40;
    }
}

namespace NS2
{
    using TestAnalyzerLib11;
    file class Friend
    {
        public void AcceptValue(in Me me)
        {
            Value += /*EA_METH_001*/ me.TakeHalfValue1(); // err
            Value += me.TakeHalfValue2(); // ok
        }
        public void CantAcceptValue(in Me me)
            => Value += /*EA_METH_001*/ me.TakeHalfValue2(); // err
        public int Value { get; private set; } = -40;
    }
}