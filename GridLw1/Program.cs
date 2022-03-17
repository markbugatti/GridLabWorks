using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GridLw1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var taskScheduler = new TaskScheduler();

            taskScheduler.AssignTasksToProcessor();

            Console.ReadKey();
        }
    }

    public class MyTask
    {
        public readonly Guid Id = Guid.NewGuid();
        public int Duration { get; set; }
    }

    public class MyProcessor
    {
        public readonly Guid Id = Guid.NewGuid();
    }

    public class TaskScheduler
    {
        private const int minProcessorCount = 2;
        private const int maxProcessorCount = 8;

        private const int minTaskCount = 1;
        private const int maxTaskCount = 30;

        private const int minTaskDuration = 5;
        private const int maxTaskDuration = 20;

        private readonly Dictionary<MyProcessor, List<MyTask>> scheduler;

        private List<MyTask> taskList;
        private List<MyProcessor> processorList;

        public TaskScheduler()
        {
            Random rnd = new Random();
            var taskCount = rnd.Next(minTaskCount, maxTaskCount);
            var processorCount = rnd.Next(minProcessorCount, maxProcessorCount);

            // initialize task list
            taskList = new List<MyTask>();

            for (int i = 0; i < taskCount; i++)
            {
                taskList.Add(new MyTask { Duration = rnd.Next(minTaskDuration, maxTaskDuration) });
            }

            var tasks = new ReadOnlyCollection<MyTask>(taskList);

            // initialize processor list
            processorList = new List<MyProcessor>();

            for (int i = 0; i < processorCount; i++)
            {
                processorList.Add(new MyProcessor());
            }

            scheduler = new Dictionary<MyProcessor, List<MyTask>>();

            processorList.ForEach(x => scheduler[x] = new List<MyTask>());
        }

        public void AssignTasksToProcessor()
        {
            taskList.Sort((x, y)  => y.Duration.CompareTo(x.Duration));

            // distribute tasks accross processors using task scheduler
            foreach (var task in taskList)
            {
                // take current task from big to small (descending)
                // check all processors and select that has minimum load
                // assign task to it.
                var orderedScheduler = scheduler.OrderBy(x => x.Value.Sum(p => p.Duration));
                var minLoadTask = orderedScheduler.First();

                minLoadTask.Value.Add(task);
            }

            // output task list with
            foreach (var processor in scheduler.Keys)
            {
                Console.WriteLine($"Processor with id {processor.Id} has next tasks: ");
                
                foreach (var task in scheduler[processor])
                {
                    Console.WriteLine($"task id: {task.Id}, task duration: {task.Duration};");
                }
                var totalTasksTime = scheduler[processor].Sum(p => p.Duration);
                Console.WriteLine($"Total tasks time: {totalTasksTime}");

                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }

}
