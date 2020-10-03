using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Denali.Services.Google
{
    public class GoogleSheetsService
    {
        private static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static string ApplicationName = "Denali";
        private readonly SheetsService _sheetsService;

        public GoogleSheetsService()
        {
            var credential = LoadCredentials();
            // Create Google Sheets API service.
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public Spreadsheet CreateSheet(string title)
        {
            Spreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.Properties = new SpreadsheetProperties
            {
                Title = title
            };

            return _sheetsService.Spreadsheets.Create(spreadsheet).Execute();
        }

        public void UpdateSheet(string spreadsheetId, List<ValueRange> ranges, string valueInputOption)
        {
            var request = new BatchUpdateValuesRequest();
            request.Data = ranges;
            request.ValueInputOption = valueInputOption;

            _sheetsService.Spreadsheets.Values.BatchUpdate(request, spreadsheetId).Execute();
        }

        public void UpdateSheetWithPositiveNegativeFormat(string sheetId, int startColumIndex = 6, int startRowIndex = 0, int endColumnIndex = 7, int endRowIndex = 20)
        {
            Request formatRequestNegative = new Request
            {
                AddConditionalFormatRule = new AddConditionalFormatRuleRequest
                {
                    Rule = new ConditionalFormatRule
                    {
                        Ranges = new List<GridRange>
                        {
                            new GridRange
                            {
                                SheetId = 0,
                                StartColumnIndex = startColumIndex,
                                EndColumnIndex = endColumnIndex,
                                StartRowIndex = startRowIndex,
                                EndRowIndex = endRowIndex
                            }
                        },
                        BooleanRule = new BooleanRule
                        {
                            Condition = new BooleanCondition
                            {
                                Type = "NUMBER_LESS_THAN_EQ",
                                Values = new List<ConditionValue>() {
                                  new ConditionValue()
                                  {
                                      UserEnteredValue = "0"
                                  }
                              }
                            },
                            Format = new CellFormat
                            {
                                BackgroundColor = new Color()
                                {
                                    Red = 1f,
                                    Green = 0.64f,
                                    Blue = 0.64f,
                                    Alpha = 1f
                                }
                            }
                        },
                    },
                }
            };

            Request formatRequestPositive = new Request
            {
                AddConditionalFormatRule = new AddConditionalFormatRuleRequest
                {
                    Rule = new ConditionalFormatRule
                    {
                        Ranges = new List<GridRange>
                        {
                            new GridRange
                            {
                                SheetId = 0,
                                StartColumnIndex = startColumIndex,
                                EndColumnIndex = endColumnIndex,
                                StartRowIndex = startRowIndex,
                                EndRowIndex = endRowIndex
                            }
                        },
                        BooleanRule = new BooleanRule
                        {
                            Condition = new BooleanCondition
                            {
                                Type = "NUMBER_GREATER_THAN_EQ",
                                Values = new List<ConditionValue>() {
                                  new ConditionValue()
                                  {
                                      UserEnteredValue = "0"
                                  }
                              }
                            },
                            Format = new CellFormat
                            {
                                BackgroundColor = new Color()
                                {
                                    Red = 0.64f,
                                    Green = 1f,
                                    Blue = 0.64f,
                                    Alpha = 1f
                                }
                            }
                        },
                    },
                }
            };

            BatchUpdateSpreadsheetRequest updateRequest = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request> { formatRequestNegative, formatRequestPositive }
            };

            _sheetsService.Spreadsheets.BatchUpdate(updateRequest, sheetId).Execute();
        }
        private UserCredential LoadCredentials()
        {
            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            return credential;
        }
    }
}
