// Compiler output follows.
namespace Physicians { 
using System; 
using System.Collections.Generic;
using YieldProlog;
public class PhysiciansContext {
public class YPInnerClass { }
public static Type getDeclaringClass() { return typeof(YPInnerClass).DeclaringType; }

public static IEnumerable<bool> physician_location(object Physician, object Location)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("physician_location_"), new object[] { Physician, Location }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> physician_exam(object Physician, object Exam)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("physician_exam_"), new object[] { Physician, Exam }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> physician_has_hw(object Physician, object Hardware)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("physician_has_hw_"), new object[] { Physician, Hardware }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> patient_location(object Patient, object Location)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("patient_location_"), new object[] { Patient, Location }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> patient_has_done(object Patient, object Exam)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("patient_has_done_"), new object[] { Patient, Exam }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> patient_has_been_prescribed(object Patient, object Exam)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("patient_has_been_prescribed_"), new object[] { Patient, Exam }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> exam_requirement(object Exam, object Requirement)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("exam_requirement_"), new object[] { Exam, Requirement }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> exam_view_hw(object Exam, object Hardware)
{
    {
        foreach (bool l2 in YP.matchDynamic(Atom.a("exam_view_hw_"), new object[] { Exam, Hardware }))
        {
            yield return false;
        }
    }
}

public static IEnumerable<bool> physician_can_view_patient(object Physician, object Patient)
{
    {
        Variable Location = new Variable();
        foreach (bool l2 in physician_location(Physician, Location))
        {
            foreach (bool l3 in patient_location(Patient, Location))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> patient_needs_done(object Patient, object Exam)
{
    {
        foreach (bool l2 in patient_has_been_prescribed(Patient, Exam))
        {
            yield return false;
        }
    }
    {
        Variable TargetExam = new Variable();
        foreach (bool l2 in exam_requirement(TargetExam, Exam))
        {
            foreach (bool l3 in patient_needs_done(Patient, TargetExam))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> patient_should_do(object Patient, object Exam)
{
    {
        foreach (bool l2 in patient_needs_done(Patient, Exam))
        {
            foreach (bool l3 in patient_has_done(Patient, Exam))
            {
                goto cutIf1;
            }
            yield return false;
        cutIf1:
            { }
        }
    }
}

public static IEnumerable<bool> patient_cannot_do(object Patient, object Exam)
{
    {
        Variable Requirement = new Variable();
        foreach (bool l2 in exam_requirement(Exam, Requirement))
        {
            foreach (bool l3 in patient_has_done(Patient, Requirement))
            {
                goto cutIf1;
            }
            yield return false;
        cutIf1:
            { }
        }
    }
}

public static IEnumerable<bool> patient_active_exam(object Patient, object Exam)
{
    {
        foreach (bool l2 in patient_should_do(Patient, Exam))
        {
            foreach (bool l3 in patient_cannot_do(Patient, Exam))
            {
                goto cutIf1;
            }
            yield return false;
        cutIf1:
            { }
        }
    }
}

public static IEnumerable<bool> physician_can_view_exam(object Physician, object Exam)
{
    {
        Variable Hardware = new Variable();
        foreach (bool l2 in exam_view_hw(Exam, Hardware))
        {
            goto cutIf1;
        }
        yield return false;
    cutIf1:
        { }
    }
    {
        Variable Hardware = new Variable();
        foreach (bool l2 in physician_has_hw(Physician, Hardware))
        {
            foreach (bool l3 in exam_view_hw(Exam, Hardware))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> device_can_display_exam(object Device, object Exam)
{
    {
        Variable Capability = new Variable();
        foreach (bool l2 in device_has_caps(Device, Capability))
        {
            foreach (bool l3 in YP.matchDynamic(Atom.a("exam_view_caps"), new object[] { Exam, Capability }))
            {
                yield return false;
            }
        }
    }
}

public static IEnumerable<bool> device_has_caps(object arg1, object arg2)
{
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("iPhone 5")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("3D acceleration")))
            {
                yield return false;
            }
        }
    }
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("iPhone 5")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("Video codec")))
            {
                yield return false;
            }
        }
    }
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("iPhone 5")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("Text display")))
            {
                yield return false;
            }
        }
    }
    {
        foreach (bool l2 in YP.unify(arg1, Atom.a("Apple Watch")))
        {
            foreach (bool l3 in YP.unify(arg2, Atom.a("Text display")))
            {
                yield return false;
            }
        }
    }
}

}}