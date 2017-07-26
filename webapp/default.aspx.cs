
//Default app page for sending text. Configure account and list info in datafiles

using System;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;

namespace webapp
{


    public partial class _default : System.Web.UI.Page
    {
        string accountSid;
        string authToken;
        string fromNum;
        DataTable accounts;
        DataTable distroLists;
        DataTable contacts;

        public void resetStatus()
        {
            //Clear status labels
            messageStatus.Text = "";
            messageError.Text = "";

            //Clear output grid
            outputGrid.DataBind();
            resultsLabel.Visible = false;
        }

        public static DataTable CsvDb(string filename, string separatorChar)
        {
            var table = new DataTable("Filecsv");
            using (var sr = new StreamReader(filename))
            {
                string line;
                var i = 0;
                while (sr.Peek() >= 0)
                {
                    try
                    {
                        line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;
                        var values = line.Split(new[] { separatorChar }, StringSplitOptions.None);
                        var row = table.NewRow();
                        for (var colNum = 0; colNum < values.Length; colNum++)
                        {
                            var value = values[colNum];
                            if (i == 0)
                            {
                                table.Columns.Add(value, typeof(String));
                            }
                            else
                            { row[table.Columns[colNum]] = value; }
                        }
                        if (i != 0) table.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {
                        string cErr = ex.Message;
                    }
                    i++;
                }
            }
            return table;
        }

        public static DataTable GetListBoxItems(ListBox listBox)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Value");
            dt.Columns.Add("Text");
            dt.Columns.Add("Selected");
            dt.Columns.Add("Attributes");


            foreach (ListItem itm in listBox.Items)
            {
                DataRow dr = dt.NewRow();
                dr[0] = itm.Value;
                dr[1] = itm.Text;
                dr[2] = itm.Selected;
                dr[3] = itm.Attributes;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                //initialize datatables and convert columnnames to uppercase
                accounts = CsvDb((Request.PhysicalApplicationPath + "dataFiles/accounts.csv"), ",");
                foreach (var columnName in accounts.Columns)
                {
                    accounts.Columns[columnName.ToString()].ColumnName = columnName.ToString().ToUpper();
                }
                distroLists = CsvDb((Request.PhysicalApplicationPath + "dataFiles/distroLists.csv"), ",");
                foreach (var columnName in distroLists.Columns)
                {
                    distroLists.Columns[columnName.ToString()].ColumnName = columnName.ToString().ToUpper();
                }
                contacts = CsvDb((Request.PhysicalApplicationPath + "dataFiles/contacts.csv"), ",");
                foreach (var columnName in contacts.Columns)
                {
                    contacts.Columns[columnName.ToString()].ColumnName = columnName.ToString().ToUpper();
                }

                //don't clear controls if this is a postback
                if (!IsPostBack)
                {
                    resetStatus();
                    fromDropDown.DataSource = accounts;
                    fromDropDown.DataTextField = "ACCOUNT";
                    fromDropDown.DataBind();
                }
            }
            catch (Exception err)
            {
                messageStatus.Text = "Error";
                messageError.Text = err.Message;
            }

        }


        protected void sendEmail_Click(object sender, EventArgs e)
        {

            resetStatus();

            string url;

            url = "http://" + HttpContext.Current.Request.Url.Authority + "/sms.aspx";

            try
            {
                using (WebClient client = new WebClient())
                {

                    byte[] response =
                    client.UploadValues(url, new NameValueCollection()
                    {
                       { "From", "E-Mail Test Button" },
                       { "To", "E-mail Tester" },
                       { "Body", "This is a test message from the webpage" },
                       { "MessageSid", "FAKESIDXXXX" }
                    });
                    messageStatus.Text = "Success";
                    messageError.Text = System.Text.Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception err)
            {
                messageStatus.Text = "Error";
                messageError.Text = err.Message;

            }
            
        }


        protected void sendText_Click(object sender, EventArgs e)
        {
            resetStatus();

            accountSid = accounts.Rows[fromDropDown.SelectedIndex]["ACCOUNTSID"].ToString();
            authToken = accounts.Rows[fromDropDown.SelectedIndex]["AUTHTOKEN"].ToString();
            fromNum = accounts.Rows[fromDropDown.SelectedIndex]["FROMNUM"].ToString();
            DataTable distroList = new DataTable();
            DataTable tmpList = new DataTable();

            if ((accountSid == "") || (authToken == "") || (fromNum == "") || (msgBody.Text == ""))
            {
                messageStatus.Text = "Missing Info";
                messageError.Text = "Please make selections";
            }
            else
            {
                distroList = new DataView(GetListBoxItems(contactsBox), "Selected = True", "Text", DataViewRowState.CurrentRows).ToTable();

                if (distroList.Rows.Count > 0)
                {
                    distroList.Columns.Add("Result Status");
                    distroList.Columns.Add("Result Message");
                    distroList.Columns.Remove("Selected");
                    distroList.Columns.Remove("Attributes");
                    distroList.Columns["Text"].ColumnName = "Name";
                    distroList.Columns["Value"].ColumnName = "Number";

                    try
                    {
                        TwilioClient.Init(accountSid, authToken);
                    }
                    catch (Exception err)
                    {
                        messageStatus.Text = "Error";
                        messageError.Text = err.Message;
                    }

                    foreach (DataRow contact in distroList.Rows)
                    {
                        try
                        {
                            var message = MessageResource.Create(
                                to: new PhoneNumber(contact["NUMBER"].ToString()),
                                from: new PhoneNumber(fromNum),
                                body: msgBody.Text);
                            contact["RESULT STATUS"] = "Success";
                        }
                        catch (Exception err)
                        {
                            contact["RESULT STATUS"] = "Error";
                            contact["RESULT MESSAGE"] = err.Message;
                        }
                    }

                    resultsLabel.Visible = true;
                    outputGrid.DataSource = distroList;
                    outputGrid.DataBind();
                }
                else
                {
                    messageStatus.Text = "Missing Info";
                    messageError.Text = "Please make selections";
                }
            }
            
        }

        protected void msgBody_TextChanged(object sender, EventArgs e)
        {
            resetStatus();
        }

        protected void fromDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetStatus();

            toListBox.DataSource = new DataView(distroLists, "account = '" + fromDropDown.Text + "'", "distroName", DataViewRowState.CurrentRows);
            toListBox.DataTextField = "distroName";
            toListBox.DataValueField = "distroID";
            toListBox.DataBind();

            contactsBox.DataSource = "";
            contactsBox.DataBind();
            contactsBox.Items.Clear();

        }

        protected void toListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetStatus();

            string condition = "";

            foreach (ListItem distroItem in toListBox.Items)
            {
                if (distroItem.Selected)
                {
                    if (condition == "")
                    {
                        condition = "'" + distroItem.Value + "'";
                    }
                    else
                    {
                        condition = condition + ",'" + distroItem.Value + "'";
                    }
                }
            }

            if (condition == "")
            {
                contactsBox.DataSource = "";
                contactsBox.DataBind();
                contactsBox.Items.Clear();
            }
            else
            {
                contactsBox.DataSource = new DataView(contacts, "distroID IN (" + condition + ")", "name", DataViewRowState.CurrentRows).ToTable(true, "name", "number");
                contactsBox.DataTextField = "name";
                contactsBox.DataValueField = "number";
                contactsBox.DataBind();
            }

            foreach (ListItem contactItem in contactsBox.Items)
            {
                contactItem.Selected = true;
            }
        }

        protected void outputGrid_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void resetButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.AbsoluteUri);
        }

    }
}