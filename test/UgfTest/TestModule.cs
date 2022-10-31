using System;
using Secyud.Ugf.Modularity;

namespace UgfTest;

public class TestModule:UgfModule
{
    public override void Configure(ConfigurationContext context)
    {
        Console.WriteLine("Configure");
    }

    public override void OnInitialization(InitializationContext context)
    {
        Console.WriteLine("OnInitialization");
    }

    public override void OnShutdown(ShutdownContext context)
    {
        Console.WriteLine("OnShutdown");
    }
}