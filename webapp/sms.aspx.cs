
//Configure as SMS URL in Twilio account to return TwiML repsponse

using System;
using System.Net.Mail;
using System.Xml;


namespace webapp
{
    public partial class sms : System.Web.UI.Page
    {
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            string from,body,to,messageSid;

            // GET request uses Request.QueryString["parameter1"]
            // POST request uses Request.Form["paramName"];


            if (Request.HttpMethod.ToString() == "POST")
            {

                from = Request.Form["From"];
                to = Request.Form["To"];
                body = Request.Form["Body"];
                messageSid = Request.Form["MessageSid"];

                Response.Clear(); 
                Response.ContentType = "text/xml"; //Must be 'text/xml'
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                XmlDocument doc = new XmlDocument();

                try
                {
                    String userName = "svcacct@domain.com"; //Should be moved to config file
                    String password = "password"; //Should be moved to config file
                    MailMessage msg = new MailMessage();
                    msg.To.Add(new MailAddress("from@domain.com")); //Should be moved to config file
                    msg.From = new MailAddress("to@domain.com"); //Should be moved to config file
                    msg.Subject = "SMS Reply to: " + to + " - From: " + from + " - MessageSid: " + messageSid;
                    msg.Body = body;
                    msg.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();
                    client.Host = "smtp.office365.com";
                    client.Credentials = new System.Net.NetworkCredential(userName, password);
                    client.Port = 587;
                    client.EnableSsl = true;
                    client.Send(msg);

                    doc.LoadXml("<Response><Message>Message was forwarded successfully</Message></Response>");
                }
                catch
                {
                    doc.LoadXml("<Response><Message>Message forward failed</Message></Response>");
                }

                doc.Save(Response.Output);
                Response.End(); 

            }

        }
    }
}