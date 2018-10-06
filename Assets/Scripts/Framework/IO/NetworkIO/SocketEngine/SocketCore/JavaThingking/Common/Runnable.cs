using System;
using System.Collections.Generic;
using System.Threading;

namespace xClient.Common
{
    public abstract class Runnable
    {
        private int priority;
		public Runnable() { 
        	priority = 0; 
        }
        
		public Runnable(int priority) { 
        	this.priority = priority; 
        }
        
		public int GetPriority { 
			get {
				return priority; 
			}
        }

        public abstract void run();
    }
}
