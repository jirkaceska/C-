using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Database.Utils
{
    class PSObjectComparer : IEqualityComparer<PSObject>
    {
        public bool Equals(PSObject x, PSObject y)
        {
            return Object.ReferenceEquals(x, y) || (
                x != null &&
                y != null &&
                x.BaseObject.Equals(y.BaseObject)
            );
        }

        public int GetHashCode(PSObject obj)
        {
            return obj.BaseObject.GetHashCode();
        }
    }

    class PSScriptResultComparer : IEqualityComparer<IEnumerable<PSObject>>
    {
        private readonly IEqualityComparer<PSObject> comparer = new PSObjectComparer();
        public bool Equals(IEnumerable<PSObject> x, IEnumerable<PSObject> y)
        {
            bool reference = Object.ReferenceEquals(x, y);
            var diff = x.Except(y, comparer);
            return Object.ReferenceEquals(x, y) || (
                x != null && 
                y != null && 
                x.Count() == y.Count() && 
                !x.Except(y, comparer).Any()
            );
        }

        public int GetHashCode(IEnumerable<PSObject> collection)
        {
            if (collection == null)
            {
                return 0;
            }

            return collection.Select(obj => 
                obj == null 
                    ? 0 
                    : obj.BaseObject.GetHashCode()
            ).Aggregate(0, (a, b) => a + b);
        }
    }
}
