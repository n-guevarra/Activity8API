using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;


namespace Act8API
{
    public partial class Form1 : Form
    {
        private readonly string apiUrl = "http://localhost/myapi/act8api.php";
        private string selectedDepartment = "";

        public Form1()
        {
            InitializeComponent();
            
        }

        private async void cardioBtn_CheckedChanged(object sender, EventArgs e)
        {
            selectedDepartment = "Cardiology";
            await FetchDoctorAndAvailableDate();
        }

        private async void hemeBtn_CheckedChanged(object sender, EventArgs e)
        {
            selectedDepartment = "Hematology";
            await FetchDoctorAndAvailableDate();
        }

        private async void pulmoBtn_CheckedChanged(object sender, EventArgs e)
        {
            selectedDepartment = "Pulmonology";
            await FetchDoctorAndAvailableDate();
        }

        private async Task FetchDoctorAndAvailableDate()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}?department={selectedDepartment}");

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(json);

                    if (data["error"] == null)
                    {
                        doctortb.Text = data["doctor"].ToString(); 
                        datetb.Text = data["available_date"].ToString(); 
                        await FetchDoctorAndAvailableDate();
;                       
                    }
                    else
                    {
                        MessageBox.Show(data["error"].ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Failed to retrieve data from the server. Status code: " + response.StatusCode);
                }
            }
        }



        private async void getBtn_Click(object sender, EventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(json);
                    gunaDataGridView1.DataSource = data.ToObject<DataTable>();
                }
                else
                {
                    MessageBox.Show("Failed to retrieve data from the server. Status code: " + response.StatusCode);
                }
            }
        }


        private async void postBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedDepartment))
            {
                MessageBox.Show("Please select a department.");
                return;
            }

            var patientData = new
            {
                first_name = fnametb.Text,
                last_name = lnametb.Text,
                department = selectedDepartment,
                doctor = doctortb.Text,
                available_date = datetb.Text
            };

            string json = JsonConvert.SerializeObject(patientData);

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(responseBody);
                }
                else
                {
                    MessageBox.Show("Failed to post data to the server. Status code: " + response.StatusCode);
                }
            }
        }
 
    }
}
