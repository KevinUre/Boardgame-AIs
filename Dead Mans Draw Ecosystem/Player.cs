using Genetics.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeadMansDraw.Ecosystem
{
    public class Player: ISpecimen
    {
        public double Fitness { get; set; }
        public List<int> Scores = new List<int>();
        public int ID;
    }
}
