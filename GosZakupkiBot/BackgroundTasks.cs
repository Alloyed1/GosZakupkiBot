using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using Quartz;

namespace GosZakupkiBot
{
    public class BackgroundTasks : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (SeleniumBot.Items.Any())
            {
                foreach (var item in SeleniumBot.Items)
                {
                    var timeSpan = item.Ostalos.Add(TimeSpan.FromSeconds(-1));
                    item.Ostalos = timeSpan;

                }

            }
            var toNextParse = Properties.Settings.Default.LastParse;
            
            if(toNextParse == "")
            {
                SeleniumBot.textBoxTimer.Invoke(new Action(() =>
                {
                    SeleniumBot.textBoxTimer.Text = "Сейчас";
                }));
            }
            else
            {
                var timeSpans = TimeSpan.Parse(toNextParse).Add(TimeSpan.FromSeconds(-1));
                Properties.Settings.Default.LastParse = timeSpans.ToString();
                Properties.Settings.Default.Save();

                SeleniumBot.textBoxTimer.Invoke(new Action(() =>
                {
                    SeleniumBot.textBoxTimer.Text = $"{timeSpans.Minutes}:{timeSpans.Seconds}";
                }));

            }
            

            
        }
    }
}