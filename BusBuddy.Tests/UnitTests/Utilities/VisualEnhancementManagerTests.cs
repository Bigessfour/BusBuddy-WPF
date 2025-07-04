using NUnit.Framework;
using FluentAssertions;
using Bus_Buddy.Utilities;
using System.Windows.Forms;
using System.Drawing;

namespace BusBuddy.Tests.UnitTests.Utilities
{    /// <summary>
     /// Tests for VisualEnhancementManager - UI visual enhancement utilities
     /// This covers complex visual styling and animation management
     /// </summary>
    [TestFixture]
    public class VisualEnhancementManagerTests
    {
        private Form _testForm = null!;
        private Button _testButton = null!;
        private Panel _testPanel = null!;

        [SetUp]
        public void SetUp()
        {
            _testForm = new Form();
            _testButton = new Button();
            _testPanel = new Panel();

            _testForm.Controls.Add(_testButton);
            _testForm.Controls.Add(_testPanel);
        }

        [Test]
        public void ApplyEnhancedTheme_WithValidForm_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => VisualEnhancementManager.ApplyEnhancedTheme(_testForm));
        }

        [Test]
        public void ApplyEnhancedTheme_WithNullForm_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => VisualEnhancementManager.ApplyEnhancedTheme(null!));
        }

        [Test]
        public void ApplyEnhancedGridVisuals_WithValidDataGrid_ShouldNotThrow()
        {
            // Arrange
            var dataGrid = new Syncfusion.WinForms.DataGrid.SfDataGrid();

            // Act & Assert
            Assert.DoesNotThrow(() => VisualEnhancementManager.ApplyEnhancedGridVisuals(dataGrid));
        }

        [Test]
        public void ApplyChartEnhancements_WithValidControl_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => VisualEnhancementManager.ApplyChartEnhancements(_testPanel));
        }

        [Test]
        public void EnableHighQualityFontRendering_WithValidControl_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => VisualEnhancementManager.EnableHighQualityFontRendering(_testForm));
        }

        [Test]
        public void ApplyEnhancedButtonStyling_WithValidButton_ShouldNotThrow()
        {
            // Arrange
            var sfButton = new Syncfusion.WinForms.Controls.SfButton();
            var backgroundColor = Color.Blue;

            // Act & Assert
            Assert.DoesNotThrow(() => VisualEnhancementManager.ApplyEnhancedButtonStyling(sfButton, backgroundColor));
        }

        [Test]
        public void GetVisualEnhancementDiagnostics_WithValidForm_ShouldReturnDiagnostics()
        {
            // Act
            var diagnostics = VisualEnhancementManager.GetVisualEnhancementDiagnostics(_testForm);

            // Assert
            diagnostics.Should().NotBeNull();
            diagnostics.Should().Contain("Visual Enhancement Diagnostics");
            diagnostics.Should().Contain("Form:");
        }

        [Test]
        public void Constants_ShouldHaveValidValues()
        {
            // Assert
            VisualEnhancementManager.OPTIMAL_FONT_FAMILY.Should().NotBeNullOrEmpty();
            VisualEnhancementManager.OPTIMAL_HEADER_FONT_SIZE.Should().BeGreaterThan(0);
            VisualEnhancementManager.OPTIMAL_CELL_FONT_SIZE.Should().BeGreaterThan(0);
            VisualEnhancementManager.ENHANCED_GRID_LINE_WIDTH.Should().BeGreaterThan(0);
        }

        [TearDown]
        public void TearDown()
        {
            _testForm?.Dispose();
            _testButton?.Dispose();
            _testPanel?.Dispose();
        }
    }


}
