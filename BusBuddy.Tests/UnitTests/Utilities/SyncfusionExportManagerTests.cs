using NUnit.Framework;
using FluentAssertions;
using Bus_Buddy.Utilities;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace BusBuddy.Tests.UnitTests.Utilities
{
    /// <summary>
    /// Tests for SyncfusionExportManager - Data export functionality
    /// This covers complex Syncfusion export operations and file management
    /// TEMPORARILY DISABLED: These tests trigger MessageBox popups that freeze the system
    /// </summary>
    [TestFixture]
    [Ignore("Tests trigger MessageBox popups - need to be refactored to avoid UI dialogs")]
    public class SyncfusionExportManagerTests
    {
        private DataTable _testDataTable = null!;
        private string _tempFilePath = string.Empty;

        [SetUp]
        public void SetUp()
        {
            _testDataTable = CreateTestDataTable();
            _tempFilePath = Path.GetTempFileName();
        }

        private static DataTable CreateTestDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Value", typeof(decimal));

            table.Rows.Add(1, "Test Item 1", 100.50m);
            table.Rows.Add(2, "Test Item 2", 200.75m);
            table.Rows.Add(3, "Test Item 3", 300.25m);

            return table;
        }
        [Test]
        public void ExportToCSV_WithValidDataGrid_ShouldCreateFile()
        {
            // Arrange
            var dataGrid = CreateTestDataGrid();

            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionExportManager.ExportToCSV(dataGrid, _tempFilePath));
            Assert.That(File.Exists(_tempFilePath), Is.True);
        }

        private static Syncfusion.WinForms.DataGrid.SfDataGrid CreateTestDataGrid()
        {
            var dataGrid = new Syncfusion.WinForms.DataGrid.SfDataGrid();
            var testData = new List<TestDataItem>
            {
                new TestDataItem { ID = 1, Name = "Test Item 1", Value = 100.50m },
                new TestDataItem { ID = 2, Name = "Test Item 2", Value = 200.75m }
            };
            dataGrid.DataSource = testData;
            return dataGrid;
        }

        public class TestDataItem
        {
            public int ID { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Value { get; set; }
        }

        [Test]
        public void ExportToXML_WithValidDataGrid_ShouldCreateFile()
        {
            // Arrange
            var dataGrid = CreateTestDataGrid();
            var xmlPath = Path.ChangeExtension(_tempFilePath, ".xml");

            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionExportManager.ExportToXML(dataGrid, xmlPath));

            // Cleanup
            if (File.Exists(xmlPath))
                File.Delete(xmlPath);
        }

        [Test]
        public void ExportToJSON_WithValidDataGrid_ShouldCreateFile()
        {
            // Arrange
            var dataGrid = CreateTestDataGrid();
            var jsonPath = Path.ChangeExtension(_tempFilePath, ".json");

            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionExportManager.ExportToJSON(dataGrid, jsonPath));

            // Cleanup
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }

        [Test]
        public void ImportFromCSV_WithValidFile_ShouldReturnDataTable()
        {
            // Arrange
            var csvContent = "ID,Name,Value\n1,Test,100.50\n2,Test2,200.75";
            File.WriteAllText(_tempFilePath, csvContent);

            // Act
            var result = SyncfusionExportManager.ImportFromCSV(_tempFilePath);

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(2);
            result.Columns.Count.Should().Be(3);
        }

        [Test]
        public void GetExportStatistics_WithValidDataGrid_ShouldReturnStatistics()
        {
            // Arrange
            var dataGrid = CreateTestDataGrid();

            // Act
            var statistics = SyncfusionExportManager.GetExportStatistics(dataGrid);

            // Assert
            statistics.Should().NotBeNull();
            statistics.Should().Contain("Columns:");
            statistics.Should().Contain("Rows:");
        }

        [Test]
        public void ExportToExcelCSV_WithValidDataGrid_ShouldNotThrow()
        {
            // Arrange
            var dataGrid = CreateTestDataGrid();
            var excelPath = Path.ChangeExtension(_tempFilePath, ".xlsx");

            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionExportManager.ExportToExcelCSV(dataGrid, excelPath));

            // Cleanup
            var csvPath = Path.ChangeExtension(excelPath, ".csv");
            if (File.Exists(csvPath))
                File.Delete(csvPath);
        }

        [Test]
        public void ExportToCSV_WithEmptyDataGrid_ShouldCreateEmptyFile()
        {
            // Arrange
            var dataGrid = new Syncfusion.WinForms.DataGrid.SfDataGrid();
            dataGrid.DataSource = new List<TestDataItem>(); // Empty list

            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionExportManager.ExportToCSV(dataGrid, _tempFilePath));
            Assert.That(File.Exists(_tempFilePath), Is.True);
        }

        [Test]
        public void ExportToCSV_WithNullDataSource_ShouldNotThrow()
        {
            // Arrange
            var dataGrid = new Syncfusion.WinForms.DataGrid.SfDataGrid();
            dataGrid.DataSource = null;

            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionExportManager.ExportToCSV(dataGrid, _tempFilePath));
        }

        [Test]
        public void ExportToXML_WithComplexData_ShouldCreateValidXML()
        {
            // Arrange
            var dataGrid = CreateTestDataGrid();
            var xmlPath = Path.ChangeExtension(_tempFilePath, ".xml");

            // Act
            Assert.DoesNotThrow(() => SyncfusionExportManager.ExportToXML(dataGrid, xmlPath));

            // Assert
            Assert.That(File.Exists(xmlPath), Is.True);
            var xmlContent = File.ReadAllText(xmlPath);
            xmlContent.Should().Contain("<?xml");

            // Cleanup
            if (File.Exists(xmlPath))
                File.Delete(xmlPath);
        }

        [Test]
        public void ExportToJSON_WithComplexData_ShouldCreateValidJSON()
        {
            // Arrange
            var dataGrid = CreateTestDataGrid();
            var jsonPath = Path.ChangeExtension(_tempFilePath, ".json");

            // Act
            Assert.DoesNotThrow(() => SyncfusionExportManager.ExportToJSON(dataGrid, jsonPath));

            // Assert
            Assert.That(File.Exists(jsonPath), Is.True);
            var jsonContent = File.ReadAllText(jsonPath);
            jsonContent.Should().Contain("[");
            jsonContent.Should().Contain("]");

            // Cleanup
            if (File.Exists(jsonPath))
                File.Delete(jsonPath);
        }

        [Test]
        public void ImportFromCSV_WithInvalidFile_ShouldReturnEmptyTable()
        {
            // Arrange
            var invalidPath = "nonexistent_file.csv";

            // Act
            var result = SyncfusionExportManager.ImportFromCSV(invalidPath);

            // Assert
            result.Should().NotBeNull();
            result.Rows.Count.Should().Be(0);
        }

        [Test]
        public void ImportFromCSV_WithMalformedCSV_ShouldHandleGracefully()
        {
            // Arrange
            var malformedCsv = "ID,Name\n1,\"Unclosed quote\n2,Valid";
            File.WriteAllText(_tempFilePath, malformedCsv);

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var result = SyncfusionExportManager.ImportFromCSV(_tempFilePath);
                result.Should().NotBeNull();
            });
        }

        [Test]
        public void GetExportStatistics_WithNullDataSource_ShouldReturnZeroStats()
        {
            // Arrange
            var dataGrid = new Syncfusion.WinForms.DataGrid.SfDataGrid();
            dataGrid.DataSource = null;

            // Act
            var statistics = SyncfusionExportManager.GetExportStatistics(dataGrid);

            // Assert
            statistics.Should().NotBeNull();
            statistics.Should().Contain("Rows: 0");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_tempFilePath))
                File.Delete(_tempFilePath);

            _testDataTable?.Dispose();
        }
    }
}
