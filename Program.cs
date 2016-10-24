using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Diagrams
{   
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("****************************************************************************");
            Console.WriteLine("*                                                                          *");
            Console.WriteLine("*   Diagram  generator for PlantUML                                        *");
            Console.WriteLine("*   To generata a class diagram specify a file .cs or a directory          *");
            Console.WriteLine("*                                                                          *");
            Console.WriteLine("****************************************************************************");

            // to test Class Diagram
            // args = new[] { @"ClassDiagramGenerator.cs", @"..\uml" }; 

            if (args.Length < 1)
            {
                Console.WriteLine("Specify a file or directory");
                return;
            }

            var input = args[0];            

            IEnumerable<string> files;
            if (Directory.Exists(input))
            {
                files = Directory.EnumerateFiles(Path.GetFullPath(input), "*.cs");
            }
            else if (File.Exists(input))
            {                
                try
                {
                    var fullname = Path.GetFullPath(input);
                    files = new[] { fullname };
                }
                catch
                {                    
                    Console.WriteLine("Invalid name");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Specify an existing file or directory");
                return;
            }

            var outputDir = "";
            if (args.Length >= 2)
            {
                if (Directory.Exists(args[1]))
                {
                    outputDir = args[1];
                }
            }

            if (outputDir == "")
            {
                outputDir = Path.Combine(Path.GetDirectoryName(files.First()), "uml");
                Directory.CreateDirectory(outputDir);
            }

            foreach (var file in files)
            {
                Console.WriteLine($"Generation PlantUML text for {file}...");
                string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file));                

                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var tree = CSharpSyntaxTree.ParseText(SourceText.From(stream));
                    var root = tree.GetRoot();

                    using (var writer = new StreamWriter(new FileStream(outputFile + ".ClassDiagram.plantuml", FileMode.OpenOrCreate, FileAccess.Write)))
                    {
                        writer.WriteLine("@startuml");

                        var gen = new ClassDiagramGenerator(writer, "    ");
                        gen.Visit(root);

                        writer.Write("@enduml");
                     }                     
                }               
            }

            Console.WriteLine("Completed");
        }
    }
}

