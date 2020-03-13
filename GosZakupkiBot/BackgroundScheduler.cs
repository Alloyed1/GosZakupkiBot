using Quartz;
using Quartz.Impl;

namespace GosZakupkiBot
{
    public class BackgroundScheduler
    {
        public static IScheduler Scheduler { get; set; }

        public static async void Start()
        {
            Scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await Scheduler.Start();
 
            IJobDetail job = JobBuilder.Create<BackgroundTasks>().Build();
            IJobDetail job3 = JobBuilder.Create<ParserJob>().Build();
            IJobDetail job2 = JobBuilder.Create<UpdateDataGridAsync>().Build();
            IJobDetail job4 = JobBuilder.Create<Worker>().Build();
 
            ITrigger trigger = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger1", "group1")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInSeconds(1)         // через 1 минуту
                    .RepeatForever())                   // бесконечное повторение
                .Build();      
            
            ITrigger trigger2 = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger12", "group12")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInSeconds(20)         // через 1 минуту
                    .RepeatForever())                   // бесконечное повторение
                .Build();// создаем триггер
            
            ITrigger trigger3 = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger3", "group3")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInMinutes(Properties.Settings.Default.ParseAllTimeout)
                    .RepeatForever())                   // бесконечное повторение
                .Build();// создаем триггер
            
            ITrigger trigger4 = TriggerBuilder.Create()  // создаем триггер
                .WithIdentity("trigger3", "group4")     // идентифицируем триггер с именем и группой
                .StartNow()                            // запуск сразу после начала выполнения
                .WithSimpleSchedule(x => x            // настраиваем выполнение действия
                    .WithIntervalInMinutes(Properties.Settings.Default.ParseToEnd)
                    .RepeatForever())                   // бесконечное повторение
                .Build();// создаем триггер
 
            await Scheduler.ScheduleJob(job, trigger); 
            await Scheduler.ScheduleJob(job2, trigger2);
            await Scheduler.ScheduleJob(job3, trigger3);
            await Scheduler.ScheduleJob(job4, trigger4);
        }
    }
}