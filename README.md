# iayos.DbUp.ScriptProviders

Script Providers for DbUp to remove namespace and folder information from embedded .sql files and .cs migration scripts. 

Specifically, it offers a new

- EmbeddedClassnameOnlyCodeScriptsProvider
- EmbeddedFilenameOnlySqlScriptsProvider, and
- CleanNameSqlAndCodeAndFolderScriptsProvider (to combine the two embedded types with the filepath provider included within the original DbUp package)

The original idea (and author of several of the concepts) can be found here: https://github.com/DbUp/DbUp/issues/166

Basically, I want migration scripts to be

- available from multiple assemblies
- applied in alphabetical order, based solely on file/class name, so that
- relocating a file or refactoring the namespace does NOT result in scripts being applied again.

### Caveats

- The logic expects / assumes that ALL migration scripts (whether .cs or .sql) are in the format {filename}.{fileext} [NOTE: with only ONE period, separating the two components).

Package is [available on NuGet](https://www.nuget.org/packages/iayos.DbUp.ScriptProviders/) as `iayos.DbUp.ScriptProviders`. Install it from NuGet Package Manager Console with:
	
~~~~
Install-Package iayos.DbUp.ScriptProviders
~~~~

### Final Notes & TODOs:

- The test case [TestCleanNameSqlAndCodeAndFolderScriptsProvider] demonstrates how scripts can be of different type (.sql and .cs), can be in different assemblies, and can be embedded or filesystem-based, but will be applied in alphabetical order regardless. However, the test case currently fails, something to do with the way the connection manager is being initialized there I think, but the code works. I'll investigate the test case failure at some point, or feel free to issue a pull request.