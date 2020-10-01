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
