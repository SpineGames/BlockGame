using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockGame.Utilities
{
    public class Octree<T>
    {
        Octree<T> _parent;
        Octree<T>[] _children;
        int _level = 1;
        Point3 _min;
        Point3 _max;

        T _tag;

        public Octree(Point3 min, Point3 max, int level, T tag)
        {

        }

        public void SubDivide()
        {
            _children = new Octree<T>[8];
            //_children[0] = new Octree<T>()
        }

        public Octree<T> GetNode(int x, int y, int z)
        {
            if (_level > 1)
            {

            }

            return this;
        }
    }
}
