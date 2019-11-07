using System;
using System.Collections.Generic;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
    public class Schedule : ContainerStepBody
    {
        public long Interval { get; set; }

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            if (context.PersistenceData == null)
            {
                TimeSpan Intervals = new DateTime(Interval) - DateTime.Now;
                return ExecutionResult.Sleep(Intervals, new SchedulePersistenceData() { Elapsed = false });
            }
            
            if (context.PersistenceData is SchedulePersistenceData)
            {
                if (!((SchedulePersistenceData)context.PersistenceData).Elapsed)
                {
                    return ExecutionResult.Branch(new List<object>() { null }, new SchedulePersistenceData() { Elapsed = true });
                }
                
                if (context.Workflow.IsBranchComplete(context.ExecutionPointer.Id))
                {
                    return ExecutionResult.Next();
                }
            
                return ExecutionResult.Persist(context.PersistenceData);
            }
            
            throw new ArgumentException();
        }
    }
}
