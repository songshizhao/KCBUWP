using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace 课程表UWP.data
{


    public class Course
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
    public class Schedule
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

    public class Weekday
    {
        [XmlElement(ElementName = "class")]
        public ObservableCollection<Course> Courses { get; set; }
    }




   









}
