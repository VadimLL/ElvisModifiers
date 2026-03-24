namespace MeAsm // not dependet from the FriendAsm assembly
{
    class Me
    {
        const string Friend = "FriendAsm.Friend"; // type name from the FriendAsm assembly

        [OnlyYou($"{Friend}", "UseMe")]
        public int Value1 { get; set; }

        [OnlyAlias($"{Friend}", "Alias1")]
        public int Value2 { get; set; }

        [OnlyYouSet($"{Friend}", "UseMe")]
        public int Value3 { get; set; }

        [OnlyYou($"{Friend}", "UseMe")]
        public int Method1()
        {
            Value1 += 1;
            return Value1;
        }

        [OnlyAlias($"{Friend}", "Alias1")]
        public int Method2()
        {
            Value2 += 1;
            return Value2;
        }
    }

    [OnlyYou($"{Friend}", "UseMe2")]
    class Me2
    {
        const string Friend = "FriendAsm.Friend"; // type name from the FriendAsm assembly
        public int Value1 { get; set; }
        public int Method1()
        {
            Value1 += 1;
            return Value1;
        }
    }
}

namespace FriendAsm // dependet from the MeAsm assembly
{
    using MeAsm;

    class Friend
    {
        public void UseMe(in Me me)
        {
            me.Method1();
            /*EA_METH_001*/ me.Method2();
            me.Value1 = 1;
            /*EA_PROP_002*/ me.Value2 = 1;
            me.Value3 = 0;
        }
        public void CantUseMe(in Me me)
        {
            /*EA_METH_001*/ me.Method1();
            /*EA_PROP_002*/ me.Value1 = 1;
            var x = me.Value3;
            /*EA_PROP_002*/ me.Value3 = 0;
        }

        [Alias("Alias1")]
        public void AUse1Me(in Me me)
        {
            /*EA_METH_001*/ me.Method1();
            me.Method2();
            /*EA_PROP_002*/ me.Value1 = 1;
            me.Value2 = 1;
        }

        [Alias("Alias2")]
        public void AUse2Me(in Me me)
        {
            /*EA_METH_001*/ me.Method1();
            /*EA_METH_001*/ me.Method2();
            /*EA_PROP_002*/ me.Value1 = 1;
            /*EA_PROP_002*/ me.Value2 = 1;
        }


        public void UseMe2(in Me2 me2)
        {
            me2.Method1();
            me2.Value1 = 1;
        }
        public void CantUseMe2(in Me2 me2)
        {
            /*EA_TYPE_001*/ me2.Method1();
            /*EA_TYPE_001*/ me2.Value1 = 1;
        }
    }

    class NoFriend
    {
        public void UseMe(in Me me)
        {
            /*EA_METH_001*/ me.Method1();
            /*EA_METH_001*/ me.Method2();
            /*EA_PROP_002*/ me.Value1 = 1;
            /*EA_PROP_002*/ me.Value2 = 1;
        }

        public void UseMe2(in Me2 me2)
        {
            /*EA_TYPE_001*/ me2.Method1();
            /*EA_TYPE_001*/ me2.Value1 = 1;
        }
    }
}
