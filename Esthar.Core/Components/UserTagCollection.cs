using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;

namespace Esthar.Core
{
    public sealed class UserTagCollection : IEnumerable<UserTag>, INotifyCollectionChanged
    {
        private readonly ConcurrentDictionary<string, UserTag> _dic;

        public UserTagCollection()
        {
            _dic = new ConcurrentDictionary<string, UserTag>();
        }

        public UserTagCollection(IEnumerable<UserTag> tags)
        {
            _dic = new ConcurrentDictionary<string, UserTag>(tags.Select(t => new KeyValuePair<string, UserTag>(t.Name, t)));
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(UserTag tag)
        {
            if (_dic.TryAdd(tag.Name, tag))
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, tag));
        }

        public UserTag Find(string name)
        {
            return _dic.TryGetValue(name);
        }

        public UserTag GetOrCreate(string name)
        {
            UserTag tag;
            if (_dic.TryGetValue(name, out tag))
                return tag;

            tag = new UserTag(name);
            Add(tag);
            return tag;
        }

        public UserTag Remove(UserTag tag)
        {
            UserTag result;
            if (_dic.TryRemove(tag.Name, out result))
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, tag));
            return result;
        }

        public void Clear()
        {
            _dic.Clear();
            InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void TryAdd(IEnumerable<UserTag> tags)
        {
            if (tags == null)
                return;

            bool addeded = false;
            foreach (UserTag tag in tags)
                addeded = _dic.TryAdd(tag.Name, tag) || addeded;

            if (addeded)
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void TryRemove(IEnumerable<UserTag> tags)
        {
            if (tags == null)
                return;

            bool removed = false;
            foreach (UserTag tag in tags)
                removed = _dic.Remove(tag.Name) != null || removed;

            if (removed)
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void TrySync(IEnumerable<UserTag> tags)
        {
            if (tags == null)
            {
                _dic.Clear();
                InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            else
            {
                bool removed = false;
                foreach (UserTag tag in _dic.Values.Except(tags))
                    removed = _dic.Remove(tag.Name) != null || removed;

                bool updated = false;
                foreach (UserTag tag in tags)
                {
                    _dic.AddOrUpdate(tag.Name, tag, (name, old) => UpdateTag(old, tag));
                    updated = true;
                }

                if (removed || updated)
                    InvokeCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private UserTag UpdateTag(UserTag oldTag, UserTag newTag)
        {
            oldTag.Priority = newTag.Priority;
            oldTag.Foreground = newTag.Foreground;
            oldTag.Background = newTag.Background;
            oldTag.Font = newTag.Font;
            oldTag.LocationBindable = newTag.LocationBindable;
            oldTag.MessageBindable = newTag.MessageBindable;
            return oldTag;
        }

        public IEnumerable<UserTag> GetAllTags()
        {
            return _dic.Values.Order();
        }

        public IEnumerable<UserTag> GetLocationTags()
        {
            return GetAllTags().Where(tag => tag.LocationBindable);
        }

        public IEnumerable<UserTag> GetMessageTags()
        {
            return GetAllTags().Where(tag => tag.MessageBindable);
        }

        public int Count
        {
            get { return _dic.Count; }
        }

        public void Save(XmlElement node)
        {
            Exceptions.CheckArgumentNull(node, "node").RemoveAll();

            foreach (UserTag tag in _dic.Values)
            {
                XmlElement child = node.CreateChildElement("UserTag");
                tag.Serialize(child);
            }
        }

        public static UserTagCollection SafeLoad(XmlElement node)
        {
            UserTagCollection tags = new UserTagCollection();
            if (node == null)
                return tags;

            foreach (XmlElement child in node.OfType<XmlElement>())
            {
                UserTag tag = Invoker.SafeInvoke(UserTag.Deserialize, child);
                if (tag != null)
                    tags.Add(tag);
            }

            return tags;
        }

        #region IEnumerable

        public IEnumerator<UserTag> GetEnumerator()
        {
            return GetAllTags().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void InvokeCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
                h(this, args);
        }

        #endregion
    }
}