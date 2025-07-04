using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using FluentAssertions;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.Windows.Forms.Tools;

namespace BusBuddy.Tests.UnitTests.UI
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class SyncfusionPanelRenderingTests
    {
        private MetroForm? _testForm;

        [SetUp]
        public void Setup()
        {
            _testForm = new MetroForm
            {
                Size = new Size(800, 600),
                Text = "Panel Rendering Test Form"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _testForm?.Dispose();
        }

        [Test]
        public void Panels_RenderAsDesigned()
        {
            // Arrange
            using var mainPanel = new GradientPanel
            {
                Name = "MainPanel",
                Size = new Size(400, 300),
                BackgroundColor = new BrushInfo(Color.White)
            };

            // Act
            _testForm?.Controls.Add(mainPanel);

            // Assert
            var foundPanel = _testForm?.Controls.Find("MainPanel", false).FirstOrDefault();
            foundPanel.Should().NotBeNull();
            foundPanel?.Size.Width.Should().Be(400);
            foundPanel?.Size.Height.Should().Be(300);
        }

        [Test]
        public void GradientPanel_BackgroundConfiguration_RendersCorrectly()
        {
            // Arrange & Act
            using var gradientPanel = new GradientPanel
            {
                BackgroundColor = new BrushInfo(Color.LightBlue),
                Size = new Size(200, 150)
            };

            // Assert
            gradientPanel.Should().NotBeNull();
            gradientPanel.Size.Should().Be(new Size(200, 150));
            gradientPanel.BackgroundColor.Should().NotBeNull();
        }

        [Test]
        public void DataGrid_Panel_IntegratesWithParentForm()
        {
            // Arrange & Act
            using var dataGridPanel = new Panel { Size = new Size(600, 400) };
            using var dataGrid = new SfDataGrid 
            { 
                Dock = DockStyle.Fill,
                AllowEditing = false
            };
            
            dataGridPanel.Controls.Add(dataGrid);
            _testForm?.Controls.Add(dataGridPanel);

            // Assert
            dataGrid.Parent.Should().Be(dataGridPanel);
            dataGridPanel.Controls.Should().Contain(dataGrid);
            dataGrid.Dock.Should().Be(DockStyle.Fill);
        }

        [Test]
        public void DockingManager_Panel_ConfiguresCorrectly()
        {
            // Arrange & Act
            using var dockingManager = new DockingManager(_testForm);
            using var dockablePanel = new Panel 
            { 
                Name = "DockablePanel",
                Size = new Size(200, 300)
            };

            _testForm?.Controls.Add(dockablePanel);
            dockingManager.SetDockLabel(dockablePanel, "Test Panel");

            // Assert
            dockingManager.Should().NotBeNull();
            dockingManager.GetDockLabel(dockablePanel).Should().Be("Test Panel");
        }

        [Test]
        public void Panel_ZOrder_MaintainsCorrectLayering()
        {
            // Arrange
            using var backgroundPanel = new Panel 
            { 
                Name = "Background",
                BackColor = Color.Gray,
                Size = new Size(400, 300)
            };
            
            using var foregroundPanel = new Panel 
            { 
                Name = "Foreground", 
                BackColor = Color.White,
                Size = new Size(200, 150),
                Location = new Point(50, 50)
            };

            // Act
            _testForm?.Controls.Add(backgroundPanel);
            _testForm?.Controls.Add(foregroundPanel);
            foregroundPanel.BringToFront();

            // Assert
            var backgroundIndex = _testForm?.Controls.GetChildIndex(backgroundPanel);
            var foregroundIndex = _testForm?.Controls.GetChildIndex(foregroundPanel);
            
            // Lower index means higher Z-order (in front)
            foregroundIndex.Should().BeLessThan(backgroundIndex);
        }

        [Test]
        public void Panel_Transparency_ConfiguresCorrectly()
        {
            // Arrange & Act
            using var transparentPanel = new Panel();
            
            // Note: Direct transparency testing is complex in unit tests
            // We verify the panel can be configured for transparency
            Assert.DoesNotThrow(() => {
                transparentPanel.BackColor = Color.Transparent;
            });

            // Assert
            transparentPanel.BackColor.Should().Be(Color.Transparent);
        }

        [Test]
        public void Panel_Anchoring_BehavesCorrectly()
        {
            // Arrange
            using var anchoredPanel = new Panel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Size = new Size(400, 100)
            };

            // Act
            _testForm?.Controls.Add(anchoredPanel);
            var originalWidth = anchoredPanel.Width;
            
            // Simulate form resize
            _testForm!.Width += 100;

            // Assert
            anchoredPanel.Anchor.Should().HaveFlag(AnchorStyles.Right);
            anchoredPanel.Anchor.Should().HaveFlag(AnchorStyles.Left);
        }

        [Test]
        public void Panel_Scrolling_ConfiguresWhenContentOverflows()
        {
            // Arrange
            using var scrollablePanel = new Panel
            {
                AutoScroll = true,
                Size = new Size(200, 200)
            };

            using var largeContent = new Panel
            {
                Size = new Size(400, 400),
                BackColor = Color.LightGray
            };

            // Act
            scrollablePanel.Controls.Add(largeContent);
            _testForm?.Controls.Add(scrollablePanel);

            // Assert
            scrollablePanel.AutoScroll.Should().BeTrue();
            scrollablePanel.Controls.Should().Contain(largeContent);
        }

        [Test]
        public void Panel_BorderStyles_RenderProperly()
        {
            // Arrange & Act
            using var borderedPanel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(150, 100)
            };

            // Assert
            borderedPanel.BorderStyle.Should().Be(BorderStyle.FixedSingle);
        }

        [Test]
        public void Panel_NestedLayout_MaintainsHierarchy()
        {
            // Arrange
            using var parentPanel = new Panel { Name = "Parent", Size = new Size(400, 300) };
            using var childPanel = new Panel { Name = "Child", Size = new Size(200, 150) };
            using var grandchildPanel = new Panel { Name = "Grandchild", Size = new Size(100, 75) };

            // Act
            parentPanel.Controls.Add(childPanel);
            childPanel.Controls.Add(grandchildPanel);
            _testForm?.Controls.Add(parentPanel);

            // Assert
            parentPanel.Controls.Should().Contain(childPanel);
            childPanel.Controls.Should().Contain(grandchildPanel);
            grandchildPanel.Parent.Should().Be(childPanel);
            childPanel.Parent.Should().Be(parentPanel);
        }
    }
}
