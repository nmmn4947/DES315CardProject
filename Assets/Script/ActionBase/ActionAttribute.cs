using System;
using UnityEngine;

public class ActionAttribute : PropertyAttribute
{
    Type type;
    public Type Type => type;

    public ActionAttribute(Type type)
    {
        this.type = type;
    }
}
