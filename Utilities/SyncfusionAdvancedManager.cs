using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Bus_Buddy.Utilities
{
    /// <summary>
    /// Advanced Syncfusion features manager implementing core documented methods
    /// Based on Syncfusion Essential Studio Version 30.1.37 documentation
    /// Focuses on available functionality without additional assembly dependencies
    /// </summary>
    public static class SyncfusionAdvancedManager
    {
        #region Grouping Operations - DOCUMENTED METHODS

        /// <summary>
        /// Configure programmatic grouping for SfDataGrid
        /// IMPLEMENTS: GroupColumnDescriptions (documented method)
        /// </summary>
        public static void ConfigureGrouping(SfDataGrid dataGrid, params string[] columnNames)
        {
            dataGrid.AllowGrouping = true;
            dataGrid.ShowGroupDropArea = true;
            dataGrid.AutoExpandGroups = true;

            // Clear existing grouping
            dataGrid.GroupColumnDescriptions.Clear();

            // Add grouping for specified columns
            foreach (string columnName in columnNames)
            {
                dataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription()
                {
                    ColumnName = columnName
                });
            }
        }

        #endregion

        #region Sorting Operations - DOCUMENTED METHODS

        /// <summary>
        /// Configure programmatic sorting
        /// IMPLEMENTS: SortColumnDescriptions (documented property)
        /// </summary>
        public static void ConfigureSorting(SfDataGrid dataGrid, string columnName,
            ListSortDirection direction = ListSortDirection.Ascending)
        {
            dataGrid.AllowSorting = true;
            dataGrid.AllowTriStateSorting = true;
            dataGrid.ShowSortNumbers = true;

            // Clear existing sorting
            dataGrid.SortColumnDescriptions.Clear();

            // Add new sort
            dataGrid.SortColumnDescriptions.Add(new SortColumnDescription()
            {
                ColumnName = columnName,
                SortDirection = direction
            });
        }

        /// <summary>
        /// Add multiple column sorting
        /// IMPLEMENTS: Multi-column SortColumnDescriptions (documented method)
        /// </summary>
        public static void ConfigureMultiSorting(SfDataGrid dataGrid,
            params (string columnName, ListSortDirection direction)[] sortColumns)
        {
            dataGrid.AllowSorting = true;
            dataGrid.AllowTriStateSorting = true;
            dataGrid.ShowSortNumbers = true;

            // Clear existing sorting
            dataGrid.SortColumnDescriptions.Clear();

            // Add multiple sorts
            foreach (var (columnName, direction) in sortColumns)
            {
                dataGrid.SortColumnDescriptions.Add(new SortColumnDescription()
                {
                    ColumnName = columnName,
                    SortDirection = direction
                });
            }
        }

        #endregion

        #region Advanced Grid Operations - DOCUMENTED METHODS

        /// <summary>
        /// Clear all operations programmatically
        /// IMPLEMENTS: ClearGrouping, ClearSorting, ClearFilters (documented methods)
        /// </summary>
        public static void ClearAllOperations(SfDataGrid dataGrid)
        {
            // DOCUMENTED METHODS
            dataGrid.ClearGrouping();
            dataGrid.ClearSorting();
            dataGrid.ClearFilters();
            dataGrid.ClearSelection();
        }

        /// <summary>
        /// Expand/Collapse operations
        /// IMPLEMENTS: ExpandAllGroup, CollapseAllGroup (documented methods)
        /// </summary>
        public static void ExpandCollapseOperations(SfDataGrid dataGrid, bool expand = true)
        {
            if (expand)
            {
                // DOCUMENTED METHODS
                dataGrid.ExpandAllGroup();
                dataGrid.ExpandAllDetailsView();
            }
            else
            {
                // DOCUMENTED METHODS
                dataGrid.CollapseAllGroup();
                dataGrid.CollapseAllDetailsView();
            }
        }

        /// <summary>
        /// Select operations programmatically
        /// IMPLEMENTS: SelectAll, SelectRows (documented methods)
        /// </summary>
        public static void SelectionOperations(SfDataGrid dataGrid, int? startRowIndex = null, int? endRowIndex = null)
        {
            if (startRowIndex.HasValue && endRowIndex.HasValue)
            {
                // DOCUMENTED METHOD
                dataGrid.SelectRows(startRowIndex.Value, endRowIndex.Value);
            }
            else
            {
                // DOCUMENTED METHOD
                dataGrid.SelectAll();
            }
        }

        #endregion

        #region Serialization Operations - DOCUMENTED METHODS

        /// <summary>
        /// Serialize grid configuration to stream
        /// IMPLEMENTS: Serialize (documented method)
        /// </summary>
        public static void SerializeGridConfiguration(SfDataGrid dataGrid, string filePath)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    // DOCUMENTED METHOD
                    dataGrid.Serialize(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to serialize grid configuration: {ex.Message}",
                    "Serialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Deserialize grid configuration from stream
        /// IMPLEMENTS: Deserialize (documented method)
        /// </summary>
        public static void DeserializeGridConfiguration(SfDataGrid dataGrid, string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        // DOCUMENTED METHOD
                        dataGrid.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to deserialize grid configuration: {ex.Message}",
                    "Deserialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Performance & Display Operations - DOCUMENTED METHODS

        /// <summary>
        /// Configure performance optimization
        /// IMPLEMENTS: EnableDataVirtualization, UsePLINQ (documented properties)
        /// </summary>
        public static void ConfigurePerformanceOptimization(SfDataGrid dataGrid, bool enableVirtualization = true)
        {
            // DOCUMENTED PROPERTIES
            dataGrid.EnableDataVirtualization = enableVirtualization;
            dataGrid.UsePLINQ = true; // Enable parallel LINQ for better performance
            dataGrid.ShowBusyIndicator = true;

            // Optimize for large datasets
            if (enableVirtualization)
            {
                dataGrid.AutoSizeController.AutoSizeRange = AutoSizeRange.VisibleRows;
            }
        }

        /// <summary>
        /// Configure row height operations
        /// IMPLEMENTS: RowHeight, HeaderRowHeight (documented properties)
        /// </summary>
        public static void ConfigureRowHeights(SfDataGrid dataGrid, int rowHeight = 30, int headerHeight = 35)
        {
            // DOCUMENTED PROPERTIES
            dataGrid.RowHeight = rowHeight;
            dataGrid.HeaderRowHeight = headerHeight;
        }

        /// <summary>
        /// Configure frozen rows and columns
        /// IMPLEMENTS: FrozenRowCount, FrozenColumnCount (documented properties)
        /// </summary>
        public static void ConfigureFrozenCells(SfDataGrid dataGrid, int frozenRows = 0, int frozenColumns = 0)
        {
            // DOCUMENTED PROPERTIES
            dataGrid.FrozenRowCount = frozenRows;
            dataGrid.FrozenColumnCount = frozenColumns;
        }

        #endregion

        #region Validation Operations - DOCUMENTED METHODS

        /// <summary>
        /// Configure validation settings
        /// IMPLEMENTS: ValidationMode, ShowErrorIcon (documented properties)
        /// </summary>
        public static void ConfigureValidation(SfDataGrid dataGrid, bool showErrorIcons = true)
        {
            // DOCUMENTED PROPERTIES
            dataGrid.ValidationMode = GridValidationMode.InView;
            dataGrid.ShowErrorIcon = showErrorIcons;
            dataGrid.ShowValidationErrorToolTip = showErrorIcons;
        }

        #endregion

        #region Copy/Paste Operations - DOCUMENTED METHODS

        /// <summary>
        /// Configure copy/paste options
        /// IMPLEMENTS: CopyOption, PasteOption (documented properties)
        /// </summary>
        public static void ConfigureCopyPaste(SfDataGrid dataGrid,
            CopyOptions copyOption = CopyOptions.CopyData,
            PasteOptions pasteOption = PasteOptions.PasteData)
        {
            // DOCUMENTED PROPERTIES
            dataGrid.CopyOption = copyOption;
            dataGrid.PasteOption = pasteOption;
        }

        #endregion

        #region Filter Operations - DOCUMENTED METHODS

        /// <summary>
        /// Configure advanced filtering
        /// IMPLEMENTS: FilterRowPosition (documented property)
        /// </summary>
        public static void ConfigureAdvancedFiltering(SfDataGrid dataGrid,
            RowPosition filterPosition = RowPosition.Top)
        {
            // DOCUMENTED PROPERTIES
            dataGrid.AllowFiltering = true;
            dataGrid.FilterRowPosition = filterPosition;
            dataGrid.FilterDelay = 500; // Half-second delay for immediate filtering
        }

        /// <summary>
        /// Clear filters for specific column
        /// IMPLEMENTS: ClearFilter (documented method overloads)
        /// </summary>
        public static void ClearColumnFilter(SfDataGrid dataGrid, string columnName)
        {
            // DOCUMENTED METHOD
            dataGrid.ClearFilter(columnName);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Apply comprehensive advanced configuration
        /// Combines multiple documented methods for complete setup
        /// </summary>
        public static void ApplyAdvancedConfiguration(SfDataGrid dataGrid,
            bool enableGrouping = true,
            bool enablePerformanceOptimization = true,
            bool enableAdvancedFiltering = true)
        {
            // Performance optimization
            if (enablePerformanceOptimization)
            {
                ConfigurePerformanceOptimization(dataGrid);
            }

            // Validation
            ConfigureValidation(dataGrid);

            // Copy/Paste
            ConfigureCopyPaste(dataGrid);

            // Row heights
            ConfigureRowHeights(dataGrid);

            // Advanced filtering
            if (enableAdvancedFiltering)
            {
                ConfigureAdvancedFiltering(dataGrid);
            }

            // Enable comprehensive features if requested
            if (enableGrouping)
            {
                dataGrid.AllowGrouping = true;
                dataGrid.ShowGroupDropArea = true;
                dataGrid.AutoExpandGroups = false; // User controls expansion
            }
            else
            {
                dataGrid.AllowGrouping = false;
                dataGrid.ShowGroupDropArea = false;
            }
        }

        #endregion
    }
}
