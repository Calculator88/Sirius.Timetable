using System;
using System.Collections.Generic;

namespace SiriusTool.Helpers
{
    public class TimetableFactoryException : Exception
    {
        public TimetableFactoryException(string message, DateTime date) : base(message)
        {
            Date = date;
        }
        public DateTime Date { get; }
    }

    public class TimetableFactory
    {
        private readonly Dictionary<DateTime, TimetableInfo> _timetableStore;

        private readonly Dictionary<DateTime, DateTime> _storeUpdates;

        public TimetableFactory()
        {
            _timetableStore = new Dictionary<DateTime, TimetableInfo>();
            _storeUpdates = new Dictionary<DateTime, DateTime>();
        }

        public void Add(DateTime date, TimetableInfo info)
        {
            if (TimetableExists(date))
                throw new TimetableFactoryException("Timetable already exists for this date", date);

            _timetableStore[date] = info;
            _storeUpdates[date] = DateTime.UtcNow;
        }

        public void ForceAdd(DateTime date, TimetableInfo info)
        {
            _timetableStore[date] = info;
            _storeUpdates[date] = DateTime.UtcNow;
        }

        public void Update(DateTime date, TimetableInfo info)
        {
            if (!TimetableExists(date))
                throw new TimetableFactoryException("Timetable for this date does not exist", date);

            _timetableStore[date] = info;
            _storeUpdates[date] = DateTime.UtcNow;
        }

        public void Remove(DateTime date)
        {
            if (!TimetableExists(date))
                throw new TimetableFactoryException("Timetable for this date does not exist", date);

            _timetableStore.Remove(date);
            _storeUpdates.Remove(date);
        }

        public TimetableInfo GetInfo(DateTime date)
        {
            if (!TimetableExists(date))
                throw new TimetableFactoryException("Timetable for this date does not exist", date);

            return _timetableStore[date];
        }

        public DateTime GetCreationTime(DateTime date)
        {
            if (!TimetableExists(date))
                throw new TimetableFactoryException("Timetable for this date does not exist", date);

            return _storeUpdates[date];
        }

        public void Clear()
        {
            _timetableStore.Clear();
            _storeUpdates.Clear();
        }

        public bool TimetableExists(DateTime date)
        {
            return _timetableStore.ContainsKey(date);
        }
    }
}