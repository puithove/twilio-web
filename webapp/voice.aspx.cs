
//Configure as Voice URL in Twilio account to return TwiML repsponse

using System;
using System.Xml;

namespace webapp
{
    public partial class voice : System.Web.UI.Page
    {
        protected void Page_LoadComplete(object sender, EventArgs e)
        {

            // GET request uses Request.QueryString["parameter1"]
            // POST request uses Request.Form["paramName"];


            if (Request.HttpMethod.ToString() == "POST")
            {


                Response.Clear(); 
                Response.ContentType = "text/xml"; //Must be 'text/xml'
                Response.ContentEncoding = System.Text.Encoding.UTF8; 
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Response><Say voice=\"alice\" language=\"en-US\">This number does not accept voice calls at this time</Say><Pause length=\"1\"/><Say voice=\"alice\" language=\"en - US\">Text messages to this number will be delivered as e-mail</Say></Response>");
                doc.Save(Response.Output);
                Response.End(); 

            }

        }
    }
}