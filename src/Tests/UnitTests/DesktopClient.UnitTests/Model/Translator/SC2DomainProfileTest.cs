using System;
using System.Text;
using System.Collections.Generic;

using AutoMapper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SC2LiquipediaStatistics.DesktopClient.Model.Translator;

namespace DesktopClient.UnitTests.Model.Translator
{
    /// <summary>
    /// Summary description for SC2DomainProfileTest
    /// </summary>
    [TestClass]
    public class SC2DomainProfileTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
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

        [TestMethod]
        public void SC2DomainProfile_should_be_valid()
        {
            var configuration = new MapperConfiguration(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new SC2DomainProfile());
            });
            configuration.AssertConfigurationIsValid("SC2LiquipediaStatistics.DesktopClient.Model.Translator.SC2DomainProfile");
        }
    }
}
