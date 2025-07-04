using NUnit.Framework;
using FluentAssertions;
using Bus_Buddy.Utilities;
using Syncfusion.Windows.Forms.Tools;
using System.Windows.Forms;
using System.Drawing;

namespace BusBuddy.Tests.UnitTests.Utilities
{
    /// <summary>
    /// Tests for SyncfusionAdvancedManager - Advanced Syncfusion component management
    /// This covers complex Syncfusion component configuration and styling
    /// </summary>
    [TestFixture]
    public class SyncfusionAdvancedManagerTests
    {
        private Form _testForm = null!;

        [SetUp]
        public void SetUp()
        {
            _testForm = new Form();
        }

        [Test]
        public void CreateOfficeStyleForm_ShouldReturnForm()
        {
            // Act
            var form = SyncfusionAdvancedManager.CreateOfficeStyleForm("Test Form");

            // Assert
            form.Should().NotBeNull();
            form.Text.Should().Be("Test Form");
            form.Dispose();
        }

        [Test]
        public void CreateOfficeStyleForm_WithNullTitle_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => SyncfusionAdvancedManager.CreateOfficeStyleForm(null!));
        }

        [Test]
        public void ConfigureDataGrid_WithValidGrid_ShouldNotThrow()
        {
            // Arrange
            var dataGrid = new Syncfusion.WinForms.DataGrid.SfDataGrid();

            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionAdvancedManager.ConfigureDataGrid(dataGrid));
        }

        [Test]
        public void ConfigureDataGrid_WithNullGrid_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => SyncfusionAdvancedManager.ConfigureDataGrid(null!));
        }

        [Test]
        public void CreateStyledButton_ShouldReturnButton()
        {
            // Act
            var button = SyncfusionAdvancedManager.CreateStyledButton("Test Button", Color.Blue);

            // Assert
            button.Should().NotBeNull();
            button.Text.Should().Be("Test Button");
            button.Dispose();
        }

        [Test]
        public void ApplyModernTheme_WithValidForm_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionAdvancedManager.ApplyModernTheme(_testForm));
        }

        [Test]
        public void SetupAdvancedLayout_WithValidForm_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => SyncfusionAdvancedManager.SetupAdvancedLayout(_testForm));
        }

        [Test]
        public void GetThemeColors_ShouldReturnColorDictionary()
        {
            // Act
            var colors = SyncfusionAdvancedManager.GetThemeColors();

            // Assert
            colors.Should().NotBeNull();
            colors.Should().NotBeEmpty();
        }

        [TearDown]
        public void TearDown()
        {
            _testForm?.Dispose();
        }
    }
}
