using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;


namespace HTML_voor_gevorderden_VI
{
    /// <summary>
    /// Summary description for BetaalService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BetaalService : System.Web.Services.WebService
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["pcfilters"].ConnectionString;

        [WebMethod]
        public string UpdateBetaalmethode(string KlantId, string Bestelguid, string Betaalmethode)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Bezorging_en_betaling SET Betaalmethode = @Betaalmethode WHERE KlantId = @KlantId AND Bestelguid = @Bestelguid";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Betaalmethode", Betaalmethode);
                        cmd.Parameters.AddWithValue("@KlantId", KlantId);
                        cmd.Parameters.AddWithValue("@Bestelguid", Bestelguid);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0 ? "Update succesvol" : "Update mislukt, geen records gevonden";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Fout bij het updaten: " + ex.Message;
            }
        }

        public class values
        {
            public string sessionId;
            public string totaal;
        }

        [WebMethod(EnableSession = true)]
        public values CreateSession(string bestelguid, string klantid, string totaalbedrag, string email, string betaalmethode)
        {
            //double totaal;
            //totaal = double.Parse(totaalbedrag);
            //long amount = Convert.ToInt32(totaal);

            string amountString = totaalbedrag;
            decimal amountDecimal = decimal.Parse(amountString, System.Globalization.CultureInfo.InvariantCulture); // Ensure correct parsing
            long amount = (long)(amountDecimal * 100); // Convert to cents

            StripeConfiguration.ApiKey = "sk_test_51O80HeCj2LNHE2SjFEe0ytNYFS9ey3v1slTvIJwYG4gEGt2XYbhCtRpx3tgy1vKsHFT6oJCrOwXCCM2qkbsVGOTT00g85J4aw1";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    betaalmethode,
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Name = "Uw bestelling bij Goossens Meubels",
                        Description = "Meubels",
                        Amount = amount,
                        Currency = "eur",
                        Quantity = 1,
                    },
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "email", email },
                        { "bestelguid", bestelguid },
                        { "klantid", klantid },
                    }
                },
                SuccessUrl = "http://localhost:51820/Success.html?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:51820/Cancel.html",
            };

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            values values = new values();
            values.sessionId = session.Id;
            values.totaal = amount.ToString();
            return values;
            //return session.Id;

        }

    }
}
