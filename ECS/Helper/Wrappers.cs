using System.Collections.Generic;
using wECS.Core;

namespace wECS.Helper
{
    public class Systems : Dictionary<ISystem, bool>
    {
        public void Add<T>(T System) where T : ISystem
        {
            Add(System, true);
        }

        public void Execute()
        {
            foreach (var systemPair in this)
            {
                if (systemPair.Value)
                {
                    var processor = systemPair.Key as IExecuteSystem;
                    if (processor != null)
                    {
                        processor.Execute();
                    }
                }
            }
        }

        public void FixedExecute()
        {
            foreach (var systemPair in this)
            {
                if (systemPair.Value)
                {
                    var processor = systemPair.Key as IFixedExecuteSystem;
                    if (processor != null)
                    {
                        processor.FixedExecute();
                    }
                }
            }
        }
    }
}