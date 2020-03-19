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
					Browser = browser,
					IsFree = true
				});

				browser.Navigate().GoToUrl("https://zakupki.mos.ru");

			}


			return Task.CompletedTask;
		}

		public async static Task MakeBet(Item item)
		{
			await Task.Delay(new Random().Next(7000, 30000));
			var browser = Browsers.FirstOrDefault(f => f.IsFree)?.Browser;

			if (browser == null)
			{
				await Task.Delay(7000);
				_ = Task.Run(() => MakeBet(item));
				return;
			}


			Browsers.FirstOrDefault(f => f.Browser == browser).IsFree = false;

			browser.Navigate().GoToUrl("https://zakupki.mos.ru/auction/" + item.Number);
			await Task.Delay(4000);
			bool isSet = false;
			try
			{
				var date = JsonConvert.DeserializeObject<ParseItem>(Properties.Settings.Default.DateParse);
				var datetimeText = browser
					.FindElements(By.TagName("label"))
					.FirstOrDefault(f => f.Text == date.Text)
					?.FindElement(By.XPath("./parent::*"))
					.FindElement(By.TagName(date.Tag))
					.Text;

				var datetime = Convert.ToDateTime(datetimeText.Split(new string[] { "по " }, StringSplitOptions.None)[1]);
				var dateTimeSpan = (datetime - DateTime.Now);
				Items.FirstOrDefault(f => f.Number == item.Number).Ostalos = (datetime - DateTime.Now);
				item.Ostalos = new TimeSpan(dateTimeSpan.Days, dateTimeSpan.Hours, dateTimeSpan.Minutes,
					dateTimeSpan.Seconds);

				var nextBet = browser
					.FindElements(By.TagName("span"))
					.FirstOrDefault(f => f.Text == "Возможная ставка")
					?.FindElement(By.XPath("./parent::*"))
					.FindElements(By.TagName("span"))[1]
					.Text;


				
				var num = float.Parse(nextBet.Replace(" ₽", ""));
				Items.FirstOrDefault(f => f.Number == item.Number).NextBet = num;
				
				
				if (num >= item.MinPrice)
				{
					isSet = true;
				}
				

			}
			catch
			{
				item.NextBet = 0;
			}

			if (isSet)
			{
				var makeBetBtn = browser.FindElements(By.TagName("button")).FirstOrDefault(f => f.Text == "Сделать ставку");
				if (makeBetBtn == null)
				{
					return;
				}
				makeBetBtn.Click();

				await Task.Delay(2400);

				browser.FindElements(By.TagName("button"))
					.FirstOrDefault(f => f.Text == "Принять условия")?.Click();
			
				await Task.Delay(1500);
			
				browser.FindElements(By.TagName("button")).Where(f => f.Text == "Сделать ставку").ToList()[1].Click();
			}

			
			
			Browsers.FirstOrDefault(f => f.Browser == browser).IsFree = true;

		}

		public async static Task GoToUrl(string link)
		{
			var browser = Browsers.FirstOrDefault(f => f.IsFree)?.Browser;

			if (browser == null)
			{
				MessageBox.Show("Свободных браузеров нет");
				return;
			}
			Browsers.FirstOrDefault(f => f.Browser == browser).IsFree = false;

			browser.Navigate().GoToUrl(link);

			Browsers.FirstOrDefault(f => f.Browser == browser).IsFree = true;

		}

		public async static Task<Item> ParseLink(string link, bool isReturn, int delay, bool isBet)
		{
			var browser = Browsers.FirstOrDefault(f => f.IsFree)?.Browser;

			if (browser == null)
			{
				await Task.Delay(7000);
				_ = Task.Run(() => ParseLink(link, isReturn, 0, isBet));
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
			catch(Exception ex)
			{
				Browsers.FirstOrDefault(f => f.Browser == browser).IsFree = true;
				_ = Task.Run(() => ParseLink(link, isReturn, 0, isBet));
				return null;
			}

			if(item.Status == "АКТИВНАЯ")
			{
				var datetimeText = browser
				.FindElements(By.TagName("label"))
				.FirstOrDefault(f => f.Text == date.Text)
				?.FindElement(By.XPath("./parent::*"))
				.FindElement(By.TagName(date.Tag))
				.Text;

				var datetime = Convert.ToDateTime(datetimeText.Split(new string[] { "по " }, StringSplitOptions.None)[1]);

				var dateTimeSpan = (datetime - DateTime.Now);
				item.Ostalos = new TimeSpan(dateTimeSpan.Days, dateTimeSpan.Hours, dateTimeSpan.Minutes,
					dateTimeSpan.Seconds);

				var actualPrice = browser
					.FindElements(By.TagName("label"))
					.FirstOrDefault(f => f.Text == price.Text)
					?.FindElement(By.XPath("./parent::*"))
					.FindElement(By.TagName(price.Tag))
					.Text;

				item.ActualPrice = float.Parse(actualPrice.Split('₽')[0].Replace(" ₽", ""));

				try
				{
					var nextBet = browser
						.FindElements(By.TagName("span"))
						.FirstOrDefault(f => f.Text == "Возможная ставка")
						?.FindElement(By.XPath("./parent::*"))
						.FindElements(By.TagName("span"))[1]
						.Text;

					item.NextBet = float.Parse(nextBet.Replace(" ₽", ""));
				}
				catch
				{
					item.NextBet = 0;
				}
			}
			
			
			Browsers.FirstOrDefault(f => f.Browser == browser).IsFree = true;
			if (isReturn)
			{
				return item;
			}
			
			Items.Add(item);
			view = new BindingListView<Item>(Items);
			MyDataGrid.DataSource = view;
			
			if(isBet) _ =Task.Run(() => MakeBet(item));
			
			return null;

		}

		public static async Task ParseAndUpdate(Item itemParse)
		{
			var item = await ParseLink("https://zakupki.mos.ru/auction/" + itemParse.Number, true,
				0, false);

			Items.FirstOrDefault(f => f.Number == itemParse.Number).Ostalos = item.Ostalos;
			Items.FirstOrDefault(f => f.Number == itemParse.Number).Status = item.Status;
			Items.FirstOrDefault(f => f.Number == itemParse.Number).ActualPrice = item.ActualPrice;
			Items.FirstOrDefault(f => f.Number == itemParse.Number).NextBet = item.NextBet;
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
				

				_ = Task.Run(() => ParseAndUpdate(Items[count]));
				await Task.Delay(700);

				count++;
			}

			Properties.Settings.Default.LastParse = new TimeSpan().Add(TimeSpan.FromMinutes(Properties.Settings.Default.ParseAllTimeout)).ToString();
			Properties.Settings.Default.Save();

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
