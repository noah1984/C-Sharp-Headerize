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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace noah1984.Headerize
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set to backup files
            bool backupFlag = true;
            // Directory was passed as argument
            bool directoryArgument = false;
            // Indicate having displayed help
            bool displayedHelp = false;
            // Tracks if there are multiple header arguments (not allowed)
            bool headerArgFlag = false;
            // Indicate invalid arguments
            bool invalidArgs = false;
            // Flag indicating a file was modified
            bool updatedSomething = false;
            // Set to replace files without prompting
            bool replaceFlag = false;
            // Restore files from backup
            bool restoreFlag = false;
            // Stores .cs files in the Visual Studio Project directory + subdirectories
            List<string> codeFiles = new List<string>();
            // Directory that contains the Visual Studio .sln file
            string directory = string.Empty;
            // Header file located in executable directory
            string headerFile = "header.txt";
            // Command line arguments were passed in



            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    if (args[i].Length > 1)
                    {
                        if (args[i][0] == '-' || args[i][0] == '/')
                        {

                            switch (args[i][1])
                            {
                                case 'b':
                                case 'B':
                                    if (args[i].Length == 2)
                                    {
                                        restoreFlag = true;
                                    }
                                    else
                                    {
                                        // Display message informing the user of invalid input
                                        Console.WriteLine("Parameter format not correct - \"" + args[i].Substring(1) + "\"");
                                        invalidArgs = true;
                                    }
                                    break;
                                case '?': //Help me Ronda
                                    // Display helpful information, indicating proper syntax, to the Console
                                    Console.WriteLine("Adds headers to beginning of .cs files in a Visual Studio Project directory");
                                    Console.WriteLine();
                                    Console.WriteLine("HEADERIZE [drive:]path [/B] [/D] [/H [drive:][path][filename]] [/R]");
                                    Console.WriteLine();
                                    Console.WriteLine("  [drive:]path");
                                    Console.WriteLine("              Specifies directory of Visual Studio project to process");
                                    Console.WriteLine();
                                    Console.WriteLine("  /B          Restore files from backup");
                                    Console.WriteLine("  /D          DO NOT backup modified files");
                                    Console.WriteLine("  /H          Specifies location of header file");
                                    Console.WriteLine("  /R          Replace pre-existing headers");
                                    Console.WriteLine();
                                    // This flag short circuits the program
                                    displayedHelp = true;
                                    break;
                                case 'd':
                                case 'D':
                                case 'h':
                                case 'H':
                                case 'r':
                                case 'R':
                                    break;
                                // Unknown parameter
                                default:
                                    // Extract meaningless command to rub back in the Users face
                                    int endOfArg = args[i].Length;
                                    int spaceLocation = args[i].IndexOf(' ');

                                    if (spaceLocation > 0)
                                    {
                                        endOfArg = spaceLocation;
                                    }
                                    // Rub it
                                    Console.WriteLine("Invalid switch - \"" + args[i].Substring(1, endOfArg - 1) + "\"");
                                    invalidArgs = true;
                                    break;
                            }
                            if(invalidArgs || displayedHelp)
                            {
                                break;
                            }
                        }
                    }
                }
                if (!displayedHelp && !invalidArgs)
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
                                if (args[i][1] != 'b' && args[i][1] != 'B')
                                {
                                    if (!restoreFlag)
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
                                                    Console.WriteLine("Parameter format not correct - \"" + args[i].Substring(1) + "\"");
                                                    invalidArgs = true;
                                                }
                                                break;
                                            // Specify header file
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
                                                    Console.WriteLine("Invalid argument - Only one header file is allowed");
                                                    invalidArgs = true;
                                                }
                                                // Used for ensuring only one header argument
                                                headerArgFlag = true;
                                                break;
                                            // Replace pre-existing headers
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
                                                    Console.WriteLine("Parameter format not correct - \"" + args[i].Substring(1) + "\"");
                                                    invalidArgs = true;
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        // Display message informing the user that they cannot combine other arguments
                                        // with restoring from backup argument
                                        Console.WriteLine("Invalid argument - Restoring from backup is exclusive");
                                        invalidArgs = true;
                                    }
                                    // Break out of loop if there is already a problem or help was displayed
                                    if (invalidArgs)
                                    {
                                        break;
                                    }
                                }
                            }
                            else // If it’s not a command then it’s a directory (hopefully)
                            {
                                // Passing in multiple directories is not allowed
                                if (directoryArgument)
                                {
                                    // Display message informing the user of invalid input
                                    Console.WriteLine("Invalid argument - Only one directory is allowed");
                                    invalidArgs = true;
                                }
                                else // User has not entered a directory yet
                                {
                                    directory = args[i];
                                    if (!Directory.Exists(directory))
                                    {
                                        // Display message informing the user that the directory does not exist
                                        Console.WriteLine("Invalid argument - Directory not found");
                                        invalidArgs = true;
                                    }
                                    else if (Directory.GetFiles(directory, "*.sln").Length == 0) // Check for .sln file
                                    {
                                        // Display message informing the user that a Visual Studio Project solution was not found   
                                        Console.WriteLine("Invalid argument - Visual Studio Project solution not found in directory");
                                        invalidArgs = true;
                                    }
                                    else
                                    {
                                        // Populate list with all .cs files in the Visual Studio Project directory + subdirectories
                                        codeFiles = CodeFilesInDirectory(directory);
                                        for (int j = 0; j < codeFiles.Count; ++j)
                                        {
                                            // Store file information to retrieve length
                                            FileInfo codeFileInformation = new FileInfo(codeFiles[j]);
                                            // As long as the file length is greater than 0
                                            if (codeFileInformation.Length > 0)
                                            {
                                                // It is pointless to add header information to "AssemblyInfo.cs"
                                                if (Path.GetFileName(codeFiles[j]).ToLower() == "assemblyinfo.cs")
                                                {
                                                    codeFiles.RemoveAt(j);
                                                }
                                            }
                                            else
                                            {
                                                codeFiles.RemoveAt(j);
                                            }
                                        }
                                        if (codeFiles.Count == 0)
                                        {
                                            Console.WriteLine("Invalid argument - No elidgable code files found in directory");
                                            invalidArgs = true;
                                        }
                                    }
                                }
                                directoryArgument = true;
                                // Break out of loop if there is already a problem 
                                if (invalidArgs)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (restoreFlag && !invalidArgs && !displayedHelp)
            {
                int statusOfBackup = BackupStatus(directory);
                if (statusOfBackup == -1)
                {
                    Console.WriteLine("Invalid argument - Backup directory not found for project");
                    invalidArgs = true;
                }
                else if (statusOfBackup == -2)
                {
                    Console.WriteLine("Invalid argument - AssemlyInfo.cs file detected in backup directory");
                    invalidArgs = true;
                }
                else if (statusOfBackup == -3)
                {
                    Console.WriteLine("Invalid argument - Empty code file detected in backup directory");
                    invalidArgs = true;
                }
                else if (statusOfBackup == -4)
                {
                    Console.WriteLine("Invalid argument - No code files detected in backup directory");
                    invalidArgs = true;
                }
                else
                {
                    // Setting this to string.Empty also works as a flag
                    string userInput = string.Empty;
                    while (userInput == string.Empty)
                    {
                        // Solicit input from user to determine if they wish to replace the file
                        Console.Write("Restore files from backup?: ");
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
                                string backupPath = "Backup\\" + new DirectoryInfo(directory).Name;
                                List<string> backupFiles = CodeFilesInDirectory(backupPath);
                                for (int i = 0; i < backupFiles.Count; ++i)
                                {
                                    string relativeFilename = GetRelativePath("Backup\\" + new DirectoryInfo(directory).Name, backupFiles[i]);
                                    if (File.Exists(directory + "\\" + relativeFilename))
                                    {
                                        string[] tempLines = File.ReadAllLines(directory + "\\" + relativeFilename);
                                        File.Copy(backupFiles[i], directory + "\\" + relativeFilename, true);
                                        File.WriteAllLines(backupFiles[i], tempLines);
                                    }
                                    else
                                    {
                                        File.Move(backupFiles[i], directory + "\\" + relativeFilename);
                                    }
                                    Console.WriteLine("Restored file: " + directory + "\\" + relativeFilename);
                                    updatedSomething = true;
                                }
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
            }
            if (!invalidArgs && !displayedHelp) // If the program has not short circuited from invalid arguments or displaying help
            {
                if(!restoreFlag)
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
                            if (!Directory.Exists(directory))
                            {
                                // Display message informing the user that the directory does not exist
                                Console.WriteLine("Directory not found");
                                inputFlag = false;
                            }
                            else if (Directory.GetFiles(directory, "*.sln").Length == 0) // Check for .sln file
                            {
                                // Display message informing the user that a Visual Studio Project solution was not found   
                                Console.WriteLine("Visual Studio Project solution not found in directory");
                                inputFlag = false;
                            }
                            else
                            {
                                // Populate list with all .cs files in the Visual Studio Project directory + subdirectories
                                codeFiles = CodeFilesInDirectory(directory);
                                for (int i = 0; i < codeFiles.Count; ++i)
                                {
                                    // Store file information to retrieve length
                                    FileInfo codeFileInformation = new FileInfo(codeFiles[i]);
                                    // As long as the file length is greater than 0
                                    if (codeFileInformation.Length > 0)
                                    {
                                        // It is pointless to add header information to "AssemblyInfo.cs"
                                        if (Path.GetFileName(codeFiles[i]).ToLower() == "assemblyinfo.cs")
                                        {
                                            codeFiles.RemoveAt(i);
                                        }
                                    }
                                    else
                                    {
                                        codeFiles.RemoveAt(i);
                                    }
                                }
                                if (codeFiles.Count == 0)
                                {
                                    Console.WriteLine("No elidgable code files found in directory");
                                    inputFlag = false;
                                }
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
                    // Read header file into string array
                    // Each element corresponds to a line from the text file
                    string[] headerFileLines = File.ReadAllLines(headerFile);
                    // Verify that the header file is in the proper format
                    // All lines of the header must begin with '/'
                    if (ValidHeader(headerFileLines))
                    {
                        // Store the location of the last occupied element + 1
                        int adjustedHeaderEntries = headerFileLines.Length;
                        // Loop backwards from the last element until an occupied element is located
                        while (headerFileLines[adjustedHeaderEntries - 1].Length == 0 && adjustedHeaderEntries > 0)
                        {
                            --adjustedHeaderEntries;
                        }
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
                                                // Stored in executable path\Backup\project name\relative file path\filename
                                                string backupFilePath = "Backup\\" + new DirectoryInfo(directory).Name + "\\" + GetRelativePath(directory, codeFiles[i]);
                                                string backupFileDirectory = Path.GetDirectoryName(backupFilePath);
                                                // Check if directory exists for path to the backup file
                                                if (!Directory.Exists(backupFileDirectory))
                                                {
                                                    // Create directory if it does not exist
                                                    Directory.CreateDirectory(backupFileDirectory);
                                                }
                                                // Copy file to backup location and overwrite if necessary
                                                File.Copy(codeFiles[i], backupFilePath, true);
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
                    }
                    else // Header file is not in proper format
                    {
                        Console.WriteLine("Header file was not in proper format");
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
        public static bool ValidHeader(string[] headerArray)
        {
            for(int i=0;i<headerArray.Length; ++i)
            {
                if(!ValidHeaderLine(headerArray[i]))
                {
                    return false;
                }
            }
            return true;
        }
        // Determine if string is in the proper header format
        public static bool ValidHeaderLine(string headerLine)
        {
            // Loop through each character of the header line
            for (int i = 0; i < headerLine.Length; ++i)
            {
                // Ignore whitespace (space and horizontal tab)
                if (headerLine[i] != ' ' && headerLine[i] != 9)
                {
                    // If the first non-whitespace character encountered is the
                    // '/' (comment) character then it is a valid header line
                    if (headerLine[i] == '/')
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            // Blank line and whitespace is considered valid header
            return true;
        }
        public static int BackupStatus(string directory)
        {
            string backupPath = "Backup\\" + new DirectoryInfo(directory).Name;
            if (!Directory.Exists(backupPath))
            {
                return -1;
            }
            List<string> backupFiles = CodeFilesInDirectory(backupPath);
            for (int i = 0; i < backupFiles.Count; ++i)
            {
                FileInfo codeFileInformation = new FileInfo(backupFiles[i]);
                if (codeFileInformation.Length > 0)
                {
                    if (Path.GetFileName(backupFiles[i]).ToLower() == "assemblyinfo.cs")
                    {
                        return -2;
                    }
                }
                else
                {
                    return -3;
                }
            }
            if (backupFiles.Count == 0)
            {
                return -4;
            }
            return 0;
        }
    }
}