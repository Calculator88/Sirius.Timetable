using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using SiriusTool.Model;

namespace SiriusTool.Helpers
{
    public sealed class TimetableInfo
    {
        public TimetableInfo(IDictionary<string, List<Event>> timetable, DateTime date)
        {
            _regx = new Regex(@"(\b(?<team>\D+)(?<digit>\d+)\b)|(?<name>\b\w+[\w\s\W]*$)");
            Timetable = new ReadOnlyDictionary<string, List<Event>>(timetable);
            Date = date;
            UpdateData();
        }

        public DateTime Date { get; set; }

        public ReadOnlyDictionary<string, List<Event>> Timetable { get; set; }

        public ReadOnlyDictionary<string, List<int>> DirectionPossibleNumbers { get; set; }

        public ReadOnlyDictionary<string, string> ShortLongTeamNameDictionary { get; set; }

        public ReadOnlyCollection<string> UnknownPossibleTeams { get; set; }

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
                    foreach (var team in teams)
                    {
                        if (!directionPossibleNumbers.ContainsKey(team.Direction))
                            directionPossibleNumbers[team.Direction] = new List<int>();
                        directionPossibleNumbers[team.Direction].Add(team.Number);

                        var fullTeam = team.Direction + ' ' + team.Number.ToString("00");
                        shortLongTeamNameDictionary[fullTeam] = fullTeam + ' ' + tname;
                    }
                }
            }
            foreach (var dict in directionPossibleNumbers) dict.Value.Sort();

            ShortLongTeamNameDictionary = new ReadOnlyDictionary<string, string>(shortLongTeamNameDictionary);
            DirectionPossibleNumbers = new ReadOnlyDictionary<string, List<int>>(directionPossibleNumbers);
            UnknownPossibleTeams = new ReadOnlyCollection<string>(unknownPossibleTeams);
        }
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