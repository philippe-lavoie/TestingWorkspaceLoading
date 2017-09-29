using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Microsoft.CodeAnalysis.MSBuild;
using System.Reflection;

namespace FromUnitTest
{
    [TestClass]
    public class LoadWorkspaceTests
    {
        [TestMethod]
        public void LoadDotNetFramework()
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
            Assert.IsTrue(project.DocumentIds.Count > 0);
        }

        private void MsWorkspace_WorkspaceFailed(object sender, Microsoft.CodeAnalysis.WorkspaceDiagnosticEventArgs e)
        {
            throw new Exception(e.Diagnostic.Message);
        }

        [TestMethod]
        public void LoadDotNetStandard()
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
            Assert.IsTrue(project.DocumentIds.Count > 0);
        }
    }
}
