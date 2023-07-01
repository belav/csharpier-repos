using System.Runtime.InteropServices;

// we only ever p/invoke into DLLs known to be in the System32 folder
[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
