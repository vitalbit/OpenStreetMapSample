﻿using Mapsui.Widgets;
using Mapsui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapsui.Styles;

namespace MapSampleCommon.CustomWidget
{
    public class CustomWidget : IWidget
    {
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public float MarginX { get; set; } = 20;
        public float MarginY { get; set; } = 20;
        public MRect? Envelope { get; set; }
        public bool HandleWidgetTouched(Navigator navigator, MPoint position)
        {
            navigator.CenterOn(0, 0);
            return true;
        }

        public Color? Color { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
