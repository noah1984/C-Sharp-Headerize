# C-Sharp-Headerize
Add headers to your Visual Studio projects

Adds headers to beginning of .cs files in a Visual Studio Project directory

HEADERIZE [drive:]path [/B] [/D] [/H [drive:][path][filename]] [/R]

  [drive:]path
              Specifies directory of Visual Studio project to process

  /B          Restore files from backup
  /D          DO NOT backup modified files
  /H          Specifies location of header file
  /R          Replace pre-existing headers

NOTE: Only supports "//", does not yet support "/*" comments.

V.02
12/14/2016

Fixed a bug that would occur if the header file was improperly formatted.
Added the option to restore files from backup.


V.01
12/10/2016

I wrote this program to be helpful in adding licensing headers to my
Visual Studio C# projects. Perhaps someone else will find this useful.

Contains command line argument functionality, as well as working as a
typical Console Application.

The program prompts the user to enter the Visual Studio project
directory that they wish to modify. It then looks for a header.txt in
the executable directory, however it will prompt the user to enter a
filename if this file is not found or empty. Next it finds all the .cs
files in the directory and subdirectories of the project and adds the
header information to each file. It is set to create backups for all
overwritten files in <executable directory>/Backup/<project name>.