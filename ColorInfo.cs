using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LabyrinthGame
{
    public class ColorInfo
    {
        public string Name { get; }
        public Color Color { get; }
        public int Price { get; }
        public bool IsPurchased { get; set; }

        public ColorInfo(string name, Color color, int price, bool isPurchased = false)
        {
            Name = name;
            Color = color;
            Price = price;
            IsPurchased = isPurchased;
        }
    }
}
