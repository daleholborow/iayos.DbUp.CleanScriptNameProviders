﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbUp.Builder;
using DbUp.Engine.Transactions;
using DbUp.Support.SqlServer;
using Xunit;

namespace iayos.DbUp.ScriptProviders.Test
{

	

	class Program
	{

		private static string _testConnString =
			"Server=localhost;Database=iayos.core.db.deploy.NUKEABLE;User Id=sa; Password=tH3dB4dm1n*5000;";

		static void Main(string[] args)
		{
			TestEmbeddedFilenameOnlySqlScriptsProvider();
			// next two fail the test because of something to do with the connectionmanager not
			// firing properly... but the code works .. have to investigate why test fails
			TestEmbeddedClassnameOnlyCodeScriptsProvider();
			TestCleanNameSqlAndCodeAndFolderScriptsProvider();
		}

		[Fact]
		public static void TestEmbeddedClassnameOnlyCodeScriptsProvider()
		{
			var upgradeEngineBuilder = new UpgradeEngineBuilder();
			IConnectionManager sqlConnectionManager = new SqlConnectionManager(_testConnString);
			var targetAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			var targetAssemblies = new List<Assembly> {targetAssembly};
			var embeddedClassnameOnlyCodeScriptsProvider =
				new EmbeddedClassnameOnlyCodeScriptsProvider(targetAssemblies.ToArray());
			var scripts = embeddedClassnameOnlyCodeScriptsProvider.GetScripts(sqlConnectionManager).ToList();
			Assert.True(scripts.Count() == 1);
			Assert.True(scripts.First().Name == "");
		}


		[Fact]
		public static void TestEmbeddedFilenameOnlySqlScriptsProvider()
		{
			var upgradeEngineBuilder = new UpgradeEngineBuilder();
			IConnectionManager sqlConnectionManager = new SqlConnectionManager(_testConnString);
			var targetAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			var targetAssemblies = new List<Assembly> {targetAssembly};
			var embeddedFilenameOnlySqlScriptsProvider = new EmbeddedFilenameOnlySqlScriptsProvider(targetAssemblies.ToArray());
			var scripts = embeddedFilenameOnlySqlScriptsProvider.GetScripts(sqlConnectionManager).ToList();
			Assert.True(scripts.Count() == 1);
			Assert.True(scripts.First().Name == "Script000001StartTheDemoWithAnEmbeddedScript.sql");
		}


		[Fact]
		public static void TestCleanNameSqlAndCodeAndFolderScriptsProvider()
		{
			var upgradeEngineBuilder = new UpgradeEngineBuilder();
			IConnectionManager sqlConnectionManager = new SqlConnectionManager(_testConnString);
			var targetAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			var targetAssemblies = new List<Assembly> {targetAssembly};
			var embeddedFilenameOnlySqlScriptsProvider = new CleanNameSqlAndCodeAndFolderScriptsProvider(
				targetAssemblies.ToArray(),
				targetAssemblies.ToArray(),
				null,
				@"..\..\Transitions\NotEmbeddedSql"
			);
			var scripts = embeddedFilenameOnlySqlScriptsProvider.GetScripts(sqlConnectionManager).ToList();
			Assert.True(scripts.Count() == 3);
			Assert.True(scripts.First().Name == "Script000001StartTheDemoWithAnEmbeddedScript.sql");
			Assert.True(scripts.Skip(1).First().Name == "Script000002NowDoSomeMoreStuffWithFileSystemScript.sql");
			Assert.True(scripts.Skip(2).First().Name == "Script000003FinishTheDemo.cs");
		}
	}
}
