using System;
using HW5.LogManipulators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HW5.Tests
{
    [TestClass]
    public class ValidatorTest
    {
        [TestMethod]
        public void Test_ValidateRandomLogs_FourWrongLogLines()
        {
            //Arrange
            string testedFilePath = TestFiles.CreateTempFile(@"..\..\InputTestFiles\HundredLogFileWrongFormat.txt");
            string expectedFilePath = @"..\..\ExpectedTestFiles\HundredLogFileWrongFormat.txt";
            Validator validator = new Validator();
            string configuration = "%h %l %u %t %r %s %b";

            //Act
            validator.ValidateRandomLogs(testedFilePath, configuration);

            //Assert
            bool areFilesEqual = TestFiles.AreFilesEqual(expectedFilePath, testedFilePath, out string message);
            Assert.IsTrue(areFilesEqual, $"Expected file and current result file are not the same. {message}");
        }

        [TestMethod]
        public void Test_ValidateRandomLogs_CorrectFile()
        {
            //Arrange
            string testedFilePath = TestFiles.CreateTempFile(@"..\..\InputTestFiles\TenLogFile.txt");
            string expectedFilePath = @"..\..\ExpectedTestFiles\TenLogFile.txt";
            Validator validator = new Validator();
            string configuration = "%t %b %h %l %u %r %s";

            //Act
            validator.ValidateRandomLogs(testedFilePath, configuration);

            //Assert
            bool areFilesEqual = TestFiles.AreFilesEqual(expectedFilePath, testedFilePath, out string message);
            Assert.IsTrue(areFilesEqual, $"Expected file and current result file are not the same. {message}");
        }
    }
}
