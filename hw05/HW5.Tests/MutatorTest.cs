using System;
using HW5.Enums;
using HW5.LogManipulators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HW5.Tests
{
    [TestClass]
    public class MutatorTest
    {

        [TestMethod]
        public void Test_HideIpAddressByLocalhost_ForHundredLines()
        {            
            //Arrange
            string testedFilePath = TestFiles.CreateTempFile(@"..\..\InputTestFiles\HundredLogFileIpAddress.txt");
            string expectedFilePath = @"..\..\ExpectedTestFiles\HundredLogFileIpAddress.txt";
            Mutator mutator = new Mutator();

            //Act
            mutator.HideIpAddressByLocalhost(testedFilePath);

            //Assert
            bool areFilesEqual = TestFiles.AreFilesEqual(expectedFilePath, testedFilePath, out string message);
            Assert.IsTrue(areFilesEqual, $"Expected file and current result file are not the same. {message}");
        }
    }
}
