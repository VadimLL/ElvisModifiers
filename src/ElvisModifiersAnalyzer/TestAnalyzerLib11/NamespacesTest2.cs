namespace TestAnalyzerLib11
{
    file class Me1
    {
        [OnlyNs(nameof(NS1))]
        [OnlyNsSet(nameof(NS2))]
        public int Value1 { get; set; }
    }

    [OnlyNs(nameof(NS1))]
    [OnlyNsSet(nameof(NS2))]
    file class Me2
    {
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
            var v = me.Value1;
            /*EA_NS_002*/ me.Value1 = 0;
        }

        public void UseMe2(in Me2 me)
        {
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
            var v = me.Value1;
            me.Value1 = 0;
        }

        public void UseMe2(in Me2 me)
        {
            var v = me.Value1;
            me.Value1 = 0;
        }
    }
}

namespace NS3
{
    using TestAnalyzerLib11;
    file class Friend
    {
        public void UseMe(in Me1 me)
        {
            var v = /*EA_NS_001*/ me.Value1;
            /*EA_NS_002*/ me.Value1 = 0;
        }

        public void UseMe2(in Me2 me)
        {
            var v = /*EA_NS_001*/ me.Value1;
            /*EA_NS_002*/ me.Value1 = 0;
        }
    }
}