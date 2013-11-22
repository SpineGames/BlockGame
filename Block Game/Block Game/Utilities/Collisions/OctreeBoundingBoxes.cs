using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Block_Game.Utilities.Collisions
{
    public class OctreeBoundingBox
    {
        OctreeBoundingBox[] subBoxes;
        BoundingBox collision;

        public OctreeBoundingBox(int size = 1)
        {
            if(size > 1)
            {
                subBoxes = new OctreeBoundingBox[8];

                for (int i = 0; i < 8; i++)
                    subBoxes[i] = new OctreeBoundingBox(size - 1);
            }
            collision = new BoundingBox(new Vector3(0), new Vector3(size));
        }

        //public RayCollision RayCast(Ray ray)
        //{
        //    if (subBoxes != null)
        //    {
        //        for (int i = 0; i < 8; i++)
        //        {
                    
        //        }
        //    }

        //    BoundingBox b = collision;
        //}
    }
}
