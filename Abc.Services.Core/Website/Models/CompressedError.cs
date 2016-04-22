namespace Abc.Website.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class CompressedError
    {
        public string Class
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
        public int Count
        {
            get;
            set;
        }
        public IEnumerable<ErrorsPerTime> Occurrences
        {
            get;
            set;
        }
    }
    [Serializable]
    public class ErrorsPerTime
    {
        public int Count
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }
    }
}