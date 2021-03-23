using AvtoPro.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvtoPro
{
    public static class ExcelFile
    {
        public delegate void OnSaveProgressChanged(int saveProgress);
        public static event OnSaveProgressChanged SaveProgressChagned;

        public delegate void OnSaveCompleted();
        public static event OnSaveCompleted SaveCompleted;

        public static string UpdateFailedResults(List<SearchResult> failedReqestsSearchReluts, string fileName)
        {
            SaveProgressChagned(0);
            int counter = 1;
            var document = SpreadsheetDocument.Open(fileName, true);
            var workbookPart = document.WorkbookPart;
            var worksheetPart = workbookPart.WorksheetParts.First();
            var data = worksheetPart.Worksheet.Elements<SheetData>().First();
            
            uint rowIndex = 1;
            foreach (var row in data.Elements<Row>().Skip(1))
            {
                rowIndex++;
                var cells = row.Elements<Cell>();
                var id = GetCellValue(cells.ElementAt(0), workbookPart);
                var brand = GetCellValue(cells.ElementAt(1), workbookPart);
                if (failedReqestsSearchReluts.Any(r => r.Id.Equals(id)))
                {
                    var searchResutl = failedReqestsSearchReluts.First(r => r.Id.Equals(id));
                    if (searchResutl.IsSuccessful)
                    {
                        InsertNumber(Regex.Replace(searchResutl.FirstPrice?.Replace(',', '.') ?? string.Empty, @"[\s+]", ""), "D", rowIndex, worksheetPart, row);
                        InsertNumber(Regex.Replace(searchResutl.SecondPrice?.Replace(',', '.') ?? string.Empty, @"[\s+]", ""), "E", rowIndex, worksheetPart, row);
                    }
                    else
                    {
                        if (searchResutl.IsCanceled)
                        {
                            InsertText("Search Canceled", "D", rowIndex, worksheetPart, row);
                        }
                        else if (searchResutl.IsSkiped)
                        {
                            InsertText("Search Skiped", "D", rowIndex, worksheetPart, row);
                        }
                        else
                        {
                            InsertText("Search Failed", "D", rowIndex, worksheetPart, row);
                        }
                    }
                }
                SaveProgressChagned(counter * 100 / failedReqestsSearchReluts.Count);
                counter++;
            }
            worksheetPart.Worksheet.Save();
            workbookPart.Workbook.Save();
            document.Save();
            document.Close();
            document.Dispose();
            SaveCompleted();
            return "results Updated";
        }
        public static List<ReadFileResult> GetSearchRequests(List<string> fileNames)
        {
            var result = new List<ReadFileResult>();
            foreach (var fileName in fileNames)
            {
                var fileResult = new ReadFileResult { FileName = fileName, Requests = new List<SearchRequest>() };

                try
                {
                    using (var document = SpreadsheetDocument.Open(fileName, false))
                    {
                        var workbookPart = document.WorkbookPart;
                        var data = workbookPart.WorksheetParts.First().Worksheet.Elements<SheetData>().First();
                        foreach (var row in data.Elements<Row>().Skip(1))
                        {
                            var cells = row.Elements<Cell>();
                            fileResult.Requests.Add(new SearchRequest
                            {
                                Id = GetCellValue(cells.ElementAt(0), workbookPart),
                                Brand = GetCellValue(cells.ElementAt(1), workbookPart),
                                OriginalPrice = GetCellValue(cells.ElementAt(4), workbookPart)
                            });
                        }
                        fileResult.Success = true;
                    }
                }
                catch
                {
                    fileResult.Success = false;
                }
                result.Add(fileResult);
            }
            return result;
        }

        public static string SaveResults(List<SearchResult> searchResults, string fileName)
        {
            SaveProgressChagned(0);
            int counter = 1;
            SpreadsheetDocument spreadsheetDocument;
            try
            {
                spreadsheetDocument = SpreadsheetDocument.Create(fileName, SpreadsheetDocumentType.Workbook);
            }
            catch (Exception)
            {
                return "Failed to create file";
            }
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            var data = new SheetData();
            worksheetPart.Worksheet = new Worksheet(data);

            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                AppendChild<Sheets>(new Sheets());
            Sheet sheet = new Sheet()
            {
                Id = spreadsheetDocument.WorkbookPart.
                GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "SearchResults"
            };
            sheets.Append(sheet);

            uint rowIndex = 1;

            var row = new Row() { RowIndex = rowIndex };
            data.Append(row);

            InsertText("Id", "A", rowIndex, worksheetPart , row);
            InsertText("Brand", "B", rowIndex, worksheetPart, row);
            InsertText("Original price, EUR", "C", rowIndex, worksheetPart, row);
            InsertText("Alternative price 1, UAH", "D", rowIndex, worksheetPart, row);
            InsertText("Alternative price 2, UAH", "E", rowIndex, worksheetPart, row);
            try
            {
                foreach (var searchResutl in searchResults)
                {
                    rowIndex++;

                    row = new Row() { RowIndex = rowIndex };
                    data.Append(row);

                    InsertText(searchResutl.Id, "A", rowIndex, worksheetPart, row);
                    InsertText(searchResutl.Brand, "B", rowIndex, worksheetPart, row);
                    InsertNumber(Regex.Replace(searchResutl.OriginalPrice?.Replace(',', '.') ?? string.Empty, @"[\s+]", ""), "C", rowIndex, worksheetPart, row);
                    if (searchResutl.IsSuccessful)
                    {
                        InsertNumber(Regex.Replace(searchResutl.FirstPrice?.Replace(',', '.') ?? string.Empty, @"[\s+]", ""), "D", rowIndex, worksheetPart, row);
                        InsertNumber(Regex.Replace(searchResutl.SecondPrice?.Replace(',', '.') ?? string.Empty, @"[\s+]", ""), "E", rowIndex, worksheetPart, row);
                    }
                    else
                    {
                        if (searchResutl.IsCanceled)
                        {
                            InsertText("Search Canceled", "D", rowIndex, worksheetPart, row);
                        }
                        else if (searchResutl.IsSkiped)
                        {
                            InsertText("Search Skiped", "D", rowIndex, worksheetPart, row);
                        }
                        else
                        {
                            InsertText("Search Failed", "D", rowIndex, worksheetPart, row);
                        }
                    }
                    SaveProgressChagned(counter * 100 / searchResults.Count);
                    counter++;
                }
            }
            catch (Exception)
            {
                throw;
            }
            worksheetPart.Worksheet.Save();
            workbookpart.Workbook.Save();
            spreadsheetDocument.Save();
            spreadsheetDocument.Close();
            spreadsheetDocument.Dispose();
            SaveCompleted();
            return "Results saved";
        }

        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart, Row row)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

           
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }
        }

        private static void InsertNumber(string text, string columnName, uint rowIndex, WorksheetPart worksheetPart, Row row)
        {
            Cell cell = InsertCellInWorksheet(columnName, rowIndex, worksheetPart, row);

            cell.CellValue = new CellValue() { Text = text };
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        }

        private static void InsertText(string text, string columnName, uint rowIndex, WorksheetPart worksheetPart, Row row)
        {
            Cell cell = InsertCellInWorksheet(columnName, rowIndex, worksheetPart, row);

            cell.CellValue = new CellValue(text);
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
        }

        private static string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            string cellValue = null;
            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
            {
                var stringId = Convert.ToInt32(cell.InnerText);
                cellValue = workbookPart.SharedStringTablePart.SharedStringTable
                    .Elements<SharedStringItem>().ElementAt(stringId).InnerText;
            }
            else
            {
                cellValue = cell.InnerText;
            }
            return cellValue;
        }

    }
}
