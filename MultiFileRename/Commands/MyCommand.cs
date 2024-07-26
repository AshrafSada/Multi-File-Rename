using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;

namespace MultiFileRename;

[Command(PackageIds.MyCommand)]
internal sealed class MyCommand : BaseCommand<MyCommand>
{
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        var dte2 = await ServiceProvider.GetGlobalServiceAsync(typeof(SDTE)) as DTE2;
        var solution = dte2.Solution;
        var solutionPath = Path.GetDirectoryName(solution.FullName);
        // get selected files

        var selectedFiles = dte2.SelectedItems
           .OfType<SelectedItem>()
           .Select(f => f.ProjectItem.FileNames[1])
           .ToList();
    }

    private static async Task RenameCSharpFilesWithNewNameAsync(string folderPath)
    {
        var acceptedExtensions = new List<string> { ".cs" };
        var cSharpFiles = Directory.EnumerateFiles(folderPath)
            .Where(f => acceptedExtensions
                .Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase));

        foreach (var csFile in cSharpFiles)
        {
            var createdTime = File.GetCreationTime(csFile);
            var fileExt = Path.GetExtension(csFile);
            var newFileName = $"{createdTime:yyyyMMddHHmmss}{fileExt}";
            var newFilePath = Path.Combine(folderPath, newFileName);

            // renaming of files
            File.Move(csFile, newFilePath);
        }

        await Task.CompletedTask;
    }
}
