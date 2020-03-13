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

namespace GosZakupkiBot
{
	public partial class Form1 : Form

	{

		bool isStart = false;
		bool IsInitial = false;
		public Form1()
		{
			InitializeComponent();
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

			await LoadSettings();
			dataGridView1.AllowUserToAddRows = false;
			
			SeleniumBot.MyDataGrid = dataGridView1;
			await SeleniumBot.UpdateDataGrid();

			startStop_btn.BackColor = Color.Red;





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
	}
}
