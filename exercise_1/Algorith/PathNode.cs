using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace exercise_1.Algorith
{
    class PathNode
    {
        public Point Position { get; set; } //текущая позиция точки
        public int GLenghtPath { get; set; } //длина от текущей до данного узла
        public int HLenght { get; set; } // эвристическая функция от старта и до конца
        public PathNode Parent { get; set; } // откуда пришли в эту точку
        public int FLenghtPath
        { //длина полного расстояния от начала и до конца
            get
            {
                return this.GLenghtPath + this.HLenght;
            }
        }
    }
}
