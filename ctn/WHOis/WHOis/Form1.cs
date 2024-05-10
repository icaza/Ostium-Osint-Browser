using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using DnsClient;
using System.Net;


namespace WHOis
{
    public partial class Form1 : Form
    {
        TcpClient tcpWhois;
        NetworkStream nsWhois;
        BufferedStream bfWhois;
        StreamWriter strmSend;
        StreamReader strmRecive;

        public Form1()
        {
            InitializeComponent();
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbServer.SelectedIndex = 0;
        }

        private void btnLookUp_Click(object sender, EventArgs e)
        {
            if (txtHostName.Text == null)
            {
                label6.Visible=true;
            }
            try
            {
                //CONNECT TO TCP CLIENT OF WHOIS
                tcpWhois = new TcpClient(cmbServer.SelectedItem.ToString(), 43);

                //SETUP THE NETWORK STREAM
                nsWhois = tcpWhois.GetStream();

                //GET THE DATA IN THE BUFFER FROM THE NETWORK STREAM
                bfWhois = new BufferedStream(nsWhois);

                strmSend = new StreamWriter(bfWhois);

                strmSend.WriteLine(txtHostName.Text);

                strmSend.Flush();

                txtxResponse.Clear();


                try
                {
                    strmRecive = new StreamReader(bfWhois);
                    string response;

                    while ((response = strmRecive.ReadLine()) != null)
                    {
                        txtxResponse.Text += response + "\r\n";
                        if (progressBar1.Value < 100)
                            progressBar1.Value += 10;
                    }
                }

                catch
                {
                    MessageBox.Show("WHOis Server Error :x");
                }

            }

            catch
            {
                MessageBox.Show("No Internet Connection or Any other Fault", "Error");
            }

            //SEND THE WHO_IS SERVER ABOUT THE HOSTNAME

            finally
            {
                try
                {
                    tcpWhois.Close();
                }
                catch
                {
                }
            }

        }

        private void txtxResponse_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void txtxResponse_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var lookup = new LookupClient();
            //var result = await lookup.QueryAsync("google.com", QueryType.A);

            //var record = result.Answers.ARecords().FirstOrDefault();
            //var ip = record?.Address;

            //MessageBox.Show(Convert.ToString(ip));

            //0

            //IPHostEntry hostInfo = Dns.GetHostEntry("www.contoso.com");

            //MessageBox.Show(Convert.ToString(hostInfo));

            //0

            //IPAddress[] addresses = Dns.GetHostAddresses("google.com");

            //Console.WriteLine($"GetHostAddresses({"google.com"}) returns:");

            //foreach (IPAddress address in addresses)
            //{
            //    Console.WriteLine($"    {address}");
            //}


        }
    }
}
