using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymGenZ.PModels
{
    public class MPackage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Time { get; set; }

        public MPackage(int id, string name, int time)
        {
            Id = id;
            Name = name;
            Time = time;
        }
    }
}

