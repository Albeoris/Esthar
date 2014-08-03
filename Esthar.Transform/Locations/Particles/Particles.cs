using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class Particles : IDisposable
    {
        public SafeHGlobalHandle PMDContent;
        public SafeHGlobalHandle PMPContent;

        public Particles(SafeHGlobalHandle pmdContent, SafeHGlobalHandle pmpContent)
        {
            PMDContent = Exceptions.CheckArgumentNull(pmdContent, "pmdContent");
            PMPContent = Exceptions.CheckArgumentNull(pmpContent, "pmpContent");
        }

        public void Dispose()
        {
            PMDContent.SafeDispose();
            PMPContent.SafeDispose();
        }
    }
}
