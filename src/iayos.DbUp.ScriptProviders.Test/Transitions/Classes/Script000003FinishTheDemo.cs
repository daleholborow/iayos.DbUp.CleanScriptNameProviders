using System;
using System.Data;
using DbUp.Engine;

namespace iayos.DbUp.ScriptProviders.Test.Transitions.Classes
{
	public class Script000003FinishTheDemo : IScript
	{
		public string ProvideScript(Func<IDbCommand> dbCommandFactory)
		{
			// Do some stuff to initialize the database and migrate data blah blah blah

			// Empty when done, no idea what dbup is using this for?
			return string.Empty;
		}
	}
}
