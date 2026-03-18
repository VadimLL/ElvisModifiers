// Copyright (c) 2025 LeaLan (VadimLL). All rights reserved.
// License: MIT

using System;

using ElvisModifiersLib;

/// <summary>
/// Exclude (switch off) [OnlyYou*<T>] on the T for some member
/// (not implemented yet)
/// </summary>
[AttributeUsage(Constants.ExcludeTargets, AllowMultiple = true)]
public class ExcludeAttribute : Attribute
// ??? another names: ExcludeYou, Exept(You), Off, Cancel, Divorce, Free, Bye,...???
{
    public ExcludeAttribute() { }
    public ExcludeAttribute(Type type/*, params string[] members*/) { } // members ???
    public ExcludeAttribute(string type/*, params string[] members*/) { } // members ???
}

[AttributeUsage(Constants.ExcludeTargets, AllowMultiple = true)]
public class ExcludeAttribute<T> : Attribute
{
    public ExcludeAttribute(/*params string[] members*/) { } // members ???
}

/* !!! [Exclude] raw concept:
 
// works only when there is [Only*] attribute on a class

[OnlyYou<MyFriend>]
[OnlyYou<MyFriend2>]
class Me
{
    public void Method1() { }
    public void Method2() { }
    ...
    public void Method10() { }

    [Exclude]
    public void Method11() { }

    [Exclude<MyFriend2>]
    public void Method12() { }
    
    [Exclude<MyFriend>(nameof(MyFriend.Test))] // ???
    public void Method13() { }
}

class MyFriend
{
    void Test(Me me)
    {
        me.Method1(); // ok
        me.Method2(); // ok
        ...
        me.Method10(); // ok
        me.Method11(); // ok
        me.Method12(); // err
        me.Method13(); // ???
    }

    void Test1(Me me)
    {
        me.Method1(); // ok
        me.Method2(); // ok
        ...
        me.Method10(); // ok
        me.Method11(); // ok
        me.Method12(); // err
        me.Method13(); // ???
    }
}

class MyFriend2
{
    void Test(Me me)
    {
        me.Method1(); // ok
        me.Method2(); // ok
        ...
        me.Method10(); // ok
        me.Method11(); // ok
        me.Method12(); // ok
        me.Method13(); // ???
    }
}

class NotMyFriend
{
    void Test(Me me)
    {
        me.Method1(); // err
        me.Method2(); // err
        ...
        me.Method10(); // err
        me.Method11(); // ok
        me.Method12(); // err
        me.Method13(); // ???
    }
}
*/



