Set WshShell = CreateObject("WScript.Shell") 
dim fso: set fso = CreateObject("Scripting.FileSystemObject")
path = Wscript.ScriptFullName
dim CurrentDirectory
file = fso.GetFile(path)
directory = fso.GetParentFolderName(file)

WshShell.Run chr(34) & directory & "\run.bat" & Chr(34), 0
Set WshShell = Nothing
