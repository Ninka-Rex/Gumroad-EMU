using Fiddler;
using System;
using System.Windows.Forms;

namespace Gumroad_EMU
{
    internal class Server
    {
        // Static members to hold reference to UI controls
        public static RichTextBox txtConsole;
        public static Button btnStart;
        public static Button btnStop;

        // Set method to initialize UI controls
        public static void Set(RichTextBox console, Button start, Button stop)
        {
            txtConsole = console;
            btnStart = start;
            btnStop = stop;
        }

        // Method to start the Fiddler server
        public static void Start()
        {
            // Set Fiddler logs to the provided console
            Logs.SetConsole(txtConsole);

            // Event handler for intercepting requests before they are sent
            FiddlerApplication.BeforeRequest += session =>
            {
                // Check if the request is to api.gumroad.com
                if (session.fullUrl.Contains("api.gumroad.com"))
                {
                    // Handling CONNECT method for HTTPS requests
                    if (session.HTTPMethodIs("CONNECT"))
                    {
                        session.oFlags["x-replywithtunnel"] = "GenerateTunnel";
                        return;
                    }

                    // Bypass the server and create a custom response
                    session.utilCreateResponseAndBypassServer();
                    //Logs.Info(session.ToString());
                    session.oResponse.headers.HTTPResponseCode = 200;
                    session.oResponse.headers.HTTPResponseStatus = "200 OK";
                    session.oResponse.headers["Content-Type"] = "application/json; charset=utf-8";
                    session.oResponse.headers["Content-Encoding"] = "gzip";
                    session.utilSetResponseBody(responder.makeResponse(session.ToString()));
                    session.oResponse.headers["Content-Length"] = session.responseBodyBytes.Length.ToString();
                }
            };

            // Check if the root certificate exists; if not, create and trust it
            if (!CertMaker.rootCertExists())
            {
                Logs.Info("Please trust the certificate to intercept HTTPS requests.");
                Logs.Info("It will be deleted when Gumroad-EMU is closed.");

                if (!CertMaker.createRootCert())
                    throw new Exception("Unable to create certificate.");

                if (!CertMaker.trustRootCert())
                {
                    Logs.Error("Unable to trust certificate!");
                    btnStart.Enabled = true; btnStop.Enabled = false;
                }
            }

            // Configure Fiddler settings and start the server
            var startupSettings = new FiddlerCoreStartupSettingsBuilder()
                .ListenOnPort(8888)
                .RegisterAsSystemProxy()
                .AllowRemoteClients()
                .DecryptSSL()
                .Build();

            FiddlerApplication.Startup(startupSettings);
            Logs.Info("Server is running on port " + startupSettings.ListenPort.ToString());
            btnStart.Enabled = false; btnStop.Enabled = true;
        }

        // Method to stop the Fiddler server
        public static void Stop()
        {
            try
            {
                // Dispose of the certificate and remove Fiddler-generated certs
                CertMaker.DoDispose();
                CertMaker.removeFiddlerGeneratedCerts();

                // Shut down the Fiddler application
                FiddlerApplication.Shutdown();
                Logs.Info("Server stopped.");
                btnStart.Enabled = true; btnStop.Enabled = false;
            }
            catch (Exception e)
            {
                // Handle exceptions and log errors
                btnStart.Enabled = false; btnStop.Enabled = true;
                Logs.Error(e.ToString());
            }
        }
    }
}
