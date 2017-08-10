using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DbUp.Builder;

namespace DbUp.Support.SqlServer
{
	public static class IayosDbUpExtensions
	{
		public static UpgradeEngineBuilder WithCleanFilenameOnlySqlScriptsEmbeddedInAssemblies(
			this UpgradeEngineBuilder builder, List<Assembly> assemblies, Func<string, bool> filter = null)
		{
			return builder.WithScripts(new EmbeddedFilenameOnlySqlScriptsProvider(assemblies.ToArray(), filter, Encoding.Default));
		}


		public static UpgradeEngineBuilder WithCleanClassnameOnlyCodeScriptsEmbeddedInAssemblies(
			this UpgradeEngineBuilder builder, List<Assembly> assemblies)
		{
			return builder.WithScripts(new EmbeddedClassnameOnlyCodeScriptsProvider(assemblies.ToArray()));
		}


		public static UpgradeEngineBuilder WithCleanFilenameSqlAndClassnameCodeAndFileFolderScripts(this UpgradeEngineBuilder builder,
			List<Assembly> assembliesToScan = null, string fileSystemScriptFolder = null)
		{
			return builder.WithScripts(
				new CleanNameSqlAndCodeAndFolderScriptsProvider(assembliesToScan?.ToArray(), assembliesToScan?.ToArray(), null,
					fileSystemScriptFolder)
			);
		}


		public static UpgradeEngineBuilder WithCleanFilenameSqlAndClassnameCodeAndFileFolderScripts(this UpgradeEngineBuilder builder,
			List<Assembly> codeAssembliesToScan = null, List<Assembly> sqlAssembliesToScan = null,
			string fileSystemScriptFolder = null)
		{
			return builder.WithScripts(
				new CleanNameSqlAndCodeAndFolderScriptsProvider(codeAssembliesToScan?.ToArray(), sqlAssembliesToScan?.ToArray(),
					null, fileSystemScriptFolder)
			);
		}
	}
}
