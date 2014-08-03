using System;

namespace Esthar.UI
{
    public static class ValueConverters
    {
        public static InverseBoolValueConverter InverseBool
        {
            get { return InverseBoolInstance.Value; }
        }

        public static AllToBoolConverter<bool> AllTrue
        {
            get { return AllTrueInstance.Value; }
        }

        public static AllToBoolConverter<bool> AllFalse
        {
            get { return AllFalseInstance.Value; }
        }

        public static AnyToBoolConverter<bool> AnyTrue
        {
            get { return AnyTrueInstance.Value; }
        }

        private static readonly Lazy<InverseBoolValueConverter> InverseBoolInstance = new Lazy<InverseBoolValueConverter>(() => new InverseBoolValueConverter());
        private static readonly Lazy<AllToBoolConverter<bool>> AllTrueInstance = new Lazy<AllToBoolConverter<bool>>(() => new AllToBoolConverter<bool>(true));
        private static readonly Lazy<AllToBoolConverter<bool>> AllFalseInstance = new Lazy<AllToBoolConverter<bool>>(() => new AllToBoolConverter<bool>(false));
        private static readonly Lazy<AnyToBoolConverter<bool>> AnyTrueInstance = new Lazy<AnyToBoolConverter<bool>>(() => new AnyToBoolConverter<bool>(true));
    }
}