using System;
using System.Collections.Generic;

namespace SiriusTool.Helpers
{
    /// <inheritdoc />
    /// <summary>
    /// Класс исключений, связанных в работе с <see cref="T:SiriusTool.Helpers.TimetableFactory" />
    /// </summary>
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

        /// <summary>
        /// Добавляет в хранилище новый экземпляр <see cref="TimetableInfo"/> для указаной даты. Если уже существует, вызывает исключение
        /// </summary>
        /// <param name="date">Дата для <see cref="TimetableInfo"/></param>
        /// <param name="info">Экземпляр <see cref="TimetableInfo"/></param>
        /// <exception cref="TimetableFactoryException"></exception>
        public void Add(DateTime date, TimetableInfo info)
        {
            if (TimetableExists(date.Date))
                throw new TimetableFactoryException("Timetable already exists for this date", date);

            _timetableStore[date.Date] = info;
            _storeUpdates[date.Date] = DateTime.UtcNow;
        }

        /// <summary>
        /// Добавляет в хранилище новый экземпляр <see cref="TimetableInfo"/> для указаной даты. Если уже существует, перезаписывает
        /// </summary>
        /// <param name="date">Дата для <see cref="TimetableInfo"/></param>
        /// <param name="info">Экземпляр <see cref="TimetableInfo"/></param>
        public void ForceAdd(DateTime date, TimetableInfo info)
        {
            _timetableStore[date.Date] = info;
            _storeUpdates[date.Date] = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет данные о <see cref="TimetableInfo"/> для указаной даты. Если не существует, вызывает исключение
        /// </summary>
        /// <param name="date">Дата для <see cref="TimetableInfo"/></param>
        /// <param name="newInfo">Экземпляр <see cref="TimetableInfo"/></param>
        /// <exception cref="TimetableFactoryException"></exception>
        public void Update(DateTime date, TimetableInfo newInfo)
        {
            if (!TimetableExists(date.Date))
                throw new TimetableFactoryException("Timetable for this date does not exist", date);

            _timetableStore[date.Date] = newInfo;
            _storeUpdates[date.Date] = DateTime.UtcNow;
        }

        /// <summary>
        /// Удаляет запись о существующем экземпляре <see cref="TimetableInfo"/> для указанной даты. Если не существует, вызывает исключение
        /// </summary>
        /// <param name="date">Дата для <see cref="TimetableInfo"/></param>
        public void Remove(DateTime date)
        {
            if (!TimetableExists(date.Date))
                throw new TimetableFactoryException("Timetable for this date does not exist", date);

            _timetableStore.Remove(date.Date);
            _storeUpdates.Remove(date.Date);
        }

        /// <summary>
        /// Возвращает хранящийся экземпляр <see cref="TimetableInfo"/> для указанной даты. Если не существует, вызывает исключение
        /// </summary>
        /// <param name="date">Дата для <see cref="TimetableInfo"/></param>
        /// <exception cref="TimetableFactoryException"></exception>
        /// <returns></returns>
        public TimetableInfo GetInfo(DateTime date)
        {
            if (!TimetableExists(date.Date))
                throw new TimetableFactoryException("Timetable for this date does not exist", date);

            return _timetableStore[date.Date];
        }

        /// <summary>
        /// Возвращает точную дату добавления экземпляра <see cref="TimetableInfo"/> в хранилище. Если такого не существует, вызывает исключение
        /// </summary>
        /// <param name="date">Дата для <see cref="TimetableInfo"/></param>
        /// <exception cref="TimetableFactoryException"></exception>
        /// <returns></returns>
        public DateTime GetCreationTime(DateTime date)
        {
            if (!TimetableExists(date.Date))
                throw new TimetableFactoryException("Timetable for this date does not exist", date);

            return _storeUpdates[date.Date];
        }


        /// <summary>
        /// Полностью очищает хранилище и сведения о всех записях
        /// </summary>
        public void Clear()
        {
            _timetableStore.Clear();
            _storeUpdates.Clear();
        }

        /// <summary>
        /// Возвращает true, если для заданной даты существует записанное значение экземпляра <see cref="TimetableInfo"/>
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool TimetableExists(DateTime date)
        {
            return _timetableStore.ContainsKey(date.Date);
        }
    }
}