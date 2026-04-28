If the Project Does not Initially Run
Ensure NuGet packages are restored.
If you encounter a `System.DllNotFoundException` for `SQLite.Interop.dll` on startup, this is a Visual Studio NuGet restore issue with SQLite native libraries. 
To resolve: Simply Clean Solution, followed by Rebuilding the Solution. This will force MSBuild to copy the required x86/x64 interop folders into the `bin/Debug` directory.
