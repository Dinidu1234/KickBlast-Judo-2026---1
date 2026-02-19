# FixBuildAndRun

1. Set **KickBlastStudentUI** as Startup Project.
2. Run **Build > Clean Solution** and then **Build > Rebuild Solution**.
3. If needed, close Visual Studio and delete `bin` and `obj` folders inside `KickBlastStudentUI`.
4. Verify `KickBlastStudentUI/Properties/launchSettings.json` has only:
   - `commandName` = `Project`
   - no `executablePath`
5. Use **Debug** configuration and **Any CPU** platform.
