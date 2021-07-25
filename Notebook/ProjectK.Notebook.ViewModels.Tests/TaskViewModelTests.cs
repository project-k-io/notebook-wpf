using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectK.Notebook.Models;
using System;

// using ProjectK.NotebookModel.Models.Versions.Version2;

namespace ProjectK.Notebook.ViewModels.Tests
{
    [TestClass]
    public class TaskViewModelTests
    {
        [TestMethod]
        public void ToStringTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void SaveToTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TaskViewModelTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TaskViewModelTest1()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TrySetIdTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void LoadFromTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void AddTest()
        {
            // Arrange
            var parent = new NodeViewModel { Model = new TaskModel { Id = Guid.NewGuid() } };
            var child = new NodeViewModel { Model = new TaskModel { Id = Guid.NewGuid() } };

            // Act
            parent.Add(child);

            // Assert
            Assert.AreEqual(child.Parent.Id, parent.Id, "TaskModel has wrong parent");
            Assert.IsTrue(parent.Nodes.Contains(child), "TaskModel has wrong child");
        }

        [TestMethod]
        public void SetParentsTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void FixTimeTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void ExtractContextTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void FixContextTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void FixTitlesTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void FixTypesTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void FindTaskTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void KeyboardActionTest()
        {
            Assert.Fail();
        }
    }
}