using AvtoPro.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace AvtoPro
{
    public class ListParser
    {
        private readonly HttpClient client;
        public ListParser()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://avto.pro");
        }

        public List<TableRow> ParseList(string uri)
        {
            var matchedRows = new List<TableRow>();
            var request = new HttpRequestMessage(HttpMethod.Get, uri.StartsWith("/") ? uri.Substring(1) : uri);
            var result = client.SendAsync(request).Result;
            if (result.StatusCode == HttpStatusCode.Moved)
            {
                result = client.SendAsync(request).Result;
            }
            if (result.StatusCode == HttpStatusCode.Found)
            {
                var match = Regex.Match(result.Content.ReadAsStringAsync().Result, "<a href=\"\\/ (.*)\\/ \">");
                if (match.Success)
                {
                    var partUri = match.Groups[1].Value;
                    result = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, partUri)).Result;
                }
            }
            if (result.StatusCode == HttpStatusCode.OK)
            {
                var doc = new HtmlDocument();
                var html = result.Content.ReadAsStringAsync().Result;
                doc.LoadHtml(html);
                var rows = doc.DocumentNode
                    .SelectNodes("//table[@id='js-partslist-primary']//tr")
                    .Cast<HtmlNode>()
                    .Select(row => new
                    {
                        Row = row,
                        Cells = row.SelectNodes("td")
                                    ?.Cast<HtmlNode>()
                                    .Select(cell => new
                                    {
                                        CellType = cell.GetAttributeValue("data-type", string.Empty),
                                        Text = Regex.Replace(cell.InnerText.Trim(), "(\\r\\n)+|\\r+|\\n+|\\t+", string.Empty)
                                    })
                    });
                foreach (var row in rows)
                {
                    if (row.Cells != null)
                    {
                        var matchedRow = new TableRow();
                        foreach (var cell in row.Cells)
                        {
                            switch (cell.CellType)
                            {
                                case "maker":
                                    matchedRow.Brand = cell.Text;
                                    break;
                                case "code":
                                    matchedRow.Id = cell.Text;
                                    break;
                                case "delivery":
                                    matchedRow.DeliveryText = cell.Text;
                                    break;
                                case "price":
                                    matchedRow.Price = cell.Text;
                                    break;
                            }
                        }
                        matchedRows.Add(matchedRow);
                    }
                }
            }
            else
            {
                return null;
            }
            return matchedRows;
        }
    }
}
