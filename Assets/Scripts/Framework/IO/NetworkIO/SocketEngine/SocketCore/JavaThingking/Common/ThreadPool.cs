using System;
using System.Collections.Generic;
using System.Threading;
using System.Text.RegularExpressions;

namespace xClient.Common
{
	public class ThreadPool {

    	private static LinkedList<int> remove = new LinkedList<int>();
		//Key is Priority, value is count
	    private static SortedDictionary<int, int> count = new SortedDictionary<int, int>();
	    
	    // 根据优先级的线程池
    	private static SortedDictionary<int, LinkedList<Runnable>> tasks =new SortedDictionary<int, LinkedList<Runnable>>();
    	
		private static int 	task_count      = 0;
	    private static long task_total 		= 0;
	    private static long time_lastadd  	= 0;
		
	    private static Object task_count_locker = new Object();

	    public int priority;

	    public static void LoadConfig()
	    {
            try
            {
                String config = Conf.Get().find("ThreadPool", "config", "(1,2)");
                if (config != null && config != "")
                {
					Regex rx = new Regex(@"\(\s*(\d+)\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);
                    MatchCollection matches = rx.Matches(config);

                    foreach (Match match in matches)
                    {
                        GroupCollection groups = match.Groups;
                        int priority = Convert.ToInt32(groups[1].Value);
                        for (int count = Convert.ToInt32(groups[2].Value); count > 0; count--)
                        {
                            AddThread(priority);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleEx.DebugLog(e.StackTrace);
                ConsoleEx.DebugLog(e.Message);
            }
	    }

	    private ThreadPool(int priority)
	    {
		    this.priority = priority;
		    lock(count)
		    {
                int c = 0;
                if (count.TryGetValue(priority, out c))
                {
                    count[priority] = c + 1;
                }
                else
                {
                    count[priority] = 1;
                }
		    }
	    }

	    public void run()
	    {
		    for (;;)
		    {
			    try
			    {
				    Runnable r = null;
				    LinkedList<Runnable> ll = null;
					if (!tasks.TryGetValue(priority, out ll)) {
						ConsoleEx.DebugLog( string.Format("Task list of priority {0} not found", priority));
                        return;
                    }
                    
                    // 执行线程池里面的线程
				    lock(ll)
				    {
					    while (ll.Count == 0)
					    {
						    Monitor.Wait(ll);
					    }
					    r = ll.Last.Value;
                        ll.RemoveLast();
					    lock( task_count_locker ) 
					    { 
					    	task_count --; 
					    }
				    }
					r.run();

				    // 清除任务
				    lock(remove)
				    {
					    if ( remove.Count != 0 && priority == remove.Last.Value )
					    {
						    remove.RemoveLast();
						    return;
					    }
				    }
			    }
                catch (Exception e) 
                { 
                	ConsoleEx.DebugLog(e.Message); 
                	ConsoleEx.DebugLog(e.StackTrace); 
                }
		    }
	    }

	    public static void AddTask(Runnable r)
	    {
		    LinkedList<Runnable> ll = null;
            if (!tasks.TryGetValue(r.GetPriority, out ll))
		    {
				ConsoleEx.DebugLog( string.Format("ThreadPool thread LinkedList == null: no Match priority {0}", r.GetPriority) );
			    return;
		    }
		    lock(ll)
		    {
			    ll.AddFirst(r);
			    lock(task_count_locker)
			    {
				    task_count ++;
				    task_total ++;
			    }
			    time_lastadd = WatchDog.GetTime();
			    Monitor.Pulse(ll);
		    }
	    }

	    public static long TaskTotal()
	    {
		    lock(task_count_locker)
		    {
			    return task_total;
		    }
	    }

	    public static long TimeLastAdd()
	    {
		    return time_lastadd;
	    }

	    public static void AddThread(int priority)
	    {
			if (!tasks.ContainsKey(priority)) {
                tasks.Add(priority, new LinkedList<Runnable>());
            }
			ConsoleEx.DebugLog( string.Format("Start thread of priority {0}", priority) );
		    
            // 启动线程轮询任务
			new Thread(() =>{ new ThreadPool(priority).run();} ).Start();
	    }

	    public static int ThreadCount()
	    {
		    int sum = 0;
		    lock(count)
		    {
                foreach (KeyValuePair<int, int> kvp in count)
                {
                    sum += kvp.Value;
                }
		    }
		    return sum;
	    }

	    public static int ThreadCount(int priority)
	    {
		    int sum = 0;
		    lock(count)
		    {
			    if (count.ContainsKey(priority))
			    {
				    sum = count[priority];
			    }
		    }
		    return sum;
	    }

	    public static void RemoveThread(int prior)
	    {
		    lock(count)
		    {
                int c = 0;
			    if (count.TryGetValue(prior, out c))
			    {
				    int n = c - 1;
				    if (n > 0)
				    {
					    count[prior] = n;
					    lock(remove)
					    {
                            remove.AddFirst(prior);
					    }
				    }
				    else
				    {
                        count.Remove(prior);
				    }
			    }
		    }
	    }
    }
}
