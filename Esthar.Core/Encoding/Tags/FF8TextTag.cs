using System;
using System.Globalization;
using System.Text;

namespace Esthar.Core
{
    public sealed class FF8TextTag
    {
        public static string[] PageSeparator = { new FF8TextTag(FF8TextTagCode.Next).ToString() };
        public static string[] LineSeparator = { new FF8TextTag(FF8TextTagCode.Line).ToString() };

        public const int MaxTagLength = 32;

        public FF8TextTagCode Code;
        public Enum Param;

        public FF8TextTag(FF8TextTagCode code, Enum param = null)
        {
            Code = code;
            Param = param;
        }

        public int Write(byte[] bytes, ref int offset)
        {
            bytes[offset++] = (byte)Code;
            if (Param == null)
                return 1;

            bytes[offset++] = (byte)(FF8TextTagParam)Param;
            return 2;
        }

        public int Write(char[] chars, ref int offset)
        {
            StringBuilder sb = new StringBuilder(MaxTagLength);
            sb.Append('{');
            sb.Append(Code);
            if (Param != null)
            {
                sb.Append(' ');
                sb.Append(Param);
            }
            sb.Append('}');

            if (sb.Length > MaxTagLength)
                throw Exceptions.CreateException("Слишком длинное имя тэга: {0}", sb.ToString());

            for (int i = 0; i < sb.Length; i++)
                chars[offset++] = sb[i];

            return sb.Length;
        }

        public static FF8TextTag TryRead(byte[] bytes, ref int offset, ref int left)
        {
            FF8TextTagCode code = (FF8TextTagCode)bytes[offset++];
            left -= 2;
            switch (code)
            {
                case FF8TextTagCode.End:
                case FF8TextTagCode.Next:
                case FF8TextTagCode.Line:
                case FF8TextTagCode.Speaker:
                    left++;
                    return new FF8TextTag(code);
                case FF8TextTagCode.Pause:
                case FF8TextTagCode.Var:
                    return new FF8TextTag(code, (FF8TextTagParam)bytes[offset++]);
                case FF8TextTagCode.Char:
                    return new FF8TextTag(code, (FF8TextTagCharacter)bytes[offset++]);
                case FF8TextTagCode.Key:
                    return new FF8TextTag(code, (FF8TextTagKey)bytes[offset++]);
                case FF8TextTagCode.Color:
                    return new FF8TextTag(code, (FF8TextTagColor)bytes[offset++]);
                case FF8TextTagCode.Dialog:
                    return new FF8TextTag(code, (FF8TextTagDialog)bytes[offset++]);
                case FF8TextTagCode.Term:
                    return new FF8TextTag(code, (FF8TextTagTerm)bytes[offset++]);
                default:
                    left += 2;
                    offset--;
                    return null;
            }
        }

        public static FF8TextTag TryRead(char[] chars, ref int offset, ref int left)
        {
            int oldOffset = offset;
            int oldleft = left;

            string tag, par;
            if (chars[offset++] != '{' || !TryGetTag(chars, ref offset, ref left, out tag, out par))
            {
                offset = oldOffset;
                left = oldleft;
                return null;
            }

            FF8TextTagCode? code = EnumCache<FF8TextTagCode>.TryParse(tag);
            if (code == null)
            {
                offset = oldOffset;
                left = oldleft;
                return null;
            }

            switch (code.Value)
            {
                case FF8TextTagCode.End:
                case FF8TextTagCode.Next:
                case FF8TextTagCode.Line:
                case FF8TextTagCode.Speaker:
                    return new FF8TextTag(code.Value);
                case FF8TextTagCode.Pause:
                case FF8TextTagCode.Var:
                    byte numArg;
                    if (byte.TryParse(par, NumberStyles.Integer, CultureInfo.InvariantCulture, out numArg))
                        return new FF8TextTag(code.Value, (FF8TextTagParam)numArg);
                    break;
                case FF8TextTagCode.Char:
                    FF8TextTagCharacter? charArg = EnumCache<FF8TextTagCharacter>.TryParse(par);
                    if (charArg != null) return new FF8TextTag(code.Value, charArg.Value);
                    break;
                case FF8TextTagCode.Key:
                    FF8TextTagKey? keyArg = EnumCache<FF8TextTagKey>.TryParse(par);
                    if (keyArg != null) return new FF8TextTag(code.Value, keyArg.Value);
                    break;
                case FF8TextTagCode.Color:
                    FF8TextTagColor? colorArg = EnumCache<FF8TextTagColor>.TryParse(par);
                    if (colorArg != null) return new FF8TextTag(code.Value, colorArg.Value);
                    break;
                case FF8TextTagCode.Dialog:
                    FF8TextTagDialog? dialogArg = EnumCache<FF8TextTagDialog>.TryParse(par);
                    if (dialogArg != null) return new FF8TextTag(code.Value, dialogArg.Value);
                    break;
                case FF8TextTagCode.Term:
                    FF8TextTagTerm? termArg = EnumCache<FF8TextTagTerm>.TryParse(par);
                    if (termArg != null) return new FF8TextTag(code.Value, termArg);
                    break;
            }

            offset = oldOffset;
            left = oldleft;
            return null;
        }

        private static bool TryGetTag(char[] chars, ref int offset, ref int left, out string tag, out string par)
        {
            int lastIndex = Array.IndexOf(chars, '}', offset);
            int length = lastIndex - offset + 1;
            if (length < 2)
            {
                tag = null;
                par = null;
                return false;
            }

            left--;
            left -= length;

            int spaceIndex = Array.IndexOf(chars, ' ', offset + 1, length - 2);
            if (spaceIndex < 0)
            {
                tag = new string(chars, offset, length - 1);
                par = string.Empty;
            }
            else
            {
                tag = new string(chars, offset, spaceIndex - offset);
                par = new string(chars, spaceIndex + 1, lastIndex - spaceIndex - 1);
            }

            offset = lastIndex + 1;
            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(MaxTagLength);
            sb.Append('{');
            sb.Append(Code);
            if (Param != null)
            {
                sb.Append(' ');
                sb.Append(Param);
            }
            sb.Append('}');
            return sb.ToString();
        }
    }
}