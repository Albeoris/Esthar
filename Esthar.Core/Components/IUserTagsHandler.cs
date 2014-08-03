using System.Collections.ObjectModel;

namespace Esthar.Core
{
    public interface IUserTagsHandler
    {
        UserTagCollection Tags { get; }
    }
}