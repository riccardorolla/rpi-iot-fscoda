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

public static IEnumerable<bool> synopsis(object Cmd, object Description)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("synopsis_"), new object[] { Cmd, Description }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> detected(object Obj, object Status)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("detected_"), new object[] { Obj, Status }))
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
        foreach (bool l2 in detected(Obj, Status))
        {
            foreach (bool l3 in action(Obj, Status, Cmd))
            {
                yield return false;
            }
        }
    }
}

}}