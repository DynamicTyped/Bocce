using System;
using System.Configuration;
using System.Globalization;
using Bocce.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Resources;
using System.Threading;

namespace Bocce.Test
{
	/// <summary>
	/// Summary description for DbResourceProviderTest
	/// </summary>
	[TestClass]
	public class DbResourceProviderTest
	{
		#region Generic Code

	    /// <summary>
	    ///Gets or sets the test context which provides
	    ///information about and functionality for the current test run.
	    ///</summary>
	    public TestContext TestContext { get; set; }

	    #region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            
            UnitTestHelper.CleanUpTable();
            UnitTestHelper.SetupTable();
        }

		//
		// Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            UnitTestHelper.CleanUp();
        }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		#endregion Generic Code

		private static DbResourceProvider CreateDefaultResourceProvider(string resourceType = "DBResourceProviderTest")
		{
            var config = DbResourceProviderSection.GetSection();

			return new DbResourceProvider(
				resourceType,
				CultureInfo.InstalledUICulture,
                new DbResourceAccessor(ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString, config.SchemaName, config.TableName));
		}

		/// <summary>
		/// Tests the method "GetObject()" of "DBResourceProvider" class that retrieves a resource entry based 
		/// on the specified culture, resource type and resource key.
		/// </summary>
		[TestMethod]
		public void get_object_does_not_fallback()
		{
			var cultureInfo = new CultureInfo("fr-FR");
			const string resourceKey = "get_object_does_not_fallback";

			var dBResourceProvider = CreateDefaultResourceProvider();
			var resourceValue = dBResourceProvider.GetObject(resourceKey, cultureInfo);

			Assert.AreEqual("w00t!", resourceValue);
		}

		/// <summary>
		/// Tests the return of appropriate error message from method "GetObject()" of "DBResourceProvider" class when 
		/// culture info is not passed.
		/// </summary>
		[TestMethod]
		public void get_object_with_null_culture_info_uses_current_ui_culture()
		{
			Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("fr-FR");
			var dBResourceProvider = CreateDefaultResourceProvider();
			var resourceValue = dBResourceProvider.GetObject("get_object_with_null_culture_info_uses_current_ui_culture", null);

			Assert.AreEqual(resourceValue, "w00t!");
		}

		/// <summary>
		/// Tests the return of appropriate error message from method "GetObject()" of "DBResourceProvider" class when 
		/// culture info is not passed.
		/// </summary>
		[TestMethod]
		public void get_object_falls_back_to_neutral_culture()
		{
			var cultureInfo = CultureInfo.GetCultureInfo("fr-FR");
			const string resourceKey = "get_object_falls_back_to_neutral_culture";

			var dBResourceProvider = CreateDefaultResourceProvider();
			var resourceValue = dBResourceProvider.GetObject(resourceKey, cultureInfo);

			Assert.AreEqual("w00t!", resourceValue);
		}

		/// <summary>
		/// Tests the return of appropriate error message from method "GetObject()" of "DBResourceProvider" class when 
		/// culture info is not passed.
		/// </summary>
		[TestMethod]
		public void get_object_falls_back_to_default_culture()
		{
			var cultureInfo = CultureInfo.GetCultureInfo("fr-FR");
			const string resourceKey = "get_object_falls_back_to_default_culture";

			var dBResourceProvider = CreateDefaultResourceProvider();
			var resourceValue = dBResourceProvider.GetObject(resourceKey, cultureInfo);

			Assert.AreEqual("w00t!", resourceValue);
		}

		/// <summary>
		/// Tests the return of appropriate error message from method "GetObject()" of "DBResourceProvider" class when 
		/// ResourceKey is empty.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void get_object_throws_argument_null_exception_when_resource_key_is_null()
		{
			var dBResourceProvider = CreateDefaultResourceProvider();
			dBResourceProvider.GetObject(null, CultureInfo.CurrentUICulture);
		}

		/// <summary>
		/// Tests the return of appropriate error message from method "GetObject()" of "DBResourceProvider" class when 
		/// ResourceType is empty.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void constructor_throws_argument_null_exception_if_resource_type_is_null()
		{
			CreateDefaultResourceProvider(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void constructor_throws_argument_null_exception_if_default_culture_is_null()
		{
			new DbResourceProvider("Don't Care", null, new DbResourceAccessor("Don't Care", "schema", "table"));
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void constructor_throws_argument_null_exception_if_accessor_is_null()
		{
			new DbResourceProvider("Don't Care", CultureInfo.InstalledUICulture, null);
		}

		/// <summary>
		/// Tests the return of appropriate error message from method "GetObject()" of "DBResourceProvider" class when 
		/// ResourceType is empty.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void constructor_throws_argument_null_exception_if_resource_type_is_empty()
		{
			CreateDefaultResourceProvider(string.Empty);
		}

		/// <summary>
		/// Tests the return of appropriate error message from method "GetObject()" of "DBResourceProvider" class when 
		/// wrong culture info is passed.
		/// </summary>
		[TestMethod]
		public void get_object_returns_null_if_resource_key_is_not_found()
		{
			var cultureInfo = new CultureInfo("en-US");
			const string resourceKey = "Bogus!";

			var dBResourceProvider = CreateDefaultResourceProvider();
			var resourceValue = dBResourceProvider.GetObject(resourceKey, cultureInfo);

			Assert.IsNull(resourceValue);
		}

		/// <summary>
		/// Tests the method "GetObject()" to return the entire list of resources in the database. 
		/// </summary>
		[TestMethod]
		public void clear_cache_removes_all_resources()
		{
			var cultureInfo = new CultureInfo("fr-FR");

		    var dbResourceProvider = CreateDefaultResourceProvider();

			// Retrieve a value to force the population of the cache.
			const string resourceKey = "Nobody cares";
			dbResourceProvider.GetObject(resourceKey, cultureInfo);

			if (dbResourceProvider.CacheCount == 0)
			{
				Assert.Inconclusive("No values were cached.");
			}

			// Clear the cache
			dbResourceProvider.ClearCache();

			Assert.AreEqual(0, dbResourceProvider.CacheCount, "The cache still contains items.");
		}

        [TestMethod]
        public void CacheTest()
        {
            var cultureInfo = new CultureInfo("fr-FR");
            const string resourceKey = "get_object_does_not_fallback";

            var dBResourceProvider = CreateDefaultResourceProvider();
            var resourceValue = dBResourceProvider.GetObject(resourceKey, cultureInfo);
            var resourceValue2 = dBResourceProvider.GetObject(resourceKey, cultureInfo);

            Assert.Inconclusive();

        }

		[TestMethod]
		public void get_resource_reader()
		{
			var resourceProvider = CreateDefaultResourceProvider();
			object reader = resourceProvider.ResourceReader;

			Assert.IsInstanceOfType(reader, typeof(IResourceReader));
		}

		/// <summary>
		/// Tests the method "GetObject()" to return the entire list of resources in the database. 
		/// </summary>
		[TestMethod]
		public void clear_all()
		{
			var providers = new[]
				{
					ClearAllSetup(),
					ClearAllSetup()
				};

			// Clear the cache
			DbResourceProvider.ClearAll();

			foreach (var provider in providers)
			{
				Assert.AreEqual(0, provider.CacheCount, "The cache still contains items.");
			}
		}

		private static DbResourceProvider ClearAllSetup()
		{
		    var dBResourceProvider = CreateDefaultResourceProvider();

			// Retrieve a value to force the population of the cache.
			const string resourceKey = "Nobody cares";
			dBResourceProvider.GetObject(resourceKey, new CultureInfo("en-US"));

			return dBResourceProvider;
		}
	}
}