using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BackgroundTasks
{


    public sealed class Course
    {
        public Course() { }

        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("index")]
        public string Index { get; set; }
        [XmlAttribute("startTime")]
        public string StartTime { get; set; }
        [XmlAttribute("endTime")]
        public string EndTime { get; set; }

        [XmlAttribute("duration")]
        public string Duration { get; set; }
        [XmlAttribute("finished")]
        public string Finished { get; set; }


        [XmlAttribute("teacher")]
        public string Teacher { get; set; }

        [XmlAttribute("room")]
        public string Room { get; set; }

        [XmlAttribute("score")]
        public string Score { get; set; }


        [XmlAttribute("weeklimit")]
        public int Weeklimit { get; set; }

        [XmlAttribute("type")]
        public int Type { get; set; }

    }

    [XmlRoot("schedule")]
    public sealed class Schedule
    {
        [XmlElement(ElementName = "Monday")]
        public Weekday day1 { get; set; }
        [XmlElement(ElementName = "Tuesday")]
        public Weekday day2 { get; set; }
        [XmlElement(ElementName = "Wednesday")]
        public Weekday day3 { get; set; }
        [XmlElement(ElementName = "Thursday")]
        public Weekday day4 { get; set; }
        [XmlElement(ElementName = "Friday")]
        public Weekday day5 { get; set; }
        [XmlElement(ElementName = "Saturday")]
        public Weekday day6 { get; set; }
        [XmlElement(ElementName = "Sunday")]
        public Weekday day7 { get; set; }


    }

    public sealed class Weekday
    {
        [XmlElement(ElementName = "class")]
        public Course[] Courses { get; set; }
    }




   









}
