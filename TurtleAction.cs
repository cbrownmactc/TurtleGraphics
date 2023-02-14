using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtleGraphics
{
    internal class TurtleAction
    {
        public Action ActionToRun { get; set; }
        
        /// <summary>
        /// Extra debugging information.
        /// </summary>
        public string DebugNote { get; set; }

        /// <summary>
        /// Process a subsequent action (if it exists) immediately.
        /// </summary>
        public bool SkipDelay { get; set; } = false;
    }
}
