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
            
        }
    }
}