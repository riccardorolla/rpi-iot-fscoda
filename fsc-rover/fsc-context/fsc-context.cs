// Compiler output follows.
namespace Fsc { 
using System; 
using System.Collections.Generic;
using YieldProlog;
public class FscContext {
public class YPInnerClass { }
public static Type getDeclaringClass() { return typeof(YPInnerClass).DeclaringType; }

public static IEnumerable<bool> request(object IdChat, object Cmd)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("request_"), new object[] { IdChat, Cmd }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> execute(object Cmd, object Out)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("execute_"), new object[] { Cmd, Out }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> observe(object Obj, object Status)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("observe_"), new object[] { Obj, Status }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> response(object IdChat, object Out)
{
    {
        Variable Cmd = new Variable();
        foreach (bool l2 in request(IdChat, Cmd))
        {
            foreach (bool l3 in execute(Cmd, Out))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> stop(object Status)
{
    {
        foreach (bool l2 in observe(Atom.a("obstacole"), Status))
        {
            yield return false;
        }
    }
}

}}