using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Bocce.Test
{
	/// <summary>
	///This is a test class for DbResourceAccessorTest and is intended
	///to contain all DbResourceAccessorTest Unit Tests
	///</summary>
	[TestClass]
	public class DbResourceAccessorTest
	{
	    /// <summary>
	    ///Gets or sets the test context which provides
	    ///information about and functionality for the current test run.
	    ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            UnitTestHelper.CleanUpTable();
            UnitTestHelper.SetupTable();
        }
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
        public static void MyClassCleanup()
        {
            UnitTestHelper.CleanUp();
        }
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		///A test for DBResourceAccessor Constructor
		///</summary>
		[TestMethod]
		public void ConstructorTest()
		{
			const string connectionString = "Something";
			object target = new DbResourceAccessor(connectionString);
			Assert.IsInstanceOfType(target, typeof(DbResourceAccessor));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void constructor_throws_argument_null_exception_when_connection_string_is_null()
		{
			new DbResourceAccessor(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void constructor_throws_argument_null_exception_when_connection_string_is_empty()
		{
			new DbResourceAccessor(string.Empty);
		}

		/// <summary>
		///A test for GetResources
		///</summary>
		[TestMethod]
		public void get_resources()
		{
			var target = new DbResourceAccessor(
                ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString);

			const string resourceType = UnitTestHelper.ResourceType;
			const string cultureCode = "en-US";

			var expected = new[]
				{
					new KeyValuePair<string,string>("get_object_does_not_fallback", "pwnd!"),
					new KeyValuePair<string,string>("get_object_falls_back_to_default_culture", "w00t!"),
                    new KeyValuePair<string,string>("get_object_falls_back_to_neutral_culture", "pwnd!")
				};

			var actual = target.GetResources(resourceType, cultureCode);

			Assert.IsTrue(expected.SequenceEqual(actual));
		}

		/// <summary>
		///A test for ConnectionString
		///</summary>
		[TestMethod]
		public void connection_string()
		{
			const string connectionString = "Don't Care";
			var target = new DbResourceAccessor(connectionString);
			const string expected = "New Value";
		    target.ConnectionString = expected;
			var actual = target.ConnectionString;
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for GetResourcesCommand
		///</summary>
		[TestMethod]
		public void get_resources_command()
		{
			const string connectionString = "Don't Care";
			var target = new DbResourceAccessor(connectionString);
			const string expected = "New Value";
		    target.GetResourcesCommand = expected;
			var actual = target.GetResourcesCommand;
			Assert.AreEqual(expected, actual);
		}
	}
}
