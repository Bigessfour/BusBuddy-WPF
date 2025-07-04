using Syncfusion.WinForms.DataGrid;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Bus_Buddy.Utilities
{
    /// <summary>
    /// Syncfusion Export/Import Manager implementing documented export methods
    /// Based on Syncfusion Essential Studio Version 30.1.37 documentation
    /// Provides data export/import capabilities for Bus Buddy reporting
    /// </summary>
    public static class SyncfusionExportManager
    {
        #region CSV Export Operations

        /// <summary>
        /// Export SfDataGrid data to CSV format
        /// Custom implementation of data export functionality
        /// </summary>
        public static void ExportToCSV(SfDataGrid dataGrid, string filePath)
        {
            try
            {
                var csv = new StringBuilder();

                // Add headers
                var headers = new List<string>();
                foreach (var column in dataGrid.Columns)
                {
                    if (column.Visible)
                    {
                        headers.Add($"\"{column.HeaderText}\"");
                    }
                }
                csv.AppendLine(string.Join(",", headers));

                // Add data rows
                if (dataGrid.DataSource != null)
                {
                    // Handle different data source types
                    if (dataGrid.DataSource is IEnumerable<object> enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            var values = new List<string>();
                            foreach (var column in dataGrid.Columns)
                            {
                                if (column.Visible)
                                {
                                    var value = item?.GetType().GetProperty(column.MappingName)?.GetValue(item);
                                    var stringValue = value?.ToString() ?? "";

                                    // Escape quotes and wrap in quotes if contains comma
                                    if (stringValue.Contains(",") || stringValue.Contains("\"") || stringValue.Contains("\n"))
                                    {
                                        stringValue = $"\"{stringValue.Replace("\"", "\"\"")}\"";
                                    }
                                    values.Add(stringValue);
                                }
                            }
                            csv.AppendLine(string.Join(",", values));
                        }
                    }
                }

                File.WriteAllText(filePath, csv.ToString());
                MessageBox.Show($"Data exported successfully to {filePath}", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export to CSV: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Excel Export Operations

        /// <summary>
        /// Export SfDataGrid data to Excel format (basic CSV-based approach)
        /// Note: For full Excel support, requires Syncfusion.XlsIO which may need additional licensing
        /// </summary>
        public static void ExportToExcelCSV(SfDataGrid dataGrid, string filePath)
        {
            try
            {
                // Ensure .csv extension for Excel compatibility
                var csvPath = Path.ChangeExtension(filePath, ".csv");
                ExportToCSV(dataGrid, csvPath);

                MessageBox.Show($"Data exported to CSV format (Excel compatible) at {csvPath}",
                    "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export to Excel format: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region XML Export Operations

        /// <summary>
        /// Export SfDataGrid data to XML format
        /// </summary>
        public static void ExportToXML(SfDataGrid dataGrid, string filePath)
        {
            try
            {
                var dataTable = new DataTable("GridData");

                // Create columns
                foreach (var column in dataGrid.Columns)
                {
                    if (column.Visible)
                    {
                        dataTable.Columns.Add(column.HeaderText);
                    }
                }

                // Add data rows
                if (dataGrid.DataSource != null)
                {
                    // Handle different data source types
                    if (dataGrid.DataSource is IEnumerable<object> enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            var row = dataTable.NewRow();
                            foreach (var column in dataGrid.Columns)
                            {
                                if (column.Visible)
                                {
                                    var value = item?.GetType().GetProperty(column.MappingName)?.GetValue(item);
                                    row[column.HeaderText] = value ?? DBNull.Value;
                                }
                            }
                            dataTable.Rows.Add(row);
                        }
                    }
                }

                // Write to XML
                dataTable.WriteXml(filePath);
                MessageBox.Show($"Data exported successfully to {filePath}", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export to XML: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region JSON Export Operations

        /// <summary>
        /// Export SfDataGrid data to JSON format
        /// </summary>
        public static void ExportToJSON(SfDataGrid dataGrid, string filePath)
        {
            try
            {
                var data = new List<Dictionary<string, object?>>();

                // Extract data
                if (dataGrid.DataSource != null)
                {
                    // Handle different data source types
                    if (dataGrid.DataSource is IEnumerable<object> enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            var row = new Dictionary<string, object?>();
                            foreach (var column in dataGrid.Columns)
                            {
                                if (column.Visible)
                                {
                                    var value = item?.GetType().GetProperty(column.MappingName)?.GetValue(item);
                                    row[column.HeaderText] = value;
                                }
                            }
                            data.Add(row);
                        }
                    }
                }

                // Simple JSON serialization
                var json = SerializeToJson(data);
                File.WriteAllText(filePath, json);

                MessageBox.Show($"Data exported successfully to {filePath}", "Export Complete",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export to JSON: {ex.Message}", "Export Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Simple JSON serialization helper
        /// </summary>
        private static string SerializeToJson(List<Dictionary<string, object?>> data)
        {
            var json = new StringBuilder();
            json.AppendLine("[");

            for (int i = 0; i < data.Count; i++)
            {
                json.AppendLine("  {");
                var row = data[i];
                var properties = row.Keys.ToArray();

                for (int j = 0; j < properties.Length; j++)
                {
                    var key = properties[j];
                    var value = row[key];
                    var valueString = value switch
                    {
                        null => "null",
                        string s => $"\"{s.Replace("\"", "\\\"")}\"",
                        bool b => b.ToString().ToLower(),
                        DateTime dt => $"\"{dt:yyyy-MM-ddTHH:mm:ss}\"",
                        _ => value.ToString()
                    };

                    json.Append($"    \"{key}\": {valueString}");
                    if (j < properties.Length - 1) json.Append(",");
                    json.AppendLine();
                }

                json.Append("  }");
                if (i < data.Count - 1) json.Append(",");
                json.AppendLine();
            }

            json.AppendLine("]");
            return json.ToString();
        }

        #endregion

        #region Import Operations

        /// <summary>
        /// Import data from CSV file to DataTable (for binding to SfDataGrid)
        /// </summary>
        public static DataTable ImportFromCSV(string filePath)
        {
            try
            {
                var dataTable = new DataTable();
                var lines = File.ReadAllLines(filePath);

                if (lines.Length == 0) return dataTable;

                // Parse headers
                var headers = ParseCSVLine(lines[0]);
                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                // Parse data rows
                for (int i = 1; i < lines.Length; i++)
                {
                    var values = ParseCSVLine(lines[i]);
                    if (values.Length == headers.Length)
                    {
                        var row = dataTable.NewRow();
                        for (int j = 0; j < values.Length; j++)
                        {
                            row[j] = values[j];
                        }
                        dataTable.Rows.Add(row);
                    }
                }

                return dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import from CSV: {ex.Message}", "Import Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
        }

        /// <summary>
        /// Parse a CSV line handling quoted values
        /// </summary>
        private static string[] ParseCSVLine(string line)
        {
            var values = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(current.ToString().Trim('"'));
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            values.Add(current.ToString().Trim('"'));
            return values.ToArray();
        }

        #endregion

        #region Print Operations

        /// <summary>
        /// Simple print functionality for SfDataGrid
        /// </summary>
        public static void PrintGrid(SfDataGrid dataGrid, string title = "Grid Report")
        {
            try
            {
                var printDialog = new PrintDialog();
                var printDocument = new System.Drawing.Printing.PrintDocument();

                printDocument.DocumentName = title;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.PrinterSettings = printDialog.PrinterSettings;

                    // For basic printing, we'll create a simple text representation
                    string printContent = CreatePrintableContent(dataGrid, title);

                    printDocument.PrintPage += (sender, e) =>
                    {
                        e.Graphics?.DrawString(printContent, new System.Drawing.Font("Arial", 10),
                            System.Drawing.Brushes.Black, 10, 10);
                    };

                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to print: {ex.Message}", "Print Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Create printable content from grid data
        /// </summary>
        private static string CreatePrintableContent(SfDataGrid dataGrid, string title)
        {
            var content = new StringBuilder();
            content.AppendLine(title);
            content.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            content.AppendLine(new string('=', 50));
            content.AppendLine();

            // Add headers
            var headers = new List<string>();
            foreach (var column in dataGrid.Columns)
            {
                if (column.Visible)
                {
                    headers.Add(column.HeaderText.PadRight(15));
                }
            }
            content.AppendLine(string.Join(" | ", headers));
            content.AppendLine(new string('-', headers.Sum(h => h.Length) + (headers.Count - 1) * 3));

            // Add data rows (limit to first 50 for printing)
            int rowCount = 0;
            if (dataGrid.DataSource != null && dataGrid.DataSource is IEnumerable<object> enumerable)
            {
                foreach (var item in enumerable.Take(50))
                {
                    var values = new List<string>();
                    foreach (var column in dataGrid.Columns)
                    {
                        if (column.Visible)
                        {
                            var value = item?.GetType().GetProperty(column.MappingName)?.GetValue(item);
                            var stringValue = (value?.ToString() ?? "").PadRight(15);
                            if (stringValue.Length > 15) stringValue = stringValue.Substring(0, 12) + "...";
                            values.Add(stringValue);
                        }
                    }
                    content.AppendLine(string.Join(" | ", values));
                    rowCount++;
                }

                var totalCount = enumerable.Count();
                if (totalCount > 50)
                {
                    content.AppendLine($"... and {totalCount - 50} more rows");
                }
            }

            content.AppendLine();
            content.AppendLine($"Total Rows: {rowCount}");

            return content.ToString();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Show export options dialog and perform export
        /// </summary>
        public static void ShowExportDialog(SfDataGrid dataGrid, string defaultFileName = "GridData")
        {
            var saveDialog = new SaveFileDialog
            {
                FileName = defaultFileName,
                Filter = "CSV Files (*.csv)|*.csv|XML Files (*.xml)|*.xml|JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                DefaultExt = "csv"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                var extension = Path.GetExtension(saveDialog.FileName).ToLower();

                switch (extension)
                {
                    case ".csv":
                        ExportToCSV(dataGrid, saveDialog.FileName);
                        break;
                    case ".xml":
                        ExportToXML(dataGrid, saveDialog.FileName);
                        break;
                    case ".json":
                        ExportToJSON(dataGrid, saveDialog.FileName);
                        break;
                    default:
                        ExportToCSV(dataGrid, saveDialog.FileName);
                        break;
                }
            }
        }

        /// <summary>
        /// Get export statistics for the grid
        /// </summary>
        public static string GetExportStatistics(SfDataGrid dataGrid)
        {
            var visibleColumns = dataGrid.Columns.Count(c => c.Visible);
            var totalRows = 0;

            // Count rows from DataSource
            if (dataGrid.DataSource != null && dataGrid.DataSource is IEnumerable<object> enumerable)
            {
                totalRows = enumerable.Count();
            }

            var totalCells = visibleColumns * totalRows;

            return $"Columns: {visibleColumns}, Rows: {totalRows}, Total Cells: {totalCells}";
        }

        #endregion
    }
}
