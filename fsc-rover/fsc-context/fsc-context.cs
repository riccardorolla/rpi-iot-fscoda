// Compiler output follows.
namespace Fsc { 
using System; 
using System.Collections.Generic;
using YieldProlog;
public class FscContext {
public class YPInnerClass { }
public static Type getDeclaringClass() { return typeof(YPInnerClass).DeclaringType; }

public static IEnumerable<bool> rover_motor(object arg1)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("direction")))
        {
            foreach (bool l3 in YP.matchDynamic(Atom.a("rover_motor_"), new object[] { Atom.a("direction") }))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> rover_led(object arg1, object arg2)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("nled")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("status")))
            {
                foreach (bool l4 in YP.matchDynamic(Atom.a("rover_led_"), new object[] { Atom.a("nled"), Atom.a("status") }))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> rover_request(object arg1, object arg2)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("nuser")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("cmd")))
            {
                foreach (bool l4 in YP.matchDynamic(Atom.a("rover_request_"), new object[] { Atom.a("nuser"), Atom.a("cmd") }))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> rover_command(object arg1, object arg2)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("cmd")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("out")))
            {
                foreach (bool l4 in YP.matchDynamic(Atom.a("rover_command_"), new object[] { Atom.a("cmd"), Atom.a("out") }))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> rover_distance(object arg1, object arg2)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("distance")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("status")))
            {
                foreach (bool l4 in YP.matchDynamic(Atom.a("rover_distance_"), new object[] { Atom.a("distance"), Atom.a("status") }))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> rover_response(object arg1, object arg2)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("nuser")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("out")))
            {
                foreach (bool l4 in rover_request(Atom.a("nuser"), Atom.a("cmd")))
                {
                    foreach (bool l5 in rover_command(Atom.a("cmd"), Atom.a("out")))
                    {
                        yield return false;
                    }
                }
            }
        }
    }
}

public static IEnumerable<bool> rover_obstacle(object arg1)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("status")))
        {
            foreach (bool l3 in rover_command(Atom.a("distance"), Atom.a("distance")))
            {
                foreach (bool l4 in rover_distance(Atom.a("distance"), Atom.a("status")))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> rover_is_on(object arg1)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("nled")))
        {
            foreach (bool l3 in rover_command(Atom.a("led"), Atom.a("out")))
            {
                foreach (bool l4 in rover_led(Atom.a("nled"), Atom.a("out")))
                {
                    yield return false;
                }
            }
        }
    }
}

public static IEnumerable<bool> rover_stop(object arg1)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("status")))
        {
            foreach (bool l3 in rover_obstacle(Atom.a("status")))
            {
                yield return false;
            }
        }
    }
}

}}