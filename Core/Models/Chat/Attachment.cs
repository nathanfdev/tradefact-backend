using System;

namespace Core.Models
{
    public class Attachment
    {
        public string Title { get; set; }

        public string TitleLink { get; set; }

        public bool? TitleLinkDownloadable { get; set; }
        public string Text { get; set; }

        public string ImageUrl { get; set; }

        public string AuthorName { get; set; }

        public string AuthorIcon { get; set; }

        public string ThumbUrl { get; set; }
        public string Color { get; set; }

        public DateTime? Timestamp { get; set; }

        protected bool Equals(Attachment other)
        {
            return string.Equals(Title, other.Title) && string.Equals(TitleLink, other.TitleLink) && TitleLinkDownloadable == other.TitleLinkDownloadable
                   && string.Equals(Text, other.Text) && string.Equals(ImageUrl, other.ImageUrl) && string.Equals(AuthorName, other.AuthorName)
                   && string.Equals(AuthorIcon, other.AuthorIcon) && string.Equals(ThumbUrl, other.ThumbUrl) && string.Equals(Color, other.Color)
                   && Timestamp.Equals(other.Timestamp);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Attachment)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (TitleLink != null ? TitleLink.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TitleLinkDownloadable.GetHashCode();
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ImageUrl != null ? ImageUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AuthorName != null ? AuthorName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AuthorIcon != null ? AuthorIcon.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ThumbUrl != null ? ThumbUrl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Color != null ? Color.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Timestamp.GetHashCode();
                return hashCode;
            }
        }
    }
}