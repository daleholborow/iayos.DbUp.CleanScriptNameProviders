using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using DbUp.Engine.Transactions;
using DbUp.ScriptProviders;

namespace DbUp.Support.SqlServer
{
	public class CleanNameSqlAndCodeAndFolderScriptsProvider : IScriptProvider
	{

		private readonly EmbeddedFilenameOnlySqlScriptsProvider _embeddedFilenameOnlySqlScriptsProvider;
		private readonly EmbeddedClassnameOnlyCodeScriptsProvider _embeddedClassnameOnlyCodeScriptsProvider;
		private readonly FileSystemScriptProvider _fileSystemScriptProvider;

		public CleanNameSqlAndCodeAndFolderScriptsProvider(
			Assembly[] codeAssemblies,
			Assembly[] sqlAssemblies,
			Func<string, bool> sqlFilter = null,
			string directoryPath = null
		)
		{
			_embeddedFilenameOnlySqlScriptsProvider = new EmbeddedFilenameOnlySqlScriptsProvider(sqlAssemblies, sqlFilter);
			_embeddedClassnameOnlyCodeScriptsProvider = new EmbeddedClassnameOnlyCodeScriptsProvider(codeAssemblies);
			_fileSystemScriptProvider = (directoryPath?.Trim().Length == 0) ? null : new FileSystemScriptProvider(directoryPath);
		}


		public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
		{
			var sqlScripts = _embeddedFilenameOnlySqlScriptsProvider.GetScripts(connectionManager).ToList();
			var codeScripts = _embeddedClassnameOnlyCodeScriptsProvider.GetScripts(connectionManager);
			var fileScripts = _fileSystemScriptProvider?.GetScripts(connectionManager) ?? new List<SqlScript>();

			var allScripts = sqlScripts.Union(codeScripts).Union(fileScripts)
				.OrderBy(x => x.Name)
				.ToList();
			return allScripts;

		}
	}
}