namespace Esthar.Core
{
    public sealed class FF8TextComment
    {
        private static readonly string[] LineCommentEnd = {"\r\n", "\n", "{Line}"};
        private static readonly string[] BlockCommentEnd = {"*/"};

        public enum CommentType
        {
            Line = '/',
            Block = '*'
        }

        public readonly CommentType Type;
        public readonly string Value;

        private FF8TextComment(CommentType commentType, string value)
        {
            Type = commentType;
            Value = value;
        }

        public static FF8TextComment TryRead(char[] chars, ref int offset, ref int left)
        {
            if (left < 2 || chars[offset] != '/')
                return null;

            CommentType commentType = (CommentType)chars[offset + 1];
            if (commentType != CommentType.Line && commentType != CommentType.Block)
                return null;

            string finded;
            string value;
            int index = StringHelper.IndexOfAny(chars, offset + 2, left - 2, out finded, commentType == CommentType.Line ? LineCommentEnd : BlockCommentEnd);

            if (index < 0)
            {
                value = new string(chars, offset + 2, left - 2);
                offset = offset + left;
                left = 0;
            }
            else
            {
                if (commentType == CommentType.Line)
                {
                    string prev;
                    if (offset != 0 && StringHelper.IndexOfAny(chars, offset, -6, out prev, LineCommentEnd) < 0)
                        finded = string.Empty;
                }
                int length = index - offset;
                value = new string(chars, offset + 2, length - 2);
                left -= length + finded.Length;
                offset = index + finded.Length;
            }

            return new FF8TextComment(commentType, value);
        }
    }
}