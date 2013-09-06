using System;
using System.IO;
using Edge.Builders;
using System.Collections.Generic;

namespace Edge.ConsoleApplication
{

    class Program
    {

        static void Main(string[] args)
        {
            string text = File.ReadAllText("MainView.edge");
            EdgeParser parser = new EdgeParser()
            {
                Assemblies = new HashSet<string>()
                {
                    "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                    "WindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                    "PresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                    "PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",
                    "System.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
                },
                Namespaces = new HashSet<string>()
                {
                    "System",
                    "System.Windows",
                    "System.Windows.Controls",
                    "System.Windows.Data",
                    "System.Windows.Documents",
                    "System.Windows.Input",
                    "System.Windows.Media",
                    "System.Windows.Media.Imaging",
                    "System.Windows.Navigation",
                    "System.Windows.Shapes"
                }
            };
            var st = parser.Parse(text);
            var a = new EdgeAnalyzer()
            {
                Assemblies = parser.Assemblies,
                Namespaces = parser.Namespaces
            };
            a.Analyze(st);
            string file = st.Build(new CSharpBuilder(), "MainView", "WPF.Views");
            File.WriteAllText("MainView.cs", file);
        }

    }

}
