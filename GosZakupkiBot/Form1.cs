using GosZakupkiBot.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using RestSharp;

namespace GosZakupkiBot
{
	public partial class Form1 : Form

	{

		bool isStart = false;
		bool IsInitial = false;
		public static string _host { get; set; } = "http://195.66.114.19";
		public Form1()
		{
			InitializeComponent();
			textBox10.Text = Properties.Settings.Default.Key;
		}
		public async Task SendItems()
		{
			while (true)
			{
				await Task.Delay(5000);

				var client = new RestClient(_host);
				var request = new RestRequest($"SetItems", Method.POST);
				request.AddHeader("Accept", "application/json");
				request.AddJsonBody(SeleniumBot.Items);

				await client.ExecuteAsync(request);

			}
		}
		public async Task AddUrl()
		{
			while (true)
			{
				await Task.Delay(1000);
				try
				{
					var client = new RestClient(_host);
					var request = new RestRequest($"GetUrl/{Properties.Settings.Default.Key}");
					var res = await client.ExecuteAsync(request);
					if (res.Content == "\"no\"" || res.Content == "") continue;

					try
					{
						await SeleniumBot.ParseLink(res.Content, false, 0, false);
					}
					catch
					{

					}
				}
				catch
				{

				}
				


			}
		}
		public async Task IsUpdateBot()
		{
			while (true)
			{
				await Task.Delay(1000);
				try
				{
					var client = new RestClient(_host);
					var request = new RestRequest($"GetIsUpdateBot/{Properties.Settings.Default.Key}");
					var res = await client.ExecuteAsync(request);

					try
					{
						var isOn = bool.Parse(res.Content);
						if (isOn) await SeleniumBot.ParseAll();
					}
					catch
					{

					}
				}
				catch
				{

				}
				
				
				
			}
		}
		public async Task DeleteItem()
		{
			while (true)
			{
				await Task.Delay(300);
				try
				{
					var client = new RestClient(_host);
					var request = new RestRequest($"GetDeleteItems");

					var content = client.Execute(request).Content;
					var num = int.Parse(content);
					if(num != 0)
					{
						SeleniumBot.Items.Remove(SeleniumBot.Items.FirstOrDefault(f => f.Number == num));
						await SeleniumBot.UpdateDataGrid();

						//Properties.Settings.Default.Items = JsonConvert.SerializeObject(SeleniumBot.Items);
						//Properties.Settings.Default.Save();
					}
					
				}
				catch(Exception ex)
				{

				}
			}
		}
		public async Task EditItem()
		{
			while (true)
			{
				await Task.Delay(200);
				try
				{
					var client = new RestClient(_host);
					var request = new RestRequest($"GetEditItem");

					var content = client.Execute(request).Content.Replace("\"", "");
					if (content == "") continue;

					var num = int.Parse(content.Split(',')[0]);
					var price = float.Parse(content.Split(',')[1]);
					var comment = content.Split(',')[2];
					var crm = content.Split(',')[2];

					SeleniumBot.Items.FirstOrDefault(f => f.Number == num).MinPrice = price;
					SeleniumBot.Items.FirstOrDefault(f => f.Number == num).Comment = comment;
					SeleniumBot.Items.FirstOrDefault(f => f.Number == num).CRMLink = crm;

					await SeleniumBot.UpdateDataGrid();


				}
				catch (Exception ex)
				{

				}
			}
		}
		public async Task StartOrStop()
		{
			while (true)
			{
				await Task.Delay(1000);
				try
				{
					var client = new RestClient(_host);
					var request = new RestRequest($"GetBotStatus/{Properties.Settings.Default.Key}");

					var res = await client.ExecuteAsync(request);

					var isOn = bool.Parse(res.Content);
					if (isOn)
					{
						if (!isStart)
						{
							if (!IsInitial)
							{
								BackgroundScheduler.Start();
								startStop_btn.BackColor = Color.Green;
								IsInitial = true;
							}
							else
							{
								await BackgroundScheduler.Scheduler.ResumeAll();
								startStop_btn.BackColor = Color.Green;
								isStart = true;
							}
						}
						
					}
					else if(isStart)
					{
						await BackgroundScheduler.Scheduler.PauseAll();
						startStop_btn.BackColor = Color.Red;
						isStart = false;
					}
				}
				catch
				{

				}
				

			}
		}

		public async Task LoadSettings()
		{
			numericUpDown3.Maximum = int.MaxValue;

			numericUpDown1.Value = Properties.Settings.Default.ParseToEnd;
			numericUpDown2.Value = Properties.Settings.Default.ParseAllTimeout;
			numericUpDown3.Value = Properties.Settings.Default.StartParse;

			var status = JsonConvert.DeserializeObject<ParseItem>(Properties.Settings.Default.StatusParse);
			var date = JsonConvert.DeserializeObject<ParseItem>(Properties.Settings.Default.DateParse);
			var price = JsonConvert.DeserializeObject<ParseItem>(Properties.Settings.Default.PriceParse);

			textBox1.Text = status.ClassName;
			textBox4.Text = status.Tag;
			textBox5.Text = status.Text;

			textBox8.Text = date.ClassName;
			textBox7.Text = date.Tag;
			textBox6.Text = date.Text;


			textBox14.Text = price.ClassName;
			textBox13.Text = price.Tag;
			textBox12.Text = price.Text;




		}


