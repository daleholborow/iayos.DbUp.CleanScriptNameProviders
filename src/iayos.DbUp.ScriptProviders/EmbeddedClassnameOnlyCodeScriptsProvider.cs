using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.Support.SqlServer
{
	/// <summary>
	/// Load up all class-based script providers and apply them using ONLY THE CLASSNAME, NOT the full namespace and folder path!
	/// An enhanced <see cref="IScriptProvider"/> implementation which retrieves upgrade scripts or IScript code upgrade scripts embedded in an assembly.
	/// </summary>
	public class EmbeddedClassnameOnlyCodeScriptsProvider : IScriptProvider
	{
		private readonly Assembly[] _assemblies;

		/// <summary>
		///     Initializes a new instance of the <see cref="EmbeddedClassnameOnlyCodeScriptsProvider" /> class.
		/// </summary>
		/// <param name="assemblies">The assemblies to search.</param>
		public EmbeddedClassnameOnlyCodeScriptsProvider(Assembly[] assemblies)
		{
			_assemblies = assemblies;
		}


		/// <summary>
		/// Gets all scripts that should be executed.
		/// </summary>
		public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
		{
			var script = typeof(IScript);
			var codeScripts = connectionManager.ExecuteCommandsWithManagedConnection(dbCommandFactory => _assemblies.SelectMany(a =>
				a.GetTypes()
					.Where(type => script.IsAssignableFrom(type) && type.IsClass)
					.Select(s => (SqlScript)new LazySqlScript(s.Name + ".cs", () => ((IScript)Activator.CreateInstance(s)).ProvideScript(dbCommandFactory)))
					.ToList()));
			return codeScripts;
		}
	}
}