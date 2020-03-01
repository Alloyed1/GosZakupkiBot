using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Quartz;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GosZakupkiBot.Models;
using Newtonsoft.Json;
using Equin.ApplicationFramework;

namespace GosZakupkiBot
{
	public static class SeleniumBot

	{
		private static List<BrowsersModel> Browsers { get; set; } = new List<BrowsersModel>();
		public static List<Item> Items { get; set; } = new List<Item>();
		public static BindingListView<Item> view { get; set; }
		public static DataGridView MyDataGrid { get; set; }

		public static TextBox textBoxFirst { get; set; }
		public static TextBox textBoxSecond { get; set; }
		public static TextBox textBoxTimer { get; set; }

		public static Task InitializeBot(int count)
		{
			for (int i = 0; i < count; i++)
			{
				var browser = new ChromeDriver();
				Browsers.Add(new BrowsersModel()
				{
					Browser = browser
				});

				browser.Navigate().GoToUrl("https://zakupki.mos.ru/auction/710761");

			}


			return Task.CompletedTask;
		}

		public async static Task<Item> ParseLink(string link, bool isReturn, int delay)
		{
			var browser = Browsers.FirstOrDefault(f => f.IsFree)?.Browser;

			if (browser == null)
			{
				await Task.Delay(5000);
				_ = Task.Run(() => ParseLink(link, isReturn, 0));
				return null;
			}

			Browsers.FirstOrDefault(f => f.Browser == browser).IsFree = false;

			browser.Navigate().GoToUrl(link);
			await Task.Delay(3000);

			var item = new Item();
			item.Number = int.Parse(link.Replace("//", "").Split('/')[2]);

			var status = JsonConvert.DeserializeObject<ParseItem>(Properties.Settings.Default.StatusParse);
			var date = JsonConvert.DeserializeObject<ParseItem>(Properties.Settings.Default.DateParse);
			var time = JsonConvert.DeserializeObject<ParseItem>(Properties.Settings.Default.TimeParse);
			var price = JsonConvert.DeserializeObject<ParseItem>(Properties.Settings.Default.PriceParse);

			try
			{
				item.Status = browser
					.FindElement(By.ClassName(status.ClassName.Split(' ')[0]))
					.FindElement(By.TagName(status.Tag))
					.Text;
			}
			catch
			{
				_ = Task.Run(() => ParseLink(link, isReturn, 0));
				return null;
			}


			var datetimeText = browser
				.FindElements(By.TagName("label"))
				.FirstOrDefault(f => f.Text == date.Text)
				?.FindElement(By.XPath("./parent::*"))
				.FindElement(By.TagName(date.Tag))
				.Text;

			var datetime = Convert.ToDateTime(datetimeText.Split(new string[] {"по "}, StringSplitOptions.None)[1]);

			var dateTimeSpan = (datetime - DateTime.Now);
			item.Ostalos = new TimeSpan(dateTimeSpan.Days, dateTimeSpan.Hours, dateTimeSpan.Minutes,
				dateTimeSpan.Seconds);

			var actualPrice = browser
				.FindElements(By.TagName("label"))
				.FirstOrDefault(f => f.Text == price.Text)
				?.FindElement(By.XPath("./parent::*"))
				.FindElement(By.TagName(price.Tag))
				.Text;

			item.ActualPrice = float.Parse(actualPrice.Replace(" ₽", ""));
			Browsers.FirstOrDefault(f => f.Browser == browser).IsFree = true;
			if (isReturn)
			{
				return item;
			}
			
			Items.Add(item);
			view = new BindingListView<Item>(Items);
			MyDataGrid.DataSource = view;
			
			return null;

		}

		public static async Task ParseAndUpdate(int index)
		{
			var item = await ParseLink("https://zakupki.mos.ru/auction/" + index, true,
				0);

			Items.FirstOrDefault(f => f.Number == index).Ostalos = item.Ostalos;
			Items.FirstOrDefault(f => f.Number == index).Status = item.Status;
			Items.FirstOrDefault(f => f.Number == index).ActualPrice = item.ActualPrice;
			Items.FirstOrDefault(f => f.Number == index).NextBet = item.NextBet;
		}

		public static async Task ParseAll()
		{
			
			textBoxSecond.Invoke(new Action(() => { textBoxSecond.Text = (Items.Count).ToString(); }));
			
			int count = 0;
			while (Items.Count != count)
			{
				if (Browsers.FirstOrDefault(f => f.IsFree) == null)
				{
					await Task.Delay(200);
					continue;
				}
				
				textBoxFirst.Invoke(new Action(() => { textBoxFirst.Text = (count + 1).ToString(); }));
				
				var num = Items[count].Number;

				_ = Task.Run(() => ParseAndUpdate(num));
				await Task.Delay(700);

				count++;
			}

			await UpdateDataGrid();
		}




		public async static Task UpdateDataGrid()
		{
			MyDataGrid.Invoke(new Action(() =>
			{
				var view = new BindingListView<Item>(Items.OrderBy(b => b.Ostalos).ToList());
				MyDataGrid.DataSource = view;
			}));
		}
	}
}
