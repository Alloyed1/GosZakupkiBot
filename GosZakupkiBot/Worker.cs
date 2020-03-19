using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quartz;

namespace GosZakupkiBot
{
    public class Worker : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (SeleniumBot.Items.Any())
            {
                foreach (var item in SeleniumBot.Items)
                {
                    if(item.Ostalos.TotalMinutes <= Properties.Settings.Default.StartParse)
                    {
                        if (SeleniumBot.Items.FirstOrDefault(f => f.Number == item.Number).MinPrice <= SeleniumBot.Items.FirstOrDefault(f => f.Number == item.Number).NextBet 
                                && SeleniumBot.Items.FirstOrDefault(f => f.Number == item.Number).MinPrice != 0)
                        {
                            _ = Task.Run(() => SeleniumBot.MakeBet(item));
                            await Task.Delay(500);
                        }
                        
                    }
                    
                }
            }
        }
    }
}