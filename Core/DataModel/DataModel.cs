﻿using System;

namespace Core.DataModel
{
    public class Icon
    {
        public string Graph { get; set; }
    }
    public class Item
    {
        public string Font { get; set; }
        public string Graph { get; set; }
    }
    public class URI
    {
        public string Intro { get; set; }
        public string Content { get; set; }
    }
    public class Feature
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }
    }

}
