using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace GosZakupkiBot
{
	class ParserJob : IJob
	{
		public async Task Execute(IJobExecutionContext context)
		{
			await SeleniumBot.ParseAll();
		}
	}
}
