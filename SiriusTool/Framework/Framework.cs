using System;     
using SiriusTool.Model;

namespace SiriusTool.Framework
{
    public delegate void ExceptionOccuredEventHandler(Exception exception);

    public delegate void TeamSelectedEventHandler(string selectedTeam);

    public delegate void ItemClickedEventHandler(Event sender);
}