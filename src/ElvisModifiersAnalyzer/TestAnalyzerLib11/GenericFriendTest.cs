namespace NS1
{
    file class Me<T>
    {
        T Value { get; set; } = default!;

        [OnlyYou(typeof(MyFriend<>))]
        public T Value2 { get; set; } = default!;

        [OnlyYou(typeof(MyFriend<>))]
        public T GetValue() => Value;

        [OnlyYou(typeof(MyFriend<>), nameof(MyFriend.Test))]
        public T GetValue2() => Value;

        void Test(Me<T> me)
        {
            var v = me.GetValue(); // ok
            v = this.GetValue(); // ok
            v = Value = Value2; // ok
        }
    }

    file class Me2
    {
        string Value { get; set; } = default!;

        [OnlyYou(typeof(MyFriend<string>))]
        public string GetValue() => Value;
    }


    file class MyFriend
    {
        public void Test(Me<string> me)
        {
            var v = /*EA_METH_001*/ me.GetValue(); // err
            v = /*EA_METH_001*/ me.GetValue2();
            v = /*EA_PROP_001*/ me.Value2;
        }

        public void Test1(Me2 me2)
        {
            var v = /*EA_METH_001*/ me2.GetValue(); // err
        }
    }

    file class MyFriend<T>
    {
        public void Test(Me<T> me)
        {
            var v = me.GetValue(); // ok
            v = me.GetValue2(); // ok
            v = me.Value2; // ok
        }

        public void Test1(Me<T> me)
        {
            var v = me.GetValue(); // ok
            v = /*EA_METH_001*/ me.GetValue2(); // err
        }

        public void Test2(Me2 me2)
        {
            var v = me2.GetValue(); // ok
        }
    }

    file class NotMyFriend<T>
    {
        public void Test(Me<T> me)
        {
            var v = /*EA_METH_001*/ me.GetValue(); // err
            v = /*EA_PROP_001*/ me.Value2; // err
        }
    }

}

namespace NS2
{
    file class MyFriend<T>
    {
        public void Test(NS1.Me<T> me)
        {
            var v = /*EA_METH_001*/ me.GetValue(); // err
            v = /*EA_PROP_001*/ me.Value2; // err
        }
    }
}