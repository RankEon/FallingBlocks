using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FallingBlocks
{
    class Randomizer
    {
        Random rand;

        string[] blockShapes = new string[7] { "lshape", "lshape_inv", "tshape", "cube", "zshape", "zshape_inv", "bar" };

        /// <summary>
        /// Constructor. We create one Random -object, which is retained
        /// (to ensure that we do not get repeatable pattern which would
        /// happen if Random would be re-created frequently as it's pseudo
        /// random -class).
        /// </summary>
        public Randomizer()
        {
            rand = new Random();
        }

        /// <summary>
        /// Randomize block shape from the available blocks (from blockShapes).
        /// </summary>
        /// <returns>Block shape -string</returns>
        public string RandomizeBlock()
        {
            return blockShapes[rand.Next(0, 7)];
        }
    }
}
