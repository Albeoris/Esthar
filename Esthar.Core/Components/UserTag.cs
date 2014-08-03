using System;
using System.Windows.Media;
using System.Xml;

namespace Esthar.Core
{
    public sealed class UserTag : IComparable, ICloneable, IEquatable<UserTag>
    {
        public string Name { get; set; }
        public int Priority { get; set; }
        public FontDescriptor Font { get; set; }
        public Color Foreground { get; set; }
        public Color Background { get; set; }
        public bool LocationBindable { get; set; }
        public bool MessageBindable { get; set; }

        public UserTag(string name)
        {
            Name = Exceptions.CheckArgumentNullOrEmprty(name, "name");
            Font = FontDescriptor.Default;
            Priority = Int32.MaxValue;
            Foreground = Colors.Black;
            Background = Colors.Transparent;
        }

        public void Serialize(XmlElement node)
        {
            Exceptions.CheckArgumentNull(node, "node").RemoveAll();

            node.SetString("Name", Name);
            node.SetInt32("Priority", Priority);
            node.SetString("Foreground", Foreground.ToHexString());
            node.SetString("Background", Background.ToHexString());
            node.SetBoolean("LocationBindable", LocationBindable);
            node.SetBoolean("MessageBindable", MessageBindable);

            Font.Serialize(node.CreateChildElement("Font"));
        }

        public static UserTag Deserialize(XmlElement node)
        {
            UserTag result = new UserTag(node.GetString("Name"))
            {
                Priority = node.FindInt32("Priority") ?? Int32.MaxValue,
                Foreground = (Color)Invoker.SafeInvoke(ColorConverter.ConvertFromString, node.FindString("Foreground"), Colors.Black),
                Background = (Color)Invoker.SafeInvoke(ColorConverter.ConvertFromString, node.FindString("Background"), Colors.Transparent),
                LocationBindable = node.FindBoolean("LocationBindable") ?? false,
                MessageBindable = node.FindBoolean("MessageBindable") ?? false,
                Font = Invoker.SafeInvoke(FontDescriptor.Deserialize, node["Font"], FontDescriptor.Default)
            };

            return result;
        }

        #region ICloneable

        public object Clone()
        {
            return new UserTag(Name)
            {
                Priority = Priority,
                Foreground = Foreground,
                Background = Background,
                LocationBindable = LocationBindable,
                MessageBindable = MessageBindable
            };
        }

        #endregion

        #region IEquatable

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is UserTag && Equals((UserTag)obj);
        }

        public bool Equals(UserTag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(UserTag left, UserTag right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UserTag left, UserTag right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region IComparable

        public int CompareTo(object obj)
        {
            UserTag other = obj as UserTag;
            if (ReferenceEquals(other, null)) return 1;
            if (ReferenceEquals(this, other)) return 0;

            int result = Priority.CompareTo(other.Priority);
            if (result != 0)
                return result;

            return String.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        #endregion
    }
}