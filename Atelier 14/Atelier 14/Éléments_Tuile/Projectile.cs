using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA.Éléments_Tuile
{
    public class Projectile : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public BoundingSphere SphèreDeCollision { get; private set; }
        public Projectile(Game game) 
            : base(game)
        {
        }

        public bool EstEnCollision(Personnage personnage)
        {
            return SphèreDeCollision.Intersects(personnage.HitBox);
        }
    }
}
