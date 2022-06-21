using System.ComponentModel.Composition;

namespace PluginContractsDll
{
    /// <summary>
    /// We need:
    /// Install-Package System.ComponentModel.Composition -Version 6.0.0
    /// </summary>
    public class PluginService
    {
        [InheritedExport(typeof(IPluginService))]
        public interface IPluginService
        {
            public int Ord { get; set; }
            string InitPlugin();
        }
    }
}
