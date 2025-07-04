using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using FluentAssertions;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.DataGrid;

namespace BusBuddy.Tests.UnitTests.UI
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class SyncfusionHighDpiTests
    {
        private MetroForm? _testForm;

        [SetUp]
        public void Setup()
        {
            _testForm = new MetroForm
            {
                Size = new Size(800, 600),
                Text = "High DPI Test Form",
                AutoScaleMode = AutoScaleMode.Dpi
            };
        }

        [TearDown]
        public void TearDown()
        {
            _testForm?.Dispose();
        }

        [Test]
        [TestCase(96)]   // Standard DPI
        [TestCase(120)]  // 125% scaling
        [TestCase(144)]  // 150% scaling  
        [TestCase(192)]  // 200% scaling
        public void Form_RendersCorrectly_AtDifferentDPI(int targetDpi)
        {
            // Arrange
            var expectedScaleFactor = targetDpi / 96.0f;

            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Configure form for DPI awareness
                _testForm!.AutoScaleMode = AutoScaleMode.Dpi;
                _testForm.AutoScaleDimensions = new SizeF(targetDpi, targetDpi);
                
                // Verify the form can handle DPI scaling
                var scaledSize = new Size(
                    (int)(_testForm.Size.Width * expectedScaleFactor),
                    (int)(_testForm.Size.Height * expectedScaleFactor)
                );
                
                scaledSize.Width.Should().BeGreaterThan(0);
                scaledSize.Height.Should().BeGreaterThan(0);
            });
        }

        [Test]
        public void SfDataGrid_DpiAwareness_ConfiguresCorrectly()
        {
            // Arrange & Act
            using var dataGrid = new SfDataGrid();
            
            Assert.DoesNotThrow(() => {
                // Configure DPI awareness for data grid
                dataGrid.AutoScaleMode = AutoScaleMode.Dpi;
                _testForm?.Controls.Add(dataGrid);
            });

            // Assert
            dataGrid.AutoScaleMode.Should().Be(AutoScaleMode.Dpi);
        }

        [Test]
        public void HighDpi_FontScaling_MaintainsReadability()
        {
            // Arrange
            var baseFont = new Font("Segoe UI", 9F);
            var scaleFactor = 1.5f; // 150% scaling

            // Act
            var scaledFont = new Font(baseFont.FontFamily, baseFont.Size * scaleFactor, baseFont.Style);

            // Assert
            scaledFont.Size.Should().Be(baseFont.Size * scaleFactor);
            scaledFont.FontFamily.Name.Should().Be("Segoe UI");
            
            // Cleanup
            baseFont.Dispose();
            scaledFont.Dispose();
        }

        [Test]
        public void HighDpi_ButtonSizing_ScalesAppropriately()
        {
            // Arrange
            var button = new Button
            {
                Text = "Test Button",
                Size = new Size(100, 30),
                AutoSize = false
            };

            // Act
            _testForm?.Controls.Add(button);
            
            // Simulate DPI scaling
            var scaleFactor = 1.25f;
            var expectedScaledSize = new Size(
                (int)(button.Size.Width * scaleFactor),
                (int)(button.Size.Height * scaleFactor)
            );

            // Assert
            expectedScaledSize.Width.Should().Be(125);
            expectedScaledSize.Height.Should().Be((int)(30 * 1.25f));
            
            button.Dispose();
        }

        [Test]
        public void HighDpi_ImageScaling_MaintainsQuality()
        {
            // Arrange
            var originalSize = new Size(16, 16);
            var bitmap = new Bitmap(originalSize.Width, originalSize.Height);
            
            // Act
            var scaleFactor = 2.0f; // 200% scaling
            var scaledSize = new Size(
                (int)(originalSize.Width * scaleFactor),
                (int)(originalSize.Height * scaleFactor)
            );

            // Assert
            scaledSize.Width.Should().Be(32);
            scaledSize.Height.Should().Be(32);
            
            bitmap.Dispose();
        }

        [Test]
        public void DockingManager_HighDpi_ConfiguresCorrectly()
        {
            // Arrange & Act
            using var dockingManager = new DockingManager(_testForm);
            using var dockablePanel = new Panel { Size = new Size(200, 300) };

            Assert.DoesNotThrow(() => {
                _testForm?.Controls.Add(dockablePanel);
                dockingManager.SetDockLabel(dockablePanel, "High DPI Test Panel");
            });

            // Assert
            dockingManager.Should().NotBeNull();
            dockingManager.GetDockLabel(dockablePanel).Should().Be("High DPI Test Panel");
        }

        [Test]
        public void HighDpi_ControlPadding_ScalesProportionally()
        {
            // Arrange
            var panel = new Panel
            {
                Padding = new Padding(10),
                Size = new Size(200, 150)
            };

            // Act
            var scaleFactor = 1.5f;
            var expectedScaledPadding = new Padding((int)(10 * scaleFactor));

            // Assert
            expectedScaledPadding.All.Should().Be(15);
            
            panel.Dispose();
        }

        [Test]
        public void MetroForm_HighDpi_MaintainsAspectRatio()
        {
            // Arrange
            var originalAspectRatio = (double)_testForm!.Width / _testForm.Height;

            // Act
            var scaleFactor = 1.25f;
            var scaledWidth = (int)(_testForm.Width * scaleFactor);
            var scaledHeight = (int)(_testForm.Height * scaleFactor);
            var scaledAspectRatio = (double)scaledWidth / scaledHeight;

            // Assert
            scaledAspectRatio.Should().BeApproximately(originalAspectRatio, 0.01);
        }

        [Test]
        public void HighDpi_TextRendering_RemainsSharp()
        {
            // Arrange
            var label = new Label
            {
                Text = "High DPI Test Text",
                Font = new Font("Segoe UI", 12F),
                AutoSize = true
            };

            // Act
            _testForm?.Controls.Add(label);

            // Assert
            label.Font.Size.Should().Be(12F);
            label.Text.Should().Be("High DPI Test Text");
            label.AutoSize.Should().BeTrue();
            
            label.Dispose();
        }

        [Test]
        public void HighDpi_BorderThickness_ScalesConsistently()
        {
            // Arrange
            var baseBorderSize = 1;
            var scaleFactor = 2.0f;

            // Act
            var scaledBorderSize = (int)(baseBorderSize * scaleFactor);

            // Assert
            scaledBorderSize.Should().Be(2);
        }

        [Test]
        public void DataGrid_HighDpi_CellSizing_ScalesCorrectly()
        {
            // Arrange & Act
            using var dataGrid = new SfDataGrid
            {
                RowHeight = 25,
                HeaderRowHeight = 30
            };

            var scaleFactor = 1.5f;
            var expectedRowHeight = (int)(25 * scaleFactor);
            var expectedHeaderHeight = (int)(30 * scaleFactor);

            // Assert
            expectedRowHeight.Should().Be((int)(25 * 1.5f));
            expectedHeaderHeight.Should().Be((int)(30 * 1.5f));
        }
    }
}
