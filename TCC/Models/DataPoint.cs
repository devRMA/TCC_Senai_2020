﻿using System;
using System.Runtime.Serialization;

namespace TCC.Models
{
    [DataContract]
    public class DataPoint
    {
        public DataPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        [DataMember(Name = "x")]
        public Nullable<double> X = null;

        [DataMember(Name = "y")]
        public Nullable<double> Y = null;
    }
}