		private async void Form1_Load(object sender, EventArgs e)
		{
			panel1.AutoScroll = false;
			panel1.HorizontalScroll.Enabled = false;
			panel1.HorizontalScroll.Visible = false;
			panel1.HorizontalScroll.Maximum = 0;panel1.AutoScroll = true;

			var res = Properties.Settings.Default.Items;
			if (res != String.Empty)
			{
				SeleniumBot.Items = JsonConvert.DeserializeObject<List<Item>>(res);

			}

			SeleniumBot.textBoxFirst = textBox2;
			SeleniumBot.textBoxSecond = textBox3;
			SeleniumBot.textBoxTimer = proverka_textBox;
			try
			{
				await LoadSettings();
			}
			catch
			{

			}
			
			dataGridView1.AllowUserToAddRows = false;
			
			SeleniumBot.MyDataGrid = dataGridView1;
			await SeleniumBot.UpdateDataGrid();

			startStop_btn.BackColor = Color.Red;


			_= Task.Run(StartOrStop);
			_= Task.Run(IsUpdateBot);
			_= Task.Run(AddUrl);
			_= Task.Run(SendItems);
			_= Task.Run(DeleteItem);
			_ = Task.Run(EditItem);





		}

		private void tabPage1_Click(object sender, EventArgs e)
		{

		}

		private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void saveDelays_btn_Click(object sender, EventArgs e)
		{
			
		}

		private void saveDelays_btn_Click_1(object sender, EventArgs e)
		{
			Properties.Settings.Default.ParseToEnd = (int)numericUpDown1.Value;
			Properties.Settings.Default.ParseAllTimeout = (int)numericUpDown2.Value;
			Properties.Settings.Default.StartParse = (int)numericUpDown3.Value;

			Properties.Settings.Default.Save();
		}

		private void saveParseOpt_btn_Click(object sender, EventArgs e)
		{
			var status = new ParseItem() { Name = "Статус", ClassName = textBox1.Text, Tag = textBox4.Text, Text = textBox5.Text };
			var dates =  new ParseItem() { Name = "Даты проведения", ClassName = textBox8.Text, Tag = textBox7.Text, Text = textBox6.Text };
			var price =  new ParseItem() { Name = "Текущая цена", ClassName = textBox14.Text, Tag = textBox13.Text, Text = textBox12.Text };

			Properties.Settings.Default.StatusParse = JsonConvert.SerializeObject(status);
			Properties.Settings.Default.DateParse = JsonConvert.SerializeObject(dates);
			Properties.Settings.Default.PriceParse = JsonConvert.SerializeObject(price);
			
			Properties.Settings.Default.Save();
		}

		private async void button1_Click(object sender, EventArgs e)
		{
			
		}

		private async void button1_Click_1(object sender, EventArgs e)
		{
			await SeleniumBot.InitializeBot((int)numericUpDown4.Value);
		}

		private async void addUrl_btn_Click(object sender, EventArgs e)
		{
			await SeleniumBot.ParseLink(link_textBox.Text, false, 0, parseNow_checkBox.Checked);
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			
		}

		private void addUrlFromBuffer_btn_Click(object sender, EventArgs e)
		{
			link_textBox.Text = Clipboard.GetText();
		}

		private async void updateLots_btn_Click(object sender, EventArgs e)
		{
			await SeleniumBot.ParseAll();
		}

		private void button2_Click(object sender, EventArgs e)
		{

			var res = JsonConvert.SerializeObject(SeleniumBot.Items);
			Properties.Settings.Default.Items = res;
			Properties.Settings.Default.Save();
		}

		private async void startStop_btn_Click(object sender, EventArgs e)
		{
			if (!IsInitial)
			{
				BackgroundScheduler.Start();
				startStop_btn.BackColor = Color.Green;
				IsInitial = true;
			}
		    else if (isStart)
			{
				await BackgroundScheduler.Scheduler.PauseAll();
				startStop_btn.BackColor = Color.Red;
				isStart = false;
			}
			else
			{
				await BackgroundScheduler.Scheduler.ResumeAll();
				startStop_btn.BackColor = Color.Green;
				isStart = true;
			}
			
		}

		private void deleteLink_btn_Click(object sender, EventArgs e)
		{
			link_textBox.Text = String.Empty;
		}

		private async void goToUrl_btn_Click(object sender, EventArgs e)
		{
			_ = Task.Run(() => SeleniumBot.GoToUrl(link_textBox.Text));
		}

		private void dataGridView1_ControlRemoved(object sender, ControlEventArgs e)
		{
			
		}

		private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			
		}

		private async void button3_Click(object sender, EventArgs e)
		{
			SeleniumBot.Items.Remove(SeleniumBot.Items.FirstOrDefault(f => f.Number == int.Parse(textBox9.Text)));
			await SeleniumBot.UpdateDataGrid();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			var guid = Guid.NewGuid();
			Properties.Settings.Default.Key = guid.ToString();
			Properties.Settings.Default.Save();

			textBox10.Text = guid.ToString();
		}
	}
}
