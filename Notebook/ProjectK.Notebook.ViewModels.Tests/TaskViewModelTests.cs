using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectK.Notebook.Models;

// using ProjectK.NotebookModel.Models.Versions.Version2;

namespace ProjectK.Notebook.ViewModels.Tests
{
    [TestClass]
    public class TaskViewModelTests
    {
        [TestMethod]
        public void ToStringTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void SaveToTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void TaskViewModelTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void TaskViewModelTest1()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void TrySetIdTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void LoadFromTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void AddTest()
        {
            // Arrange
            var parent = new NodeViewModel {Model = new TaskModel {Id = Guid.NewGuid()}};
            var child = new NodeViewModel {Model = new TaskModel {Id = Guid.NewGuid()}};

            // Act
            parent.Add(child);

            // Assert
            //AK double actual = account.Balance;
            //AK Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");
            Assert.AreEqual(child.Parent.Id, parent.Id, "TaskModel has wrong parent");
            Assert.IsTrue(parent.Nodes.Contains(child), "TaskModel has wrong child");
        }

        [TestMethod]
        public void SetParentsTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void FixTimeTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void ExtractContextTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void FixContextTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void FixTitlesTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void FixTypesTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void FindTaskTest()
        {
            //AK Assert.Fail();
        }

        [TestMethod]
        public void KeyboardActionTest()
        {
            //AK Assert.Fail();
        }
    }
}