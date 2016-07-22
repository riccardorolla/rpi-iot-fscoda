// Compiler output follows.
namespace Rpi { 
using System; 
using System.Collections.Generic;
using YieldProlog;
public class RpiContext {
public class YPInnerClass { }
public static Type getDeclaringClass() { return typeof(YPInnerClass).DeclaringType; }

public static IEnumerable<bool> gpio_direction(object Pin, object Direction)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("gpio_direction_"), new object[] { Pin, Direction }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> gpio_resistor(object Pin, object Resistor)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("gpio_resistor_"), new object[] { Pin, Resistor }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> gpio_digital(object Pin, object Status)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("gpio_digital_"), new object[] { Pin, Status }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> gpio_device(object Name, object Pin)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("gpio_device_"), new object[] { Name, Pin }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> button(object Status)
{
    {
        Variable Pin = new Variable();
        foreach (bool l2 in gpio_device(Atom.a("button"), Pin))
        {
            foreach (bool l3 in gpio_digital(Pin, Status))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> led(object Status)
{
    {
        Variable Pin = new Variable();
        foreach (bool l2 in gpio_device(Atom.a("led"), Pin))
        {
            foreach (bool l3 in gpio_digital(Pin, Status))
            {
                yield return false;
            }
        }
    }
}

}}