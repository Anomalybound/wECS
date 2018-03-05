using System;
using System.Collections;
using System.Collections.Generic;
using AgentProcessor.Core;

namespace AgentProcessor.Helper
{
    public class Processors : Dictionary<ISystem, bool>
    {
        public void Add<T>(T Processor) where T : ISystem
        {
            Add(Processor, true);
        }

        public void Execute()
        {
            foreach (var processorPair in this)
            {
                if (processorPair.Value)
                {
                    var processor = processorPair.Key as IExecuteSystem;
                    if (processor != null)
                    {
                        processor.Execute();
                    }
                }
            }
        }
    }
}