using Bocce.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;

namespace Bocce.Test
{
    /// <summary>
	///This is a test class for DbResourceProviderSectionTest and is intended
	///to contain all DBResourceProviderSectionTest Unit Tests
	///</summary>
	[TestClass]
	public class DbResourceProviderSectionTest
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		///A test for DBResourceProviderSection Constructor
		///</summary>
		[TestMethod]
		public void ConstructorTest()
		{
			object target = new DbResourceProviderSection();
			Assert.IsInstanceOfType(target, typeof(DbResourceProviderSection));
		}

		/// <summary>
		///A test for GetSection
		///</summary>
		[TestMethod]
		public void get_section()
		{
			var actual = DbResourceProviderSection.GetSection();
			Assert.IsInstanceOfType(actual, typeof(DbResourceProviderSection));
		}

		/// <summary>
		///A test for ConnectionString
		///</summary>
		[TestMethod]
		public void connection_string_returns_value_from_app_config()
		{
			var target = DbResourceProviderSection.GetSection();
            var expected = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
            var actual = target.ConnectionString;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for DatabaseName
		///</summary>
		[TestMethod]
		public void database_name_returns_value_from_app_config()
		{
			var target = DbResourceProviderSection.GetSection();
            const string expected = "SQLConnectionString";
			var actual = target.DatabaseName;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for DefaultCulture
		///</summary>
		[TestMethod]
		public void default_culture_returns_value_from_app_config()
		{
			var target = DbResourceProviderSection.GetSection();
			const string expected = "en-US";
			var actual = target.DefaultCulture;
			Assert.AreEqual(expected, actual);
		}
	}
}
