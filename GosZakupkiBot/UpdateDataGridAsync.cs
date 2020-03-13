using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Quartz;

namespace GosZakupkiBot
{
	public class UpdateDataGridAsync : IJob
	{
		public async Task Execute(IJobExecutionContext context)
		{
			if (SeleniumBot.Items.Any())
			{
				await SeleniumBot.UpdateDataGrid();
			}
            
		}
	}
}
