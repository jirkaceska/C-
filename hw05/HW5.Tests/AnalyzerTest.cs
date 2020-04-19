using System;
using HW5.Enums;
using HW5.LogManipulators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HW5.Tests
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void Test_GetNumberOfClassStatusCodes_OnEmptyFile()
        {
            //Arrange
            string testedFilePath = TestFiles.CreateTempFile(@"..\..\InputTestFiles\emptyfile.txt");
            HttpStatusClass testedStatusClass = HttpStatusClass.Successful;
            Analyzer analyzer = new Analyzer();
            uint expectedNumberOfClassStatusCodes = 0;

            //Act
            uint resultNumberOfClassStatusCodes =
                analyzer.GetNumberOfClassStatusCodes(testedFilePath, testedStatusClass);

            //Assert
            Assert.AreEqual(expectedNumberOfClassStatusCodes, resultNumberOfClassStatusCodes);
        }

        [TestMethod]
        public void Test_GetNumberOfClassStatusCodes_ForServerErrorClass()
        {
            //Arrange
            string testedFilePath = TestFiles.CreateTempFile(@"..\..\InputTestFiles\HundredLogFile.txt");
            HttpStatusClass testedStatusClass = HttpStatusClass.ClientError;
            Analyzer analyzer = new Analyzer();
            uint expectedNumberOfClassStatusCodes = 31;

            //Act
            uint resultNumberOfClassStatusCodes =
                analyzer.GetNumberOfClassStatusCodes(testedFilePath, testedStatusClass);

            //Assert
            Assert.AreEqual(expectedNumberOfClassStatusCodes, resultNumberOfClassStatusCodes);
        }
    }
}
