using Icaza;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;

namespace Ostium
{
    public partial class DeserializeJson_Frm : Form
    {
        readonly IcazaClass senderror = new IcazaClass();

        public DeserializeJson_Frm()
        {
            InitializeComponent();

            DomainURL_Txt.Text = new Uri(@Class_Var.URL_URI).Host;
            Output_Data.Font = new Font("Consolas", 10);
        }

        async void Fetch_Btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DomainURL_Txt.Text))
            {
                MessageBox.Show("Insert Domain name first!", "Empty Domain Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string[] urls = {
                "https://dns.google/resolve?name=" + DomainURL_Txt.Text + "&type=TXT",
                "https://dns.google/resolve?name=" + DomainURL_Txt.Text + "&type=NS",
                "https://dns.google/resolve?name=" + DomainURL_Txt.Text + "&type=MX",
                "https://dns.google/resolve?name=" + DomainURL_Txt.Text + "&type=A"
            };

            using (HttpClient client = new HttpClient())
            {
                foreach (string url in urls)
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        JObject json = JObject.Parse(jsonContent);

                        AppendDnsResponse(Output_Data, "Result of : " + url, json);
                    }
                    catch (Exception ex)
                    {
                        Output_Data.SelectionColor = Color.Red;
                        Output_Data.AppendText("Error while retrieving " + url + " : " + ex.Message + "\n\n");
                    }
                }
            }
        }

        void AppendDnsResponse(RichTextBox rtb, string header, JObject json)
        {
            rtb.SelectionColor = Color.Yellow;
            rtb.SelectionFont = new Font("Consolas", 10, FontStyle.Bold);
            rtb.AppendText(header + "\n");

            rtb.SelectionColor = Color.Lime;
            rtb.SelectionFont = new Font("Consolas", 10, FontStyle.Regular);

            rtb.AppendText("Status : " + json["Status"] + "\n");

            if (json["Question"] is JArray questions && questions.Count > 0)
            {
                rtb.SelectionColor = Color.Purple;
                rtb.AppendText("Question(s) :\n");
                rtb.SelectionColor = Color.Orange;
                foreach (var question in questions)
                {
                    rtb.AppendText("  - Name : " + question["name"] + ", Type : " + question["type"] + "\n");
                }
            }

            if (json["Answer"] is JArray answers && answers.Count > 0)
            {
                rtb.SelectionColor = Color.Green;
                rtb.AppendText("Response(s) :\n");
                rtb.SelectionColor = Color.Orange;
                foreach (var answer in answers)
                {
                    rtb.AppendText("  - Name : " + answer["name"] +
                                   ", Type : " + answer["type"] +
                                   ", TTL : " + answer["TTL"] +
                                   ", Data : " + answer["data"] + "\n");
                }
            }
            else
            {
                rtb.SelectionColor = Color.Red;
                rtb.AppendText("No answer found.\n");
            }

            rtb.SelectionColor = Color.White;
            rtb.AppendText("\n");
        }

        void ExportData_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Output_Data.Text))
                {
                    Stream isData;
                    SaveFileDialog saveFD = new SaveFileDialog
                    {
                        InitialDirectory = Application.StartupPath,
                        Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                        FilterIndex = 2,
                        RestoreDirectory = true
                    };

                    if (saveFD.ShowDialog() == DialogResult.OK)
                    {
                        if ((isData = saveFD.OpenFile()) != null)
                        {
                            using (StreamWriter SW = new StreamWriter(isData))
                            {
                                SW.Write(Output_Data.Text);
                            }

                            isData.Close();

                            Console.Beep(1200, 200);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                senderror.ErrorLog("Error! : ", ex.ToString(), "DeserializeJson_Frm", Application.StartupPath);
            }
        }
    }
}
