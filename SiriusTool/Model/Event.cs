using System;
using System.Collections.Generic;

namespace SiriusTool.Model
{
    /// <summary>
    /// Contains useful data for one event
    /// </summary>
    public class Event
    { 
        /// <summary>
        /// Title of the event
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Time, when event starts
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// Time, when event ends
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Places of the event
        /// </summary>
        public List<PlaceNode> Rooms { get; set; }
    }
}