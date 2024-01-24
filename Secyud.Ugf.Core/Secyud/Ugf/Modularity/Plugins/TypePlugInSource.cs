using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Modularity.Plugins
{
    public class TypePlugInSource : IPlugInSource
    {
        protected Type[] ModuleTypes;


        protected TypePlugInSource()
        {
            
        }
        
        /// <summary>
        /// type should be inherited from IUgfModule
        /// </summary>
        /// <param name="moduleTypes"></param>
        public TypePlugInSource(params Type[] moduleTypes)
        {
            ModuleTypes = moduleTypes ?? Type.EmptyTypes;
        }

        public IEnumerable<Type> GetModules()
        {
            return ModuleTypes;
        }
    }
}