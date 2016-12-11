// Headerize: Add headers to your Visual Studio projects
// Copyright (C) 2016 Noah Allen
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.
//
// V.01
// 12/10/2016
//
// I wrote this program to be helpful in adding licensing headers to my
// Visual Studio C# projects. Perhaps someone else will find this useful.
//
// Contains command line argument functionality, as well as working as a
// typical Console Application.
// 
// The program prompts the user to enter the Visual Studio project
// directory that they wish to modify. It then looks for a header.txt in
// the executable directory, however it will prompt the user to enter a
// filename if this file is not found or empty. Next it finds all the .cs
// files in the directory and subdirectories of the project and adds the
// header information to each file. It is set to create backups for all
// overwritten files in <executable directory>/Backup/<project name>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Headerize
{
    class Program
    {
        static void Main(string[] args)
        {
            // Header file located in executable directory
            string headerFile = "header.txt";
            // Directory that contains the Visual Studio .sln file
            string directory = string.Empty;
            // Set to backup files
            bool backupFlag = true;
            // Tracks if there are multiple header arguments (not allowed)
            bool headerArgFlag = false;
            // Set to replace files without prompting
            bool replaceFlag = false;
            // Indicate invalid arguments
            bool invalidArgs = false;
            // Indicate having displayed help
            bool displayedHelp = false;
            // If command line arguments were passed in...
            if (args.Length > 0)
            {
                // Cycle through each argument
                for (int i = 0; i < args.Length; ++i)
                {
                    // Make sure argument is at least 2 characters
                    if (args[i].Length > 1)
                    {
                        // Check if first character of argument indicates command
                        if (args[i][0] == '-' || args[i][0] == '/')
                        {
                            // Determine action to take based on first character of command
                            switch (args[i][1])
                            {

                                case 'd': // Fall through to support both lower and upper case
                                case 'D':
                                    // As long as it is only 'd' or 'D'
                                    if (args[i].Length == 2)
                                    {
                                        // User DOES NOT wish to back up files
                                        backupFlag = false;
                                    }
                                    else
                                    {
                                        // Display message informing the user of invalid input
                                        Console.WriteLine("Parameter format not correct - " + args[i].Substring(1) + "\"");
                                        invalidArgs = true;
                                    }
                                    break;
                                case 'h': // Fall through to support both lower and upper case
                                case 'H':
                                    // This enforces allowing only one header argument
                                    if (!headerArgFlag)
                                    {
                                        // Ensure a file name was supplied
                                        if (args[i].Length > 2)
                                        {
                                            // Ensure space character is in proper location
                                            if (args[i][2] == ' ')
                                            {
                                                // Extract filename from substring
                                                headerFile = args[i].Substring(3);
                                                // Check if file exists
                                                if (File.Exists(headerFile))
                                                {
                                                    // Store file information to retrieve length
                                                    FileInfo headerFileInformation = new FileInfo(headerFile);
                                                    // If the file is empty then display message informing the user
                                                    if (headerFileInformation.Length == 0)
                                                    {
                                                        Console.WriteLine("Invalid argument - Header file is empty");
                                                        invalidArgs = true;
                                                    }
                                                }
                                                else
                                                {
                                                    // Display message informing the user that the file does not exist
                                                    Console.WriteLine("Invalid argument - Header file not found");
                                                    invalidArgs = true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Display message informing the user that only one header file is allowed
                                        Console.WriteLine("Invalid argument - only one header file is allowed");
                                        invalidArgs = true;
                                    }
                                    // Used for ensuring only one header argument
                                    headerArgFlag = true;
                                    break;
                                case 'r': // Fall through to support both lower and upper case
                                case 'R':
                                    // As long as it is only 'r' or 'R'
                                    if (args[i].Length == 2)
                                    {
                                        // User has selected to replace files without being prompted
                                        replaceFlag = true;
                                    }
                                    else
                                    {
                                        // Display message informing the user of invalid input
                                        Console.WriteLine("Parameter format not correct - " + args[i].Substring(1) + "\"");
                                        invalidArgs = true;
                                    }
                                    break;
                                case '?': // Help me Ronda :)
                                    // Display helpful information, indicating proper syntax, to the Console
                                    Console.WriteLine("Adds headers to beginning of .cs files in a Visual Studio Project directory");
                                    Console.WriteLine();
                                    Console.WriteLine("HEADERIZE [drive:]path [/H [drive:][path][filename]] [/R]");
                                    Console.WriteLine();
                                    Console.WriteLine("  [drive:]path");
                                    Console.WriteLine("              Specifies directory of Visual Studio project to process");
                                    Console.WriteLine();
                                    Console.WriteLine("  /D          DO NOT backup modified files");
                                    Console.WriteLine("  /H          Specifies location of header file");
                                    Console.WriteLine("  /R          Replace pre-existing headers");
                                    Console.WriteLine();
                                    // This flag short circuits the program
                                    displayedHelp = true;
                                    break;
                                default:
                                    // Extract meaningless command to rub back in the Users face
                                    int endOfArg = args[i].Length;
                                    int spaceLocation = args[i].IndexOf(' ');
                                    if (spaceLocation > 0)
                                    {
                                        endOfArg = spaceLocation;
                                    }
                                    // Rub it
                                    Console.WriteLine("Invalid switch - \"" + args[i].Substring(1, endOfArg) + "\"");
                                    invalidArgs = true;
                                    break;

                            }
                            // Break out of loop if there is already a problem 
                            if (invalidArgs)
                            {
                                break;
                            }
                        }
                        else // If it’s not a command then it’s a directory (hopefully)
                        {
                            // A non-empty directory varaible indicates multiple directories having
                            // been passed in, which is not allowed
                            if (directory != string.Empty)
                            {
                                // Display message informing the user of invalid input
                                Console.WriteLine("Invalid argument - only one directory is allowed");
                                invalidArgs = true;
                                break;
                            }
                            else // User has not entered a directory yet
                            {
                                directory = args[i];
                                // Check if directory exists
                                if (!Directory.Exists(directory))
                                {
                                    // Display message informing the user of invalid input
                                    Console.WriteLine("Invalid argument - Directory not found");
                                    invalidArgs = true;
                                    break;
                                }
                                else if (Directory.GetFiles(directory, "*.sln").Length == 0) // Check for .sln file
                                {
                                    // Display message informing the user that a Visual Studio Project solution was not found
                                    Console.WriteLine("Invalid argument - Visual Studio Project solution not found");
                                    invalidArgs = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            // If the program has not short circuited from invalid arguments or displaying help
            if (!invalidArgs && !displayedHelp)
            {
                // If a directory was not passed as an argument
                if (directory == string.Empty)
                {
                    // This flag assists in validating user input
                    bool inputFlag = false;
                    while (!inputFlag)
                    {
                        inputFlag = true;
                        // Solicit input from user for the Visual Studio Project directory
                        Console.Write("Visual Studio Project directory: ");
                        // Read input into "directory" variable
                        directory = Console.ReadLine();
                        // Check if directory exists
                        if (!Directory.Exists(directory))
                        {
                            // Display message informing the user that the directory does not exist
                            Console.WriteLine("Directory not found");
                            inputFlag = false;
                        }
                        else if (Directory.GetFiles(directory, "*.sln").Length == 0) // Check for .sln file
                        {
                            // Display message informing the user that a Visual Studio Project solution was not found   
                            Console.WriteLine("Invalid argument - Visual Studio Project solution not found");
                            inputFlag = false;
                        }
                    }
                }
                // If header file was not passed as an argument
                if (!headerArgFlag)
                {
                    bool solicitHeaderFile = true;
                    // Check for default header file located in executable path
                    if (File.Exists(headerFile))
                    {
                        solicitHeaderFile = false;
                        // Store file information to retrieve length
                        FileInfo headerFileInformation = new FileInfo(headerFile);
                        // If the file is empty then trip flag to solicit input from user
                        if (headerFileInformation.Length == 0)
                        {
                            solicitHeaderFile = true;
                        }
                        else // Use header file in executable path
                        {
                            Console.WriteLine("Using \"" + headerFile + "\" from execuatable path");
                        }
                    }
                    if (solicitHeaderFile) // Did not find header file in executable path or it was empty
                    {
                        // This flag assits in validating user input
                        bool inputFlag = false;
                        while (!inputFlag)
                        {
                            inputFlag = true;
                            // Solicit input from user for the header file location
                            Console.Write("Header file: ");
                            // Read input into "headerFile" variable
                            headerFile = Console.ReadLine();
                            // If the user did not enter any input, it is possible that they copied
                            // header.txt into executable path and are expecting the program to figure
                            // it out
                            if (headerFile == string.Empty)
                            {
                                headerFile = "header.txt";
                            }
                            // Check if header file exists
                            if (File.Exists(headerFile))
                            {
                                // Store file information to retrieve length
                                FileInfo headerFileInformation = new FileInfo(headerFile);
                                // If the file is empty, display message informing the user
                                if (headerFileInformation.Length == 0)
                                {
                                    Console.WriteLine("Header file is empty");
                                    inputFlag = false;
                                }
                            }
                            else // Header file does not exist
                            {
                                Console.WriteLine("File not found");
                                inputFlag = false;
                            }
                        }
                    }
                }
                // Flag indicating a file was modified
                bool updatedSomething = false;
                // Read header file into string array
                // Each element corresponds to a line from the text file
                string[] headerFileLines = File.ReadAllLines(headerFile);
                // Store the location of the last occupied element + 1
                int adjustedHeaderEntries = headerFileLines.Length;
                // Loop backwards from the last element until an occupied element is located
                while (headerFileLines[adjustedHeaderEntries - 1].Length == 0 && adjustedHeaderEntries > 0)
                {
                    --adjustedHeaderEntries;
                }
                // Populate list with all .cs files in the Visual Studio Project directory + subdirectories
                List<string> codeFiles = CodeFilesInDirectory(directory);
                // Cycle through each code file element
                for (int i = 0; i < codeFiles.Count; ++i)
                {
                    // Store file information to retrieve length
                    FileInfo codeFileInformation = new FileInfo(codeFiles[i]);
                    // As long as the file length is greater than 0
                    if (codeFileInformation.Length > 0)
                    {
                        // It is pointless to add header information to "AssemblyInfo.cs"
                        if (Path.GetFileName(codeFiles[i]).ToLower() != "assemblyinfo.cs")
                        {
                            // Flag indicating the code file already contains a header
                            bool existingHeader = false;
                            // Read .cs file into string array
                            // Each element corresponds to a line from the file
                            string[] codeFileLines = File.ReadAllLines(codeFiles[i]);
                            // Build string containing the new header
                            string newCodeContents = String.Join(Environment.NewLine, headerFileLines, 0, adjustedHeaderEntries) + Environment.NewLine;
                            // As long as the first line in the .cs is not empty
                            if (codeFileLines[0].Length > 0)
                            {
                                // if the '/' is detected, then there is already a header
                                if (codeFileLines[0][0] == '/')
                                {
                                    existingHeader = true;
                                }
                            }
                            // Store index of the first line of code in the .cs file
                            int firstCodeLine = 0;
                            // Flag to indicate if file should be updated
                            bool updateFlag = true;
                            // If code file already contains a header
                            if (existingHeader)
                            {
                                // Set to be false, prove true if differences are detected between files
                                updateFlag = false;
                                // The incrementor "j" runs a parallel comparison of the
                                // header file and the code file
                                int j = 0;
                                for (; j < adjustedHeaderEntries && j < codeFileLines.Length; ++j)
                                {
                                    // If there is a difference detected at index "j"
                                    if (headerFileLines[j] != codeFileLines[j])
                                    {
                                        // The program stops comparing the header file and the code file
                                        // and moves on to its new mission: finding the first line of code
                                        // in the code file
                                        firstCodeLine = GetFirstCodeLine(codeFileLines, j);
                                        // Flag to update
                                        updateFlag = true;
                                        // Break out of comparing header file and code file
                                        break;
                                    }
                                }
                                // Comparison of the code file and header have been the same so far
                                if (!updateFlag)
                                {
                                    // Find the nearest line of code continuing where "j" left off
                                    firstCodeLine = GetFirstCodeLine(codeFileLines, j);
                                    // If the first line of code in the code file is the next line
                                    // then both files are identical
                                    if (firstCodeLine != j + 1)
                                    {
                                        // Flag to update
                                        updateFlag = true;
                                    }
                                }
                            }
                            // If there were differences in the new header and existing code file
                            if (updateFlag)
                            {
                                // Compose string with new header and existing code
                                newCodeContents += Environment.NewLine + String.Join(Environment.NewLine, codeFileLines, firstCodeLine, codeFileLines.Length - firstCodeLine);
                                // Flag which indicates the user's response of solicitation to replace file
                                bool performReplace = false;
                                // If the user has not selected to replace files without being prompted
                                if (!replaceFlag)
                                {
                                    // Setting this to string.Empty also works as a flag
                                    string userInput = string.Empty;
                                    while (userInput == string.Empty)
                                    {
                                        // Solicit input from user to determine if they wish to replace the file
                                        Console.Write("Update \"" + codeFiles[i] + "\"?: ");
                                        // Read input to "userInput" variable, converted to lower case for increased functionality
                                        userInput = Console.ReadLine().ToLower();
                                        // If the user has enter at least something as input
                                        if (userInput.Length > 0)
                                        {
                                            // The user did not enter either 'y' or 'n'
                                            if (userInput[0] != 'y' && userInput[0] != 'n')
                                            {
                                                // Display message informing the user that they must enter 'y' or 'n'
                                                Console.WriteLine("Invalid input  ('y' or 'n')");
                                                // Set userInput to empty as a flag
                                                userInput = string.Empty;
                                            }
                                            else if (userInput[0] == 'y')
                                            {
                                                // set flag to replace the file
                                                performReplace = true;
                                            }
                                            else
                                            {
                                                // Display message informing the user that they have selected
                                                // to bypass replacing the file
                                                Console.WriteLine("File byassed");
                                            }
                                        }
                                        else // The user did not enter anything
                                        {
                                            // Display message informing the user that they must enter 'y' or 'n'
                                            Console.WriteLine("Invalid input  ('y' or 'n')");
                                            // Set userInput to empty as a flag
                                            userInput = string.Empty;
                                        }
                                    }
                                }
                                else
                                {
                                    // perform a replace on the file since the user has selected to replace
                                    // files without being prompted
                                    performReplace = true;
                                }
                                // If the user has selected to replace the file
                                if (performReplace)
                                {
                                    // If the user has not selected to bypass backing up files
                                    if (backupFlag)
                                    {
                                        // Create variable for the backup file name
                                        // Stored in executable path\Backup\relative file path\filename
                                        string relativeFilePath = "Backup\\" + GetRelativePath(directory, codeFiles[i]);
                                        // Extract directory from the backup file path
                                        string relativeDirectoryPath = Path.GetDirectoryName(relativeFilePath);
                                        // Check if directory exists for path to the backup file
                                        if (!Directory.Exists(relativeDirectoryPath))
                                        {
                                            // Create directory if it does not exist
                                            Directory.CreateDirectory(relativeDirectoryPath);
                                        }
                                        // Copy file to backup location and overwrite if necessary
                                        File.Copy(codeFiles[i], relativeFilePath, true);
                                    }
                                    // If the option to replace the file was set through the command line argument
                                    if (replaceFlag)
                                    {
                                        // Display message informing the user that the file was updated
                                        // and which file was updated
                                        Console.WriteLine("Updated file: " + codeFiles[i]);
                                    }
                                    else
                                    {
                                        // Display message informing the user that the file was updated
                                        Console.WriteLine("File updated");
                                    }
                                    // Update the file
                                    File.WriteAllText(codeFiles[i], newCodeContents);
                                    // Set flag indicating at least one file was updated
                                    updatedSomething = true;
                                }
                            }
                        }
                    }
                }
                if (!updatedSomething) // No files were changed
                {
                    Console.WriteLine("No actions were performed");
                }
                // This is only printed if there were no invalid command line arguments and
                // the help argument was not passed in
                Console.WriteLine("Done!");
            }
            // If no command line arguments were passed in
            if (args.Length == 0)
            {
                // Read Console input to prevent the window from immediately closing
                Console.ReadLine();
            }
        }
        // Compares 2 paths and return the difference between them
        public static string GetRelativePath(string path1, string path2)
        {
            string shortPath;
            string longPath;
            if (path1.Length >= path2.Length)
            {
                longPath = path1;
                shortPath = path2;
            }
            else
            {
                longPath = path2;
                shortPath = path1;
            }
            int index = longPath.IndexOf(shortPath) + shortPath.Length;
            if (index < longPath.Length)
            {
                return longPath.Substring(index).TrimStart('\\');
            }
            return string.Empty;
        }
        // Returns .cs code files in specified directory and subdirectories
        public static List<string> CodeFilesInDirectory(string directory)
        {
            List<string> accumulatedFiles = new List<string>();
            accumulatedFiles.AddRange(Directory.GetFiles(directory, "*.cs").ToList());
            string[] subDirectories = Directory.GetDirectories(directory);
            for (int i = 0; i < subDirectories.Length; ++i)
            {
                accumulatedFiles.AddRange(CodeFilesInDirectory(subDirectories[i]));
            }
            return accumulatedFiles;
        }
        // Gets first line of code, as opposed to comments, in array
        public static int GetFirstCodeLine(string[] codeArray, int startIndex)
        {
            // Continue where the loop left off
            for (int j = startIndex; j < codeArray.Length; ++j)
            {
                // "k" is an incrementor for the characters in the current line
                // of code being examined in the code file
                for (int k = 0; k < codeArray[j].Length; ++k)
                {
                    // Ignore whitespace (space and horizontal tab)
                    if (codeArray[j][k] != ' ' && codeArray[j][k] != 9)
                    {
                        // If the first non-whitespace character encountered is not indicate
                        // the '/' (comment) character then it is code
                        if (codeArray[j][k] != '/')
                        {
                            // return current value of incrementor "j"
                            return j;
                        }
                        else // This line of the code file is still part of the header
                        {
                            // Break out of character examination
                            break;
                        }
                    }
                }
            }
            // First code line is beginning (0 serves the purpose of this program)
            return 0;
        }
    }
}