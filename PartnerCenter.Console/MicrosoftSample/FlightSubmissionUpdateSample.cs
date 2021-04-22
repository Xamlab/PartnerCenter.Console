using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PartnerCenter.Console.Services;

namespace PartnerCenter.Console.MicrosoftSample
{
     /// <summary>
    /// Demonstrates how to update a flight submission with a new package
    /// </summary>
    public class FlightSubmissionUpdateSample
    {
        private ClientConfiguration ClientConfig { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">An instance of ClientConfiguration that contains all parameters populated</param>
        [DebuggerStepThrough]
        public FlightSubmissionUpdateSample(ClientConfiguration c)
        {
            this.ClientConfig = c;
        }

        public void RunFlightSubmissionUpdateSample(string bundlePath, ITokenService tokenService)
        {
            // **********************
            //       SETTINGS
            // **********************
            var appId = this.ClientConfig.ApplicationId;
            var flightId = this.ClientConfig.FlightId;
            var clientId = this.ClientConfig.ClientId;
            var clientSecret = this.ClientConfig.ClientSecret;
            var serviceEndpoint = this.ClientConfig.ServiceUrl;
            var tokenEndpoint = this.ClientConfig.TokenEndpoint;
            var scope = this.ClientConfig.Scope;

            // Get authorization token
            System.Console.WriteLine("Getting authorization token ");
            var accessToken = tokenService.GetPartnerCenterTokenAsync().Result.AccessToken;

            System.Console.WriteLine("Getting flight");
            var client = new IngestionClient(accessToken, serviceEndpoint);

            dynamic flight = client.Invoke<dynamic>(
                HttpMethod.Get,
                relativeUrl: string.Format(
                    CultureInfo.InvariantCulture,
                    IngestionClient.GetFlightUrlTemplate,
                    IngestionClient.Version,
                    IngestionClient.Tenant,
                    appId,
                    flightId),
                requestContent: null).Result;
            System.Console.WriteLine(flight.ToString());

            if (flight.pendingFlightSubmission != null)
            {
                var submissionId = flight.pendingFlightSubmission.id.Value as string;

                // Let's try deleting it. If it was NOT creationg via the API, then you need to
                // manually delete it from the dashboard. This is a safety measure to make sure that a
                // human user and an automated system don't make conflicting edits.
                System.Console.WriteLine("Deleting the pending submission");

                client.Invoke<dynamic>(
                    HttpMethod.Delete,
                    relativeUrl: string.Format(
                        CultureInfo.InvariantCulture,
                        IngestionClient.GetFlightSubmissionUrlTemplate,
                        IngestionClient.Version,
                        IngestionClient.Tenant,
                        appId,
                        flightId,
                        submissionId),
                    requestContent: null).Wait();
            }

            // Create a new submission, which will be an exact copy of the last published submission.
            System.Console.WriteLine("Creating a new submission");
            dynamic flightSubmission = client.Invoke<dynamic>(
                HttpMethod.Post,
                relativeUrl: string.Format(
                    CultureInfo.InvariantCulture,
                    IngestionClient.CreateFlightSubmissionUrlTemplate,
                    IngestionClient.Version,
                    IngestionClient.Tenant,
                    appId,
                    flightId),
                requestContent: null).Result;

            // Update packages.
            // Let's say we want to delete the existing package:
            flightSubmission.flightPackages[0].fileStatus = "PendingDelete";

            // Let's add a new package.
            var packages = new List<dynamic>();
            packages.Add(flightSubmission.flightPackages[0]);
            packages.Add(
                new
                {
                    fileStatus = "PendingUpload",
                    fileName = Path.GetFileName(bundlePath),
                });


            flightSubmission.flightPackages = JToken.FromObject(packages.ToArray());
            var flightSubmissionId = flightSubmission.id.Value as string;

            // Upload the zip archive with all new files to the SAS URL returned with the submission.
            var fileUploadUrl = (string)flightSubmission.fileUploadUrl.Value;
            System.Console.WriteLine("FileUploadUrl: " + fileUploadUrl);
            System.Console.WriteLine("Uploading file");
            IngestionClient.UploadFileToBlob(bundlePath, fileUploadUrl).Wait();

            // Update the submission.
            System.Console.WriteLine("Updating the submission");
            client.Invoke<dynamic>(
                HttpMethod.Put,
                relativeUrl: string.Format(
                    CultureInfo.InvariantCulture,
                    IngestionClient.GetFlightSubmissionUrlTemplate,
                    IngestionClient.Version,
                    IngestionClient.Tenant,
                    appId,
                    flightId,
                    flightSubmissionId),
                requestContent: flightSubmission).Wait();

            // Tell the system that we are done updating the submission.
            System.Console.WriteLine("Committing the submission");
            client.Invoke<dynamic>(
                HttpMethod.Post,
                relativeUrl: string.Format(
                    CultureInfo.InvariantCulture,
                    IngestionClient.CommitFlightSubmissionUrlTemplate,
                    IngestionClient.Version,
                    IngestionClient.Tenant,
                    appId,
                    flightId,
                    flightSubmissionId),
                requestContent: null).Wait();

            // Periodically check the status until it changes from "CommitsStarted" to either
            // successful status or a failure.
            System.Console.WriteLine("Waiting for the submission commit processing to complete. This may take a couple of minutes.");
            string submissionStatus = null;
            do
            {
                Task.Delay(TimeSpan.FromSeconds(5)).Wait();
                dynamic statusResource = client.Invoke<dynamic>(
                    HttpMethod.Get,
                    relativeUrl: string.Format(
                        CultureInfo.InvariantCulture,
                        IngestionClient.FlightSubmissionStatusUrlTemplate,
                        IngestionClient.Version,
                        IngestionClient.Tenant,
                        appId,
                        flightId,
                        flightSubmissionId),
                    requestContent: null).Result;

                submissionStatus = statusResource.status.Value as string;
                System.Console.WriteLine("Current status: " + submissionStatus);
            }
            while ("CommitStarted".Equals(submissionStatus));

            if ("CommitFailed".Equals(submissionStatus))
            {
                System.Console.WriteLine("Submission has failed. Please check the Errors collection of the submissionResource response.");
            }
            else
            {
                System.Console.WriteLine("Submission commit success!");
            }
        }
    }
}