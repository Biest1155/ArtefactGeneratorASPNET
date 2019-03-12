using System;
using System.Collections.Generic;
using System.Text;

namespace ArtefactEntities.Models
{
    public class Artefacts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; } // Arcane, fire, ice
        public string Type { get; set; } // weapon, armor, trinket
        public string Description { get; set; } // what is it
        public bool setpiece { get; set; } // Does it belong to a set
    }
}
