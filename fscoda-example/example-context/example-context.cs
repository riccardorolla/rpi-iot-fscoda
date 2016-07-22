// Compiler output follows.
namespace Example { 
using System; 
using System.Collections.Generic;
using YieldProlog;
public class ExampleContext {
public class YPInnerClass { }
public static Type getDeclaringClass() { return typeof(YPInnerClass).DeclaringType; }

public static IEnumerable<bool> video(object arg1)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("hd")))
        {
            foreach (bool l3 in YP.matchDynamic(Atom.a("screen_quality"), new object[] { Atom.a("hd") }))
            {
                foreach (bool l4 in YP.matchDynamic(Atom.a("supported_codec"), new object[] { Atom.a("H.264") }))
                {
                    foreach (bool l5 in YP.matchDynamic(Atom.a("battery_level"), new object[] { Atom.a("low") }))
                    {
                        goto cutIf1;
                    }
                    yield return false;
                cutIf1:
                    { }
                }
            }
        }
    }
}

public static IEnumerable<bool> use_qrcode(object X)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("user_prefer"), new object[] { Atom.a("qr_code") }))
        {
            foreach (bool l3 in YP.matchDynamic(Atom.a("qr_decoder"), new object[] { X }))
            {
                foreach (bool l4 in YP.matchDynamic(Atom.a("device"), new object[] { Atom.a("camera") }))
                {
                    yield return false;
                }
            }
        }
    }
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("qr_decoder"), new object[] { X }))
        {
            foreach (bool l3 in YP.matchDynamic(Atom.a("device"), new object[] { Atom.a("camera") }))
            {
                foreach (bool l4 in YP.matchDynamic(Atom.a("device"), new object[] { Atom.a("irda") }))
                {
                    goto cutIf1;
                }
                foreach (bool l4 in YP.matchDynamic(Atom.a("device"), new object[] { Atom.a("rfid_reader") }))
                {
                    goto cutIf2;
                }
                foreach (bool l4 in YP.matchDynamic(Atom.a("device"), new object[] { Atom.a("bluetooth") }))
                {
                    goto cutIf3;
                }
                yield return false;
            cutIf3:
            cutIf2:
            cutIf1:
                { }
            }
        }
    }
}

public static IEnumerable<bool> direct_comm()
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("device"), new object[] { Atom.a("irda") }))
        {
            yield return false;
        }
    }
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("device"), new object[] { Atom.a("bluetooth") }))
        {
            yield return false;
        }
    }
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("device"), new object[] { Atom.a("rfid_reader") }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> orientation(object S)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("orientation0"), new object[] { S }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> sscreen(object S)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("sscreen0"), new object[] { S }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> supported_media(object F)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("supported_media0"), new object[] { F }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> camera(object C)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("camera0"), new object[] { C }))
        {
            yield return false;
        }
    }
}

}}