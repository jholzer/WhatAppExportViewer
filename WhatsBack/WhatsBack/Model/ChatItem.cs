using System;
using System.Collections.Generic;
using System.Text;

namespace WhatsBack.Model
{
    public class ChatItem
    {
        public ChatItem(string name, string text, DateTime timeStamp, string tag, string sourceFile)
        {
            Name = name;
            Text = text;
            TimeStamp = timeStamp;
            Tag = tag;
            SourceFile = sourceFile;
        }

        public string Name { get; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; }
        public string Tag { get; }
        public string SourceFile { get; }

        public void AppendText(string text)
        {
            Text = $"{Text}\n{text}";
        }

        #region Equality comparer

        private sealed class NameTextTimeStampEqualityComparer : IEqualityComparer<ChatItem>
        {
            public bool Equals(ChatItem x, ChatItem y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Name.Trim() == y.Name.Trim() 
                       && x.Text.Trim() == y.Text.Trim() 
                       && x.TimeStamp.Equals(y.TimeStamp);
            }

            public int GetHashCode(ChatItem obj)
            {
                unchecked
                {
                    var hashCode = (obj.Name != null ? obj.Name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Text != null ? obj.Text.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.TimeStamp.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<ChatItem> Comparer { get; } = new NameTextTimeStampEqualityComparer();

        #endregion
    }
}
