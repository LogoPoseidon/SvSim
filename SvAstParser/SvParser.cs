using System.Diagnostics;
using System.Text;
using SvAstParser.Serializer;

namespace SvAstParser;

public static class SvParser
{
    /// <summary>
    /// Parses an existing slang AST JSON file from disk and returns the resolved AST representation.
    /// </summary>
    /// <param name="filePath">The path to the pre-generated slang AST JSON file.</param>
    /// <returns>The resolved <see cref="TopLevel"/> AST node.</returns>
    public static TopLevel ParseFromAstJsonFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("The file path cannot be null or whitespace.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The specified AST JSON file could not be found.", filePath);
        }

        var json = File.ReadAllText(filePath);
        return SlangSerializer.Parse(json);
    }

    /// <summary>
    /// Compiles a single SystemVerilog source file using slang and returns the resolved AST representation.
    /// </summary>
    /// <param name="filePath">The path to the SystemVerilog source file (.sv or .v).</param>
    /// <param name="slangExecutable">The name or absolute path of the slang executable. Defaults to "slang".</param>
    /// <param name="additionalArgs">Optional additional arguments to pass to slang.</param>
    /// <returns>The resolved <see cref="TopLevel"/> AST node.</returns>
    public static TopLevel ParseFromSystemVerilogFilePath(
        string filePath,
        string slangExecutable = "slang",
        IEnumerable<string>? additionalArgs = null)
    {
        return string.IsNullOrWhiteSpace(filePath)
            ? throw new ArgumentException("The file path cannot be null or whitespace.", nameof(filePath))
            : ParseFromSystemVerilogFilePaths([filePath], slangExecutable, additionalArgs);
    }

    /// <summary>
    /// Compiles multiple SystemVerilog source files using slang and returns the resolved AST representation.
    /// </summary>
    /// <param name="filePaths">A collection of paths to the SystemVerilog source files to compile together.</param>
    /// <param name="slangExecutable">The name or absolute path of the slang executable. Defaults to "slang".</param>
    /// <param name="additionalArgs">Optional additional arguments to pass to slang.</param>
    /// <returns>The resolved <see cref="TopLevel"/> AST node.</returns>
    public static TopLevel ParseFromSystemVerilogFilePaths(
        IEnumerable<string> filePaths,
        string slangExecutable = "slang",
        IEnumerable<string>? additionalArgs = null)
    {
        if (filePaths == null)
        {
            throw new ArgumentNullException(nameof(filePaths), "The collection of file paths cannot be null.");
        }

        var arguments = new StringBuilder();
        var filesAdded = 0;

        foreach (var path in filePaths)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("File paths in the collection cannot be null or whitespace.");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The specified SystemVerilog source file could not be found: '{path}'",
                    path);
            }

            arguments.Append($"\"{path}\" ");
            filesAdded++;
        }

        if (filesAdded == 0)
        {
            throw new ArgumentException("The collection of file paths must contain at least one valid path.",
                nameof(filePaths));
        }

        arguments.Append("--ast-json - --quiet");

        if (additionalArgs != null)
        {
            foreach (var arg in additionalArgs)
            {
                if (!string.IsNullOrWhiteSpace(arg))
                {
                    arguments.Append(' ').Append(arg);
                }
            }
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = slangExecutable,
            Arguments = arguments.ToString(),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException(
                    $"Failed to start the slang process using executable '{slangExecutable}'.");
            }

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            var json = outputTask.GetAwaiter().GetResult();
            var errors = errorTask.GetAwaiter().GetResult();

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidOperationException(
                    $"slang execution completed with exit code {process.ExitCode} but did not produce AST output.\nError Output:\n{errors}");
            }

            return SlangSerializer.Parse(json);
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not FileNotFoundException &&
                                   ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"An unexpected error occurred while running slang: {ex.Message}", ex);
        }
    }
}