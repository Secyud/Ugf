using System;
using Secyud.Ugf.DataManager;

namespace Secyud.Ugf.DependencyInjection;

public class DefaultTypeAnalyser:ITypeAnalyzer
{
    public void AnalyzeType(Type type)
    {
        TypeIdMapper.SetType(type,TypeIdMapper.GetId(type));
    }
}