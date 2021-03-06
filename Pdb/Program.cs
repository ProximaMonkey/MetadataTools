﻿using System;
using System.IO;

namespace MetadataTools
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            PdbInfo.LogAction = Log;

            if (args.Length == 2)
            {
                if (args[0].EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                    args[0].EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    var dll = Path.GetFullPath(args[0]);
                    if (!File.Exists(dll))
                    {
                        Error("dll file not found: " + dll);
                        return;
                    }

                    if (args[1].EndsWith(".pdb", StringComparison.OrdinalIgnoreCase))
                    {
                        var pdb = Path.GetFullPath(args[1]);
                        if (!File.Exists(pdb))
                        {
                            Error("pdb file not found: " + pdb);
                            return;
                        }

                        CheckMatch(dll, pdb);
                    }

                    var directory = Path.GetFullPath(args[1]);
                    if (Directory.Exists(directory))
                    {
                        FindMatchingPdb(dll, directory);
                    }
                }
            }
        }

        private static void FindMatchingPdb(string dll, string directory)
        {
            var pdbs = Directory.GetFiles(
                directory,
                Path.GetFileNameWithoutExtension(dll) + ".pdb",
                SearchOption.AllDirectories);
            var debugDirectory = PdbInfo.Read(dll);
            foreach (var pdb in pdbs)
            {
                if (PdbInfo.IsMatch(debugDirectory, pdb))
                {
                    Log("Match: " + pdb, ConsoleColor.Green);
                }
            }
        }

        private static void CheckMatch(string dll, string pdb)
        {
            if (PdbInfo.IsMatch(dll, pdb))
            {
                Log("Match", ConsoleColor.Green);
            }
            else
            {
                Log("No match", ConsoleColor.Red);
            }
        }

        private static void Error(string text)
        {
            Console.Error.WriteLine(text);
        }

        private static void PrintHelp()
        {
            Log(@"Usage: Pdb.exe <input.dll> <input.pdb>
    Check if the .dll matches the .pdb.");
        }

        private static void Log(string text)
        {
            Log(text, ConsoleColor.DarkGray);
        }

        private static void Log(string text, ConsoleColor color)
        {
            lock (typeof(Program))
            {
                var oldColor = Console.ForegroundColor;
                if (oldColor != color)
                {
                    Console.ForegroundColor = color;
                }

                Console.WriteLine(text);

                if (oldColor != color)
                {
                    Console.ForegroundColor = oldColor;
                }
            }
        }
    }
}
