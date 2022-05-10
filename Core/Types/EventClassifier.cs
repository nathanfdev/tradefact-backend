using Core.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Types
{
    [TypescriptAutoGeneration]
    public class EventClassifier: Enumeration
    {
        public static EventClassifier Planned = new EventClassifier(0, "Planned");
        public static EventClassifier Active = new EventClassifier(1, "Active");
        public static EventClassifier Complete = new EventClassifier(2, "Complete");

        public EventClassifier(int id, string name): base(id, name)
        {
        }
    }
}
