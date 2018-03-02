using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using SiriusTool.Model;

namespace SiriusTool.Framework
{
    public delegate void TimetableDownloadCompletedEventHandler(object sender, DownloadStringCompletedEventArgs args);

    public delegate void TimetableParsingCompletedEventHandler(ParsingCompletedEventArgs args);

    public class ParsingCompletedEventArgs : AsyncCompletedEventArgs
    {
        public ParsingCompletedEventArgs(Exception error, bool cancelled, object userState, Dictionary<string, List<Event>> parsed) : base(error, cancelled, userState)
        {
            ParsedTimetable = parsed;
        }

        /// <summary>
        /// Teamname - event-list dictionary for every team
        /// </summary>
        public Dictionary<string, List<Event>> ParsedTimetable { get; }
    }

    public class TimetableParsingException : Exception
    {
        public TimetableParsingException() : base("An error occured while parsing json from schedule server")
        {
        }
    }
}