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

				foreach (DataGridViewRow dgvr in SeleniumBot.MyDataGrid.Rows)
				{
					if ((string)dgvr.Cells[1].Value != "АКТИВНАЯ")
					{
						dgvr.Cells[1].Style.BackColor = Color.Brown;
					}

					if(TimeSpan.Parse((string) dgvr.Cells[6].Value).TotalMinutes < Properties.Settings.Default.StartParse)
					{
						dgvr.Cells[6].Style.BackColor = Color.Brown;
					}
				}
			}
            
		}
	}
}
