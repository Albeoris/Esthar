using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esthar.Core
{
    public static class FormatHelper
    {
        public static string BytesFormat(double value)
        {
            int i = 0;
            while ((value > 1024))
            {
                value /= 1024;
                i++;
            }

            switch (i)
            {
                case 0:
                    return string.Format("{0:F2} {1}", value, Lang.MeasurementsByteAbbr);
                case 1:
                    return string.Format("{0:F2} {1}", value, Lang.MeasurementsKByteAbbr);
                case 2:
                    return string.Format("{0:F2} {1}", value, Lang.MeasurementsMByteAbbr);
                case 3:
                    return string.Format("{0:F2} {1}", value, Lang.MeasurementsGByteAbbr);
                case 4:
                    return string.Format("{0:F2} {1}", value, Lang.MeasurementsTByteAbbr);
                case 5:
                    return string.Format("{0:F2} {1}", value, Lang.MeasurementsPByteAbbr);
                case 6:
                    return string.Format("{0:F2} {1}", value, Lang.MeasurementsEByteAbbr);
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }
    }
}
