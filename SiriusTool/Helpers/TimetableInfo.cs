using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using SiriusTool.Model;

namespace SiriusTool.Helpers
{
    /// <summary>
    /// Предоставляет разделенные данные расписания для всех команд на определенную дату
    /// </summary>
    public sealed class TimetableInfo
    {
        /// <summary>
        /// </summary>
        /// <param name="timetable">Массив данных с расписаниями для каждой команды</param>
        /// <param name="date">Дата</param>
        public TimetableInfo(IDictionary<string, List<Event>> timetable, DateTime date)
        {
            _regx = new Regex(@"(\b(?<team>\D+)(?<digit>\d+)\b)|(?<name>\b\w+[\w\s\W]*$)", RegexOptions.Compiled);
            Timetable = new ReadOnlyDictionary<string, List<Event>>(timetable);
            Date = date;
            UpdateData();
        }

        /// <summary>
        /// Дата для всех расписаний
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Словарь с расписаниями для каждой команды FullTeamName/Timetable
        /// </summary>
        public ReadOnlyDictionary<string, List<Event>> Timetable { get; set; }

        /// <summary>
        /// Для каждой группы команд (наука, спорт и т.д.) задает список возможных номеров команд
        /// </summary>
        public ReadOnlyDictionary<string, List<int>> DirectionPossibleNumbers { get; set; }

        /// <summary>
        /// Сопоставляет короткий псевдоним команды полному имени
        /// </summary>
        public ReadOnlyDictionary<string, string> ShortLongTeamNameDictionary { get; set; }

        /// <summary>
        /// Возвращает все команды без номеров
        /// </summary>
        public ReadOnlyCollection<string> UnknownPossibleTeams { get; set; }

        /// <summary>
        /// На осонове заданой Timetable обновляет все записи словарей
        /// </summary>
        private void UpdateData()
        {
            var shortLongTeamNameDictionary = new Dictionary<string, string>();
            var directionPossibleNumbers = new Dictionary<string, List<int>>();
            var unknownPossibleTeams = new List<string>();

            foreach (var timetableNode in Timetable)
            {
                timetableNode.Value.Sort(ComparingEvents);                                      

                var matches = _regx.Matches(timetableNode.Key);
                if (matches.Count == 0)
                    throw new Exception("No matches found in teamNode");                                                 
                                                                                                
                if (matches.Count == 1)
                {                                                                               
                    var name = matches[0].Groups["name"].Value;                                                                                                    

                    if (String.IsNullOrWhiteSpace(name))
                    {
                        var direction = matches[0].Groups["team"].Value;                            
                        var num = int.Parse(matches[0].Groups["digit"].Value);                      
                        var teamName = direction + num.ToString("00");
                        shortLongTeamNameDictionary[teamName] = teamName;
                        if (!directionPossibleNumbers.ContainsKey(direction))
                            directionPossibleNumbers[direction] = new List<int>();
                        directionPossibleNumbers[direction].Add(num);
                    }
                    else
                    {
                        unknownPossibleTeams.Add(name);
                        shortLongTeamNameDictionary[name] = name;
                    }
                }
                else
                {
                    var tname = "";
                    var teams = new List<(string Direction, int Number)>();
                    for (var i = 0; i < matches.Count; ++i)
                    {
                        var match = matches[i].Groups;

                        var direction = match["team"].Value;
                        var number = int.Parse(match["digit"].Value);
                        var name = match["name"].Value;
                        
                        if(!String.IsNullOrWhiteSpace(name) && !String.IsNullOrWhiteSpace(tname))
                            throw new Exception("Team can have only one name");
                        tname = name;
                        teams.Add((direction, number));
                    }
                    foreach (var (direction, number) in teams)
                    {
                        if (!directionPossibleNumbers.ContainsKey(direction))
                            directionPossibleNumbers[direction] = new List<int>();
                        directionPossibleNumbers[direction].Add(number);

                        var fullTeam = direction + ' ' + number.ToString("00");
                        shortLongTeamNameDictionary[fullTeam] = fullTeam + ' ' + tname;
                    }
                }
            }
            foreach (var dict in directionPossibleNumbers) dict.Value.Sort();

            ShortLongTeamNameDictionary = new ReadOnlyDictionary<string, string>(shortLongTeamNameDictionary);
            DirectionPossibleNumbers = new ReadOnlyDictionary<string, List<int>>(directionPossibleNumbers);
            UnknownPossibleTeams = new ReadOnlyCollection<string>(unknownPossibleTeams);
        }

        /// <summary>
        /// Используется для упорядочивания событий в расписании по времени начала
        /// </summary>
        /// <param name="act1"></param>
        /// <param name="act2"></param>
        /// <returns></returns>
        private static int ComparingEvents(Event act1, Event act2)
        {
            if (act1.Start == act2.Start)
            {
                if (act1.End == act2.End) return 0;
                if (act1.End < act2.End) return -1;
                return 1;
            }
            if (act1.Start < act2.Start) return -1;
            return 1;
        }

        private readonly Regex _regx;
    }
}