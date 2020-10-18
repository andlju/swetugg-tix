﻿namespace Swetugg.Tix.Process.Funcs.Options
{
    public class ProcessOptions
    {
        public string AzureWebJobsStorage { get; set; }
        public string TixServiceBus { get; set; }
        public string ProcessEventsDbConnection { get; set; }
        public string ActivityCommandsQueue { get; set; }
        public string TicketCommandsQueue { get; set; }
    }
}