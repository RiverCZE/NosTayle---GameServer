using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NosTayleGameServer.NosTale.Items.Others
{
    class Locomotion
    {
        internal int morphMale;
        internal int morphFemale;
        internal int speed;

        public Locomotion(int morphMale, int morphFemale, int speed)
        {
            this.morphMale = morphMale;
            this.morphFemale = morphFemale;
            this.speed = speed;
        }

        public static Locomotion GetLocomotion(int id)
        {
            switch(id)
            {
                case 1:
                    return null;
                case 5: //Balai 
                    return new Locomotion(2432, 2433, 21);
                case 6: //Nossi
                    return new Locomotion(2517, 2518, 21);
                case 7: //Licorne blanche
                    return new Locomotion(2526, 2527, 22);
                case 8: //Licorne rose
                    return new Locomotion(2528, 2529, 22);
                case 9: //Licorne noir
                    return new Locomotion(2530, 2531, 50);
                default:
                    return null;
            }
        }

        public int GetMorphByGender(int gender)
        {
            if (gender == 0)
                return this.morphMale;
            return this.morphFemale;
        }
    }
}
