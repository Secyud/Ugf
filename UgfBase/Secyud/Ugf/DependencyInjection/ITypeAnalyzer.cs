using System;

namespace Secyud.Ugf.DependencyInjection;

public interface ITypeAnalyzer
{
    public void AnalyzeType(Type type);
}