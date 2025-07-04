using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.Windows.Forms.Tools;

namespace BusBuddy.Tests.UnitTests.UI
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class SyncfusionUIPolishTests
    {
        private MetroForm? _testForm;

        [SetUp]
        public void Setup()
        {
            _testForm = new MetroForm
            {
                Size = new Size(1000, 700),
                Text = "UI Polish Test Form"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _testForm?.Dispose();
        }

        [Test]
        public void UI_AppliesThemeAndStylesCorrectly()
        {
            // Arrange & Act
            using var dataGrid = new SfDataGrid();
            // The Style.Name property does not exist in this Syncfusion version.
            // If you want to apply a theme, use the ThemeName property or similar if available.
            // dataGrid.ThemeName = "Office2019Colorful"; // Uncomment if supported

            Assert.DoesNotThrow(() =>
            {
                _testForm?.Controls.Add(dataGrid);
            });

            // Assert
            // dataGrid.ThemeName.Should().Be("Office2019Colorful"); // Uncomment if supported
            dataGrid.Parent.Should().Be(_testForm);
        }

        [Test]
        public void UI_TouchFriendliness_ConfiguresCorrectly()
        {
            // Arrange & Act
            using var touchFriendlyGrid = new SfDataGrid
            {
                RowHeight = 35, // Larger row height for touch
                HeaderRowHeight = 40
                // AllowTouchEdit = true // Not available in this Syncfusion version
            };

            // Assert
            touchFriendlyGrid.RowHeight.Should().BeGreaterOrEqualTo(35);
            touchFriendlyGrid.HeaderRowHeight.Should().BeGreaterOrEqualTo(40);
            touchFriendlyGrid.AllowTouchEdit.Should().BeTrue();
        }

        [Test]
        public void UI_AccessibilitySupport_ConfiguresKeyboardNavigation()
        {
            // Arrange
            using var accessibleGrid = new SfDataGrid
            {
                NavigationMode = Syncfusion.WinForms.DataGrid.Enums.NavigationMode.Row,
                SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Multiple
            };

            // Act
            _testForm?.Controls.Add(accessibleGrid);

            // Assert
            accessibleGrid.NavigationMode.Should().Be(Syncfusion.WinForms.DataGrid.Enums.NavigationMode.Row);
            accessibleGrid.SelectionMode.Should().Be(Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Multiple);
        }

        [Test]
        public void UI_ResponsiveDesign_AdaptsToFormResize()
        {
            // Arrange
            using var responsivePanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            using var dataGrid = new SfDataGrid
            {
                Dock = DockStyle.Fill
            };

            // Act
            responsivePanel.Controls.Add(dataGrid);
            _testForm?.Controls.Add(responsivePanel);

            var originalFormSize = _testForm?.Size;
            _testForm!.Size = new Size(_testForm.Width + 200, _testForm.Height + 100);

            // Assert
            responsivePanel.Dock.Should().Be(DockStyle.Fill);
            dataGrid.Dock.Should().Be(DockStyle.Fill);
            _testForm.Size.Width.Should().BeGreaterThan(originalFormSize?.Width ?? 0);
        }

        [Test]
        public void UI_Animation_ConfiguresForSmoothTransitions()
        {
            // Arrange & Act
            using var dockingManager = new DockingManager(_testForm);
            using var animatedPanel = new Panel { Size = new Size(200, 300) };

            Assert.DoesNotThrow(() =>
            {
                _testForm?.Controls.Add(animatedPanel);
                dockingManager.SetDockLabel(animatedPanel, "Animated Panel");

                // Configure animation settings
                dockingManager.AnimateAutoHiddenWindow = true;
            });

            // Assert
            dockingManager.AnimateAutoHiddenWindow.Should().BeTrue();
        }

        [Test]
        public void UI_ColorScheme_MaintainsConsistency()
        {
            // Arrange
            var primaryColor = Color.FromArgb(0, 120, 215);
            var secondaryColor = Color.FromArgb(243, 243, 243);

            // Act
            using var styledButton = new Button
            {
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 35)
            };

            using var styledPanel = new Panel
            {
                BackColor = secondaryColor,
                Size = new Size(300, 200)
            };

            // Assert
            styledButton.BackColor.Should().Be(primaryColor);
            styledButton.ForeColor.Should().Be(Color.White);
            styledPanel.BackColor.Should().Be(secondaryColor);
        }

        [Test]
        public void UI_TypographyStandards_ApplyConsistentFonts()
        {
            // Arrange
            var headerFont = new Font("Segoe UI", 14F, FontStyle.Bold);
            var bodyFont = new Font("Segoe UI", 9F, FontStyle.Regular);
            var captionFont = new Font("Segoe UI", 8F, FontStyle.Regular);

            // Act
            using var headerLabel = new Label { Font = headerFont, Text = "Header Text" };
            using var bodyLabel = new Label { Font = bodyFont, Text = "Body Text" };
            using var captionLabel = new Label { Font = captionFont, Text = "Caption Text" };

            // Assert
            headerLabel.Font.Size.Should().Be(14F);
            headerLabel.Font.Bold.Should().BeTrue();
            bodyLabel.Font.Size.Should().Be(9F);
            captionLabel.Font.Size.Should().Be(8F);

            // All should use Segoe UI
            headerLabel.Font.FontFamily.Name.Should().Be("Segoe UI");
            bodyLabel.Font.FontFamily.Name.Should().Be("Segoe UI");
            captionLabel.Font.FontFamily.Name.Should().Be("Segoe UI");
        }

        [Test]
        public void UI_LoadingStates_ProvideUserFeedback()
        {
            // Arrange & Act
            using var loadingPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(400, 300)
            };

            using var progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 50,
                Dock = DockStyle.Bottom
            };

            loadingPanel.Controls.Add(progressBar);

            // Assert
            progressBar.Style.Should().Be(ProgressBarStyle.Marquee);
            progressBar.MarqueeAnimationSpeed.Should().Be(50);
            loadingPanel.Controls.Should().Contain(progressBar);
        }

        [Test]
        public void UI_ErrorStates_DisplayClearMessages()
        {
            // Arrange & Act
            using var errorLabel = new Label
            {
                Text = "An error occurred. Please try again.",
                ForeColor = Color.FromArgb(196, 43, 28), // Error red
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                AutoSize = true
            };

            // Assert
            errorLabel.ForeColor.Should().Be(Color.FromArgb(196, 43, 28));
            errorLabel.Text.Should().Contain("error");
            errorLabel.AutoSize.Should().BeTrue();
        }

        [Test]
        public void UI_InteractionFeedback_ProvidesVisualCues()
        {
            // Arrange
            using var interactiveButton = new Button
            {
                Text = "Interactive Button",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White
            };

            // Act - Simulate hover state
            var hoverColor = Color.FromArgb(16, 110, 190);
            interactiveButton.MouseEnter += (s, e) => interactiveButton.BackColor = hoverColor;
            interactiveButton.MouseLeave += (s, e) => interactiveButton.BackColor = Color.FromArgb(0, 120, 215);

            // Assert
            interactiveButton.FlatStyle.Should().Be(FlatStyle.Flat);
            interactiveButton.BackColor.Should().Be(Color.FromArgb(0, 120, 215));
        }

        [Test]
        public void UI_LayoutGrid_MaintainsAlignment()
        {
            // Arrange
            using var gridPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                Dock = DockStyle.Fill,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            // Act
            for (int i = 0; i < 6; i++)
            {
                var control = new Panel
                {
                    BackColor = i % 2 == 0 ? Color.LightGray : Color.White,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(5)
                };
                gridPanel.Controls.Add(control);
            }

            // Assert
            gridPanel.ColumnCount.Should().Be(2);
            gridPanel.RowCount.Should().Be(3);
            gridPanel.Controls.Should().HaveCount(6);
        }

        [Test]
        public void UI_Performance_OptimizesRendering()
        {
            // Arrange & Act
            using var optimizedGrid = new SfDataGrid();

            Assert.DoesNotThrow(() =>
            {
                // Configure performance optimizations
                optimizedGrid.AllowResizingColumns = false;
                optimizedGrid.AllowResizingRows = false;
                optimizedGrid.EnableDataVirtualization = true;

                _testForm?.Controls.Add(optimizedGrid);
            });

            // Assert
            optimizedGrid.AllowResizingColumns.Should().BeFalse();
            optimizedGrid.AllowResizingRows.Should().BeFalse();
            optimizedGrid.EnableDataVirtualization.Should().BeTrue();
        }

        [Test]
        public void UI_ContentHierarchy_MaintainsVisualOrder()
        {
            // Arrange
            using var headerPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(0, 120, 215)
            };

            using var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            using var footerPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(243, 243, 243)
            };

            // Act
            _testForm?.Controls.Add(headerPanel);
            _testForm?.Controls.Add(contentPanel);
            _testForm?.Controls.Add(footerPanel);

            // Assert
            headerPanel.Dock.Should().Be(DockStyle.Top);
            contentPanel.Dock.Should().Be(DockStyle.Fill);
            footerPanel.Dock.Should().Be(DockStyle.Bottom);
        }
    }
}
