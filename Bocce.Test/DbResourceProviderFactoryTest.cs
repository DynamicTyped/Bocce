﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bocce.Test
{
	/// <summary>
	///This is a test class for DbResourceProviderFactoryTest and is intended
	///to contain all DbResourceProviderFactoryTest Unit Tests
	///</summary>
	[TestClass]
	public class DbResourceProviderFactoryTest
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
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
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
		///A test for DBResourceProviderFactory Constructor
		///</summary>
		[TestMethod]
		public void constructor_test()
		{
			object target = new DbResourceProviderFactory();
			Assert.IsInstanceOfType(target, typeof(DbResourceProviderFactory));
		}

		/// <summary>
		///A test for CreateGlobalResourceProvider
		///</summary>
		[TestMethod]
		public void CreateGlobalResourceProviderTest()
		{
			var target = new DbResourceProviderFactory();
			const string classKey = "DbResourceProviderFactoryTest";
			var actual = target.CreateGlobalResourceProvider(classKey);

			Assert.IsInstanceOfType(actual, typeof(DbResourceProvider));
		}

		/// <summary>
		///A test for CreateLocalResourceProvider
		///</summary>
		[TestMethod]
		public void CreateLocalResourceProviderTest()
		{
			var target = new DbResourceProviderFactory();
			const string virtualPath = "DbResourceProviderFactoryTest";
			var actual = target.CreateLocalResourceProvider(virtualPath);

			Assert.IsInstanceOfType(actual, typeof(DbResourceProvider));
		}
	}
}
