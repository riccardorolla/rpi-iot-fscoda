// Compiler output follows.
namespace Fsc { 
using System; 
using System.Collections.Generic;
using YieldProlog;
public class FscContext {
public class YPInnerClass { }
public static Type getDeclaringClass() { return typeof(YPInnerClass).DeclaringType; }

public static IEnumerable<bool> usrcmd(object UserCmd, object SysCmd)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("usrcmd_"), new object[] { UserCmd, SysCmd }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> cmddesc(object SysCmd, object Desc)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("cmddesc_"), new object[] { SysCmd, Desc }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> objcmd(object Obj, object SysCmd)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("objcmd_"), new object[] { Obj, SysCmd }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> request(object Id, object UserCmd)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("request_"), new object[] { Id, UserCmd }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> result(object SysCmd, object Out)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("result_"), new object[] { SysCmd, Out }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> recognition(object Obj, object Value)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("recognition_"), new object[] { Obj, Value }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> confidence(object Obj, object Min, object Max)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("confidence_"), new object[] { Obj, Min, Max }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> action(object Obj, object Status, object Cmd)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("action_"), new object[] { Obj, Status, Cmd }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> response(object Id, object Out)
{
    {
        Variable UserCmd = new Variable();
        Variable SysCmd = new Variable();
        foreach (bool l2 in request(Id, UserCmd))
        {
            foreach (bool l3 in usrcmd(UserCmd, SysCmd))
            {
                foreach (bool l4 in result(SysCmd, Out))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> usrcmddesc(object UserCmd, object Desc)
{
    {
        Variable Cmd = new Variable();
        foreach (bool l2 in usrcmd(UserCmd, Cmd))
        {
            foreach (bool l3 in cmddesc(Cmd, Desc))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> detected(object Obj)
{
    {
        Variable Value = new Variable();
        Variable Min = new Variable();
        Variable Max = new Variable();
        foreach (bool l2 in recognition(Obj, Value))
        {
            foreach (bool l3 in confidence(Obj, Min, Max))
            {
                if (YP.greaterThan(Value, Min))
                {
                    if (YP.lessThan(Value, Max))
                    {
                        yield return false;
                    }
                }
            }
        }
    }
    {
        Variable Value = new Variable();
        Variable Min = new Variable();
        Variable Max = new Variable();
        foreach (bool l2 in recognition(Obj, Value))
        {
            foreach (bool l3 in confidence(Obj, Min, Max))
            {
                foreach (bool l4 in YP.unify(Value, Min))
                {
                    yield return false;
                }
            }
        }
    }
    {
        Variable Value = new Variable();
        Variable Min = new Variable();
        Variable Max = new Variable();
        foreach (bool l2 in recognition(Obj, Value))
        {
            foreach (bool l3 in confidence(Obj, Min, Max))
            {
                foreach (bool l4 in YP.unify(Value, Max))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> undetected(object Obj)
{
    {
        Variable Value = new Variable();
        Variable Min = new Variable();
        Variable Max = new Variable();
        foreach (bool l2 in recognition(Obj, Value))
        {
            foreach (bool l3 in confidence(Obj, Min, Max))
            {
                if (YP.lessThan(Value, Min))
                {
                    yield return false;
                }
            }
        }
    }
    {
        Variable Value = new Variable();
        Variable Min = new Variable();
        Variable Max = new Variable();
        foreach (bool l2 in recognition(Obj, Value))
        {
            foreach (bool l3 in confidence(Obj, Min, Max))
            {
                if (YP.greaterThan(Value, Max))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> next(object SysCmd)
{
    {
        Variable Obj = new Variable();
        foreach (bool l2 in detected(Obj))
        {
            foreach (bool l3 in action(Obj, Atom.a("true"), SysCmd))
            {
                yield return false;
            }
        }
    }
    {
        Variable Obj = new Variable();
        foreach (bool l2 in undetected(Obj))
        {
            foreach (bool l3 in action(Obj, Atom.a("false"), SysCmd))
            {
                yield return false;
            }
        }
    }
    {
        Variable Id = new Variable();
        Variable UserCmd = new Variable();
        foreach (bool l2 in request(Id, UserCmd))
        {
            foreach (bool l3 in usrcmd(UserCmd, SysCmd))
            {
                yield return false;
            }
        }
    }
}

}}