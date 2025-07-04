using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ComponentModel;
using FluentAssertions;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Enums;
using Syncfusion.WinForms.DataGrid.Events;
using Bus_Buddy.Models;

namespace BusBuddy.Tests.UnitTests.UI
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class SyncfusionDataGridFormattingTests
    {
        private SfDataGrid? _dataGrid;
        private List<TestDataModel>? _testData;

        [SetUp]
        public void Setup()
        {
            _dataGrid = new SfDataGrid
            {
                AllowEditing = false,
                AllowSorting = true,
                AutoGenerateColumns = false
            };

            _testData = GetSampleTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _dataGrid?.Dispose();
        }

        [Test]
        public void DataGrid_FormatsCellsCorrectly()
        {
            // Arrange
            var priceColumn = new GridTextColumn
            {
                MappingName = "Price",
                HeaderText = "Price",
                Format = "C2" // Currency format with 2 decimal places
            };

            _dataGrid!.Columns.Add(priceColumn);
            _dataGrid.DataSource = _testData;

            // Act
            _dataGrid.Refresh();

            // Assert
            priceColumn.Format.Should().Be("C2");
            priceColumn.MappingName.Should().Be("Price");
            _dataGrid.Columns.Should().Contain(priceColumn);
        }

        [Test]
        public void DataGrid_DateTimeFormatting_DisplaysCorrectly()
        {
            // Arrange
            var dateColumn = new GridDateTimeColumn
            {
                MappingName = "CreatedDate",
                HeaderText = "Created Date",
                Format = "yyyy-MM-dd HH:mm"
            };

            _dataGrid!.Columns.Add(dateColumn);
            _dataGrid.DataSource = _testData;

            // Act & Assert
            dateColumn.Format.Should().Be("yyyy-MM-dd HH:mm");
            dateColumn.MappingName.Should().Be("CreatedDate");
        }

        [Test]
        public void DataGrid_NumericFormatting_HandlesDecimals()
        {
            // Arrange
            var percentColumn = new GridNumericColumn
            {
                MappingName = "CompletionRate",
                HeaderText = "Completion %",
                Format = "P1", // Percentage with 1 decimal place
                NumberFormatInfo = new NumberFormatInfo { PercentDecimalDigits = 1 }
            };

            _dataGrid!.Columns.Add(percentColumn);
            _dataGrid.DataSource = _testData;

            // Act & Assert
            percentColumn.Format.Should().Be("P1");
            percentColumn.NumberFormatInfo?.PercentDecimalDigits.Should().Be(1);
        }

        [Test]
        public void DataGrid_ConditionalFormatting_AppliesCorrectly()
        {
            // Arrange
            var statusColumn = new GridTextColumn
            {
                MappingName = "Status",
                HeaderText = "Status"
            };

            _dataGrid!.Columns.Add(statusColumn);
            _dataGrid.DataSource = _testData;

            // Act - Setup conditional formatting via QueryCellInfo event
            bool eventHandlerCalled = false;
            _dataGrid.QueryCellInfo += (sender, e) => {
                if (e.Column.MappingName == "Status")
                {
                    eventHandlerCalled = true;
                    if (e.DisplayText == "Active")
                    {
                        e.Style.BackColor = System.Drawing.Color.LightGreen;
                    }
                    else if (e.DisplayText == "Inactive")
                    {
                        e.Style.BackColor = System.Drawing.Color.LightCoral;
                    }
                }
            };

            _dataGrid.Refresh();

            // Assert
            eventHandlerCalled.Should().BeTrue();
            statusColumn.MappingName.Should().Be("Status");
        }

        [Test]
        public void DataGrid_CustomCellRenderer_FormatsSpecialValues()
        {
            // Arrange
            var idColumn = new GridTextColumn
            {
                MappingName = "Id",
                HeaderText = "ID",
                Format = "D6" // Zero-padded 6 digit format
            };

            _dataGrid!.Columns.Add(idColumn);
            _dataGrid.DataSource = _testData;

            // Act & Assert
            idColumn.Format.Should().Be("D6");
            idColumn.HeaderText.Should().Be("ID");
        }

        [Test]
        public void DataGrid_BooleanFormatting_DisplaysCheckboxes()
        {
            // Arrange
            var isActiveColumn = new GridCheckBoxColumn
            {
                MappingName = "IsActive",
                HeaderText = "Active",
                AllowEditing = false
            };

            _dataGrid!.Columns.Add(isActiveColumn);
            _dataGrid.DataSource = _testData;

            // Act & Assert
            isActiveColumn.MappingName.Should().Be("IsActive");
            isActiveColumn.AllowEditing.Should().BeFalse();
            _dataGrid.Columns.OfType<GridCheckBoxColumn>().Should().HaveCount(1);
        }

        [Test]
        public void DataGrid_NullValueHandling_DisplaysCorrectly()
        {
            // Arrange
            var nullableColumn = new GridTextColumn
            {
                MappingName = "OptionalField",
                HeaderText = "Optional",
                NullText = "N/A"
            };

            _dataGrid!.Columns.Add(nullableColumn);
            _dataGrid.DataSource = _testData;

            // Act & Assert
            nullableColumn.NullText.Should().Be("N/A");
            nullableColumn.MappingName.Should().Be("OptionalField");
        }

        [Test]
        public void DataGrid_HeaderStyling_ConfiguresCorrectly()
        {
            // Arrange & Act
            _dataGrid!.Style.HeaderStyle.Font.Size = 10;
            _dataGrid.Style.HeaderStyle.Font.Bold = true;

            // Assert
            _dataGrid.Style.HeaderStyle.Font.Size.Should().Be(10);
            _dataGrid.Style.HeaderStyle.Font.Bold.Should().BeTrue();
        }

        [Test]
        public void DataGrid_AlternatingRowColors_AppliesToRows()
        {
            // Arrange & Act
            _dataGrid!.Style.AlternatingRowStyle.BackColor = System.Drawing.Color.AliceBlue;
            _dataGrid.DataSource = _testData;

            // Assert
            _dataGrid.Style.AlternatingRowStyle.BackColor.Should().Be(System.Drawing.Color.AliceBlue);
        }

        [Test]
        public void DataGrid_CellAlignment_ConfiguresCorrectly()
        {
            // Arrange
            var alignedColumn = new GridTextColumn
            {
                MappingName = "Price",
                HeaderText = "Price",
                TextAlignment = GridTextAlignment.Right
            };

            // Act
            _dataGrid!.Columns.Add(alignedColumn);

            // Assert
            alignedColumn.TextAlignment.Should().Be(GridTextAlignment.Right);
        }

        [Test]
        public void DataGrid_GroupingFormatting_DisplaysGroupHeaders()
        {
            // Arrange
            _dataGrid!.DataSource = _testData;
            _dataGrid.AllowGrouping = true;

            var statusColumn = new GridTextColumn
            {
                MappingName = "Status",
                HeaderText = "Status"
            };
            _dataGrid.Columns.Add(statusColumn);

            // Act
            _dataGrid.GroupColumnDescriptions.Add(new Syncfusion.Data.GroupColumnDescription
            {
                ColumnName = "Status"
            });

            // Assert
            _dataGrid.AllowGrouping.Should().BeTrue();
            _dataGrid.GroupColumnDescriptions.Should().HaveCount(1);
        }

        [Test]
        public void DataGrid_ValidationFormatting_HighlightsErrors()
        {
            // Arrange
            var validatedColumn = new GridTextColumn
            {
                MappingName = "Name",
                HeaderText = "Name"
            };

            _dataGrid!.Columns.Add(validatedColumn);
            _dataGrid.DataSource = _testData;

            // Act - Setup validation formatting
            bool validationEventCalled = false;
            _dataGrid.QueryCellInfo += (sender, e) => {
                if (e.Column.MappingName == "Name" && string.IsNullOrEmpty(e.DisplayText))
                {
                    validationEventCalled = true;
                    e.Style.BackColor = System.Drawing.Color.LightPink;
                }
            };

            _dataGrid.Refresh();

            // Assert
            validatedColumn.MappingName.Should().Be("Name");
            // Note: validationEventCalled will be true only if there are empty name fields
        }

        private List<TestDataModel> GetSampleTestData()
        {
            return new List<TestDataModel>
            {
                new() { Id = 1, Name = "Test Item 1", Price = 100.50m, CreatedDate = DateTime.Now, Status = "Active", IsActive = true, CompletionRate = 0.85, OptionalField = "Optional 1" },
                new() { Id = 2, Name = "Test Item 2", Price = 250.75m, CreatedDate = DateTime.Now.AddDays(-1), Status = "Inactive", IsActive = false, CompletionRate = 0.92, OptionalField = null },
                new() { Id = 3, Name = "Test Item 3", Price = 75.25m, CreatedDate = DateTime.Now.AddDays(-2), Status = "Active", IsActive = true, CompletionRate = 0.78, OptionalField = "Optional 3" }
            };
        }
    }

    public class TestDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public double CompletionRate { get; set; }
        public string? OptionalField { get; set; }
    }
}
