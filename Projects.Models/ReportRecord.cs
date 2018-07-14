using System;

namespace Projects.Models
{
    public class ReportRecord
    {
        public ReportRecord()
        {
            Level = 1;
            Duration = new TimeSpan();
        }

        public int Level { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public TimeSpan Duration { get; set; }
        public override string ToString()
        {
            return string.Format("Level={0,2}, Type={1,5}, Duration={2}", Level, Type, Duration);
        }
    }
}