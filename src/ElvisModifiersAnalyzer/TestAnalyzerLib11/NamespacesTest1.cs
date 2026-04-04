namespace TestAnalyzerLib11
{
    file class Me1
    {
        [OnlyNs(nameof(NS1))]
        public void Method1() { }

        [OnlyNsSet(nameof(NS1))]
        public int Value1 { get; set; }

        [OnlyYou<NS1.Friend>]
        public void Method2() { }
    }

    [OnlyNs(nameof(NS1))]
    file class Me2
    {
        public void Method1() { }

        [OnlyNsSet(nameof(NS1))]
        public int Value1 { get; set; }
    }
}

namespace NS1
{
    using TestAnalyzerLib11;
    file class Friend
    {
        public void UseMe(in Me1 me)
        {
            me.Method1();
            me.Method2();
            var v = me.Value1;
            me.Value1 = 0;
        }

        public void UseMe2(in Me2 me)
        {
            me.Method1();
            var v = me.Value1;
            /*EA_NS_002*/ me.Value1 = 0;
        }
    }
}

namespace NS2
{
    using TestAnalyzerLib11;
    file class Friend
    {
        public void UseMe(in Me1 me)
        {
            /*EA_NS_001*/ me.Method1();
            /*EA_METH_001*/ me.Method2();
            var v = me.Value1;
            /*EA_NS_002*/ me.Value1 = 0;
        }

        public void UseMe2(in Me2 me)
        {
            /*EA_NS_001*/ me.Method1();
            var v = /*EA_NS_001*/ me.Value1;
            /*EA_NS_002*/ me.Value1 = 0;
        }
    }
}