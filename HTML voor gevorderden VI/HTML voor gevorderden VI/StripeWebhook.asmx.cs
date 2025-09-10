using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Stripe;
using Stripe.Checkout;

namespace HTML_voor_gevorderden_VI
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class StripeWebhook : System.Web.Services.WebService
    {
        // Use environment variables or secure configuration instead of hardcoding the secret
        //private const string Secret = "whsec_939da226210278e828d46ea9149dbd6ff6e41576768eef8ae60b038fdd7b0bf4"; // stripe CLI
        private const string Secret = "whsec_r4PQG3pC35jBphBtVbBl3o8FhUdmRXmw"; // Ngrok Stripe Dashboard

        [WebMethod]
        public void HandleWebhook()
        {
            try
            {
                // Lees de Stripe-handtekening en JSON-inhoud
                string stripeSignature = Context.Request.Headers["Stripe-Signature"];
                using (StreamReader reader = new StreamReader(Context.Request.InputStream))
                {
                    string json = reader.ReadToEnd();

                    // Controleer en valideer het webhook-event
                    var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, Secret);

                    if (stripeEvent.Type == "payment_intent.succeeded")
                    {
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                        if (paymentIntent != null)
                        {
                            // Log de webhook-data
                            //string logPath = HttpContext.Current.Server.MapPath("~/stripe_webhook_log.txt");
                            //string logEntry = $"{paymentIntent.Metadata["email"]} \n\n";

                            try
                            {
                                //using (StreamWriter sw = System.IO.File.AppendText(logPath))
                                //{
                                    //sw.WriteLine(logEntry);
                                //}
                            }
                            catch
                            {

                            }
                            //string email = paymentIntent.Metadata["email"];
                            //string bestelguid = paymentIntent.Metadata["bestelguid"];
                            //string klantid = paymentIntent.Metadata["klantid"];
                            //Mail(email, bestelguid, klantid);
                            Mailempty();
                        }
                    }

                }
            }
            catch (StripeException stripeEx)
            {
                LogError($"Stripe-webhook verificatie mislukt: {stripeEx.Message}");
            }
            catch (Exception ex)
            {
                LogError($"Fout bij verwerken webhook: {ex.Message}");
            }
        }

        private void LogError(string message)
        {
            Console.Error.WriteLine($"[ERROR] {message}");
        }

        public void Mail(string email, string bestelguid, string klantid)
        {

            // SMTP-configuratie
            using (SmtpClient smtp = new SmtpClient("mail.aspworld.nl"))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("webshop@aspworld.nl", "^9ffo75D6");
                smtp.EnableSsl = false;
                smtp.Port = 25;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("webshop@aspworld.nl"),
                    Subject = "Mail van Webhook op Webshopgenerator",
                    Body = "Bestelguid:  " + bestelguid + "\r\n" + "KlantId:  " + klantid + "\r\n",
                };
                mailMessage.To.Add(email);
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = false;
                smtp.Send(mailMessage);
            }
        }

        public void Mailempty()
        {

            // SMTP-configuratie
            using (SmtpClient smtp = new SmtpClient("mail.aspworld.nl"))
            {
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("webshop@aspworld.nl", "^9ffo75D6");
                smtp.EnableSsl = false;
                smtp.Port = 25;

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("webshop@aspworld.nl"),
                    Subject = "Mail van Webhook op Webshopgenerator",
                    Body = "",
                };
                mailMessage.To.Add("maurice.lemmens5678@gmail.com");
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = false;
                smtp.Send(mailMessage);
            }
        }
    }


}
