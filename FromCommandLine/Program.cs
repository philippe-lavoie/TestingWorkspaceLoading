using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mcd.Testing.Tooling.CommandLine
{
    public class TestRunner : IDisposable
    {
        Stopwatch watch = new Stopwatch();
        readonly String methodName;
        string extra = null;
        public TestRunner(string name)
        {
            watch.Start();
            methodName = name;
            Console.Out.Write("{0,30}", methodName);
        }

        public void Run(Func<bool> func)
        {
            var currentColor = Console.ForegroundColor;
            try
            {
                var result = func();                
                Console.ForegroundColor = result ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Out.Write("{0,6}", result ? "pass" : "fail");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Out.Write("{0,6}", "fail");
                extra = ex.ToString();
            }
            Console.ForegroundColor = currentColor;
        }

        public void Dispose()
        {            
            watch.Stop();
            Console.Out.WriteLine("{0,10}", watch.ElapsedMilliseconds);
            if(extra != null)
            {
                Console.Out.WriteLine(extra.Length > 400 ? extra.Substring(0,400) : extra);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();

            Console.Out.WriteLine("Tests are starting");

            using (var runner = new TestRunner(nameof(TestLoadingFrameworkProject)))
            {
                runner.Run(program.TestLoadingFrameworkProject);
            }
            using (var runner = new TestRunner(nameof(TestLoadingStandardProject)))
            {
                runner.Run(program.TestLoadingStandardProject);
            }
        }


        public bool TestLoadingFrameworkProject()
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileInfo = new FileInfo(assemblyLocation);
            var folder = fileInfo.Directory;
            var parent = folder.Parent.Parent;

            var projectPath = Path.Combine(parent.FullName, "ADotNetFrameworkLibrary");
            projectPath = Path.Combine(projectPath, "ADotNetFrameworkLibrary.csproj");

            var msWorkspace = MSBuildWorkspace.Create();
            msWorkspace.WorkspaceFailed += MsWorkspace_WorkspaceFailed;
            var project = msWorkspace.OpenProjectAsync(projectPath).Result;
            var compilation = project.GetCompilationAsync().Result;
            return project.DocumentIds.Count > 0;
        }

        public bool TestLoadingStandardProject()
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileInfo = new FileInfo(assemblyLocation);
            var folder = fileInfo.Directory;
            var parent = folder.Parent.Parent;

            var projectPath = Path.Combine(parent.FullName, "ADotNetStandardLibrary");
            projectPath = Path.Combine(projectPath, "ADotNetStandardLibrary.csproj");

            var msWorkspace = MSBuildWorkspace.Create();
            msWorkspace.WorkspaceFailed += MsWorkspace_WorkspaceFailed;
            var project = msWorkspace.OpenProjectAsync(projectPath).Result;
            var compilation = project.GetCompilationAsync().Result;
            return project.DocumentIds.Count > 0;
        }

        private void MsWorkspace_WorkspaceFailed(object sender, Microsoft.CodeAnalysis.WorkspaceDiagnosticEventArgs e)
        {
            throw new Exception(e.Diagnostic.Message);
        }
    }
}
