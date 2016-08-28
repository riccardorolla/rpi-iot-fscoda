// Compiler output follows.
namespace Fsc { 
using System; 
using System.Collections.Generic;
using YieldProlog;
public class FscContext {
public class YPInnerClass { }
public static Type getDeclaringClass() { return typeof(YPInnerClass).DeclaringType; }

public static IEnumerable<bool> request(object IdChat, object Cmd, object Param)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("request_"), new object[] { IdChat, Cmd, Param }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> result(object Cmd, object Out)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("result_"), new object[] { Cmd, Out }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> found(object Obj, object Status)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("found_"), new object[] { Obj, Status }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> rule(object Obj, object Status, object Cmd)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("rule_"), new object[] { Obj, Status, Cmd }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> user_command(object Prompt, object Cmd)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("user_command_"), new object[] { Prompt, Cmd }))
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

public static IEnumerable<bool> response(object IdChat, object Out)
{
    {
        Variable Cmd = new Variable();
        Variable Param = new Variable();
        foreach (bool l2 in request(IdChat, Cmd, Param))
        {
            foreach (bool l3 in result(Cmd, Out))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> next(object Obj, object Cmd)
{
    {
        Variable Status = new Variable();
        foreach (bool l2 in found(Obj, Status))
        {
            foreach (bool l3 in rule(Obj, Status, Cmd))
            {
                yield return false;
            }
        }
    }
}

}}