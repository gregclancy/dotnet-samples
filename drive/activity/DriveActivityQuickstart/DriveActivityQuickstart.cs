﻿// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START drive_activity_quickstart]
using Google.Apis.Auth.OAuth2;
using Google.Apis.Appsactivity.v1;
using Google.Apis.Appsactivity.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace DriveActivityQuickstart
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
	// in token.json
	static string[] Scopes = { AppsactivityService.Scope.Activity };
        static string ApplicationName = "Drive API .NET Quickstart";

        static void Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

	    // Create Google Drive Activity API service.
            var service = new AppsactivityService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            ActivitiesResource.ListRequest listRequest = service.Activities.List();
            listRequest.Source = "drive.google.com";
            listRequest.DriveAncestorId = "root";
            listRequest.PageSize = 10;

            // List activities.
	    IList<Google.Apis.Appsactivity.v1.Data.Activity> activities = listRequest.Execute().Activities;
            Console.WriteLine("Activities:");
            if (activities != null && activities.Count > 0)
            {
                foreach (var activity in activities)
                {
                    Event activityEvent = activity.CombinedEvent;
                    User user = activityEvent.User;
                    Target target = activityEvent.Target;
                    if (user != null && target != null)
                    {
                        var epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        DateTime date = epoc.AddMilliseconds(
                            (long)activityEvent.EventTimeMillis).ToLocalTime();
                        Console.WriteLine("{0}: {1} {2} {3} {4}", date, user.Name,
                            activityEvent.PrimaryEventType, target.Name, target.MimeType);
                    }
                }
            }
            else
            {
                Console.WriteLine("No recent activity.");
            }
            Console.Read();
        }
    }
}
// [END drive_activity_quickstart]
