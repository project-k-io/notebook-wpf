using System;
using Microsoft.Extensions.Logging;

namespace ProjectK.View.Helpers.ViewModels
{
    public class OutputRecordViewModel
    {
        private const string Format = "{0}\t{1}\t{2}\t{3}\t{4}";

        public int ID { get; set; }

        public LogLevel Type { get; set; }

        public DateTime Date { get; set; }

        public string State { get; set; }

        public string Message { get; set; }

        public static string Header =>
            string.Format("{0}\t{1}\t{2}\t{3}\t{4}", "ID", "Type", "Date", "State", "Message");

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", ID, Type, Date, State, Message);
        }
    }
}