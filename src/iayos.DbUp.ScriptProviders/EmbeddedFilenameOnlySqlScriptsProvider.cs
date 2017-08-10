using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DbUp.Engine;
using DbUp.Engine.Transactions;

namespace DbUp.Support.SqlServer
{
	/// <summary>
	/// An <see cref="IScriptProvider" /> implementation which retrieves upgrade scripts embedded
	/// in assemblies, but rather than the default option of using the entire namespace and folder
	/// path etc, only the filename will be used.
	/// Makes assumption that all script files and classes contain ONLY one "." character, 
	/// e.g "00001DaleStartsTheDb.sql" and "00002DaleMakesChanges.cs" and never "00003Dale.Made.A.Mistake.sql"
	/// </summary>
	public class EmbeddedFilenameOnlySqlScriptsProvider : IScriptProvider
	{
		private readonly Assembly[] _assemblies;
		private readonly Encoding _encoding;
		private readonly Func<string, bool> _filter;

		/// <summary>
		///     Initializes a new instance of the <see cref="EmbeddedFilenameOnlySqlScriptsProvider" /> class.
		/// </summary>
		/// <param name="assemblies">The assemblies to search.</param>
		/// <param name="filter"></param>
		/// <param name="encoding">The encoding.</param>
		public EmbeddedFilenameOnlySqlScriptsProvider(Assembly[] assemblies, Func<string, bool> filter = null, Encoding encoding = null)
		{
			_assemblies = assemblies;
			_encoding = encoding ?? Encoding.Default;
			_filter = filter;
		}

		/// <summary>
		///     Gets all embedded .sql scripts that should be executed
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SqlScript> GetScripts(IConnectionManager connectionManager)
		{
			var assembliesResourceCollection = _assemblies
				.Select(assembly => new
				{
					Assembly = assembly,
					ResourceNames = assembly.GetManifestResourceNames()
					.Where(s => s.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase))
					.ToArray()
				});

			// Apply the filter if it was specified
			if (_filter != null)
			{
				assembliesResourceCollection =
					assembliesResourceCollection.Select(
						x => new
						{
							Assembly = x.Assembly,
							ResourceNames = x.ResourceNames.Where(_filter).ToArray()
						}
					);
			}

			var sqlScripts = assembliesResourceCollection.SelectMany(
					x =>
						x.ResourceNames.Select(
							resourceName =>
								FromStreamWithCleanScriptname(resourceName, x.Assembly.GetManifestResourceStream(resourceName), _encoding)))
				.OrderBy(sqlScript => sqlScript.Name)
				.ToList();

			return sqlScripts;
		}


		/// <summary>
		/// example taken from https://github.com/DbUp/DbUp/issues/166 
		/// Makes assumption that all script files and classes contain ONLY one "." character, 
		/// e.g "00001DaleStartsTheDb.sql" and "00002DaleMakesChanges.cs" and never "00003Dale.Made.A.Mistake.sql"
		/// </summary>
		/// <returns></returns>
		private SqlScript FromStreamWithCleanScriptname(string scriptName, Stream stream, Encoding encodingStyle)
		{
			using (var resourceStreamReader = new StreamReader(stream, encodingStyle, true))
			{
				var c = resourceStreamReader.ReadToEnd();

				// Drop the project name and any folder names by taking only the last two qualifiers of the scriptname (filename + ext [cs or sql])
				var pieces = scriptName.Split('.');
				var fileOnlyScriptName = new SqlScript(string.Join(".", pieces.Skip(pieces.Count() - 2)), c);
				return fileOnlyScriptName;
			}
		}

	}
}
